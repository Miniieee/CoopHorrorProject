using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class RelayServices : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("join code: " + joinCode);
            
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("joining with " +  joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }    
}
