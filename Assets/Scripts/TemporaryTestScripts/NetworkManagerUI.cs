using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    
    void Awake()
    {
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }

}
