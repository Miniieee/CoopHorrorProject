using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using UI_Scripts;
// Add this import for .FirstOrDefault()
using System.Linq;

public class LobbyServices : MonoBehaviour
{
    public static LobbyServices Instance;

    private const int MAX_PLAYER_AMOUNT = 5;
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    private Lobby joinedLobby;
    private float heartbeatTimer;

    [SerializeField] private CharacterSelectionUI characterSelectionUI;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        InitializeUnityAutentication();
    }

    private async void InitializeUnityAutentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 99999).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER_AMOUNT - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER_AMOUNT, createLobbyOptions);

            // --- Relay Allocation ---
            Allocation allocation = await AllocateRelay();
            if (allocation == null)
            {
                Debug.LogError("Failed to create Relay allocation!");
                return;
            }

            // Get the Relay join code
            string relayJoinCode = await GetRelayJoinCode(allocation);

            // Update the Lobby with the Relay join code
            await LobbyService.Instance.UpdateLobbyAsync(
                joinedLobby.Id,
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KEY_RELAY_JOIN_CODE,
                            new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                        }
                    }
                }
            );

            // ---- Manually build RelayServerData using the chosen endpoint for "udp" ----
            var endpoint = allocation.ServerEndpoints.FirstOrDefault(e => e.ConnectionType == "udp");
            if (endpoint == null)
            {
                Debug.LogError("No UDP endpoint found in the Relay allocation!");
                return;
            }

            // For a host, the hostConnectionData is typically the same as connectionData
            RelayServerData relayServerData = new RelayServerData(
                endpoint.Host,
                (ushort)endpoint.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.ConnectionData, // host also uses the same for hostConnectionData
                allocation.Key,
                endpoint.Secure
            );

            // Set Relay server data into the UnityTransport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // Start as host
            NetworkManager.Singleton.StartHost();

            Debug.Log("Lobby has been created with Relay and Host started!");

            // Show the updated Lobby details
            characterSelectionUI.ShowLobbyDetails(Instance.GetLobby());
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            // Join Relay
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            if (joinAllocation == null)
            {
                Debug.LogError("Failed to join Relay allocation!");
                return;
            }

            // ---- Pick "udp" endpoint from the joinAllocation ----
            var endpoint = joinAllocation.ServerEndpoints.FirstOrDefault(e => e.ConnectionType == "udp");
            if (endpoint == null)
            {
                Debug.LogError("No UDP endpoint found in the joinAllocation!");
                return;
            }

            RelayServerData relayServerData = new RelayServerData(
                endpoint.Host,
                (ushort)endpoint.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                joinAllocation.Key,
                endpoint.Secure
            );

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

            Debug.Log("Joined the Lobby via QuickJoin, Relay client started.");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            // Join Relay
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            if (joinAllocation == null)
            {
                Debug.LogError("Failed to join Relay allocation!");
                return;
            }

            // ---- Pick "udp" endpoint from the joinAllocation ----
            var endpoint = joinAllocation.ServerEndpoints.FirstOrDefault(e => e.ConnectionType == "udp");
            if (endpoint == null)
            {
                Debug.LogError("No UDP endpoint found in the joinAllocation!");
                return;
            }

            RelayServerData relayServerData = new RelayServerData(
                endpoint.Host,
                (ushort)endpoint.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                joinAllocation.Key,
                endpoint.Secure
            );

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

            Debug.Log("Joined the Lobby by code, Relay client started.");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (!IsLobbyHost()) return;

        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer <= 0f)
        {
            float heartbeatTimerMax = 15f;
            heartbeatTimer = heartbeatTimerMax;

            await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null &&
               joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void DeleteLobby()
    {
        if (joinedLobby == null) return;

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
