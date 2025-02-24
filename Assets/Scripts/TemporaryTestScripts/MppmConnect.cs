using Unity.Netcode;
using UnityEngine;
using Unity.Multiplayer.Playmode;
using System.Linq;

/// A MonoBehaviour to automatically start Netcode for GameObjects
/// clients, hosts, and servers
public class MppmConnect : MonoBehaviour
{
    void Start()
    {
        var mppmTag = CurrentPlayer.ReadOnlyTags();
        var networkManager = NetworkManager.Singleton;
        if (mppmTag.Contains("Server"))
        {
            networkManager.StartServer();
        }
        else if (mppmTag.Contains("Host"))
        {
            networkManager.StartHost();
        }
        else if (mppmTag.Contains("Client"))
        {
            networkManager.StartClient();
        }
    }
}