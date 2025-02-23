using Interfaces;
using Unity.Netcode;
using UnityEngine;

public class PlayerEquipmentActivate : NetworkBehaviour
{
    private PlayerControls playerControls;
    private HandEquipmentInventory handEquipmentInventory;

    private void Awake()
    {
        playerControls = new PlayerControls();
        handEquipmentInventory = GetComponent<HandEquipmentInventory>();
    }

    void Start()
    {
        playerControls.Player.Activate.performed += ctx => ActivateEquipment();
    }

    private void ActivateEquipment()
    {
        if (!IsOwner) { return; }

        GameObject currentEquipment = handEquipmentInventory.ActiveHandEquipment();

        if (currentEquipment == null) { return; }

        if (currentEquipment.TryGetComponent(out IActivatable activatable))
        {
            activatable.Activate(NetworkObjectId);
        }

        ActivateEquipmentRpc(NetworkObjectId);
    }

    [Rpc(SendTo.Everyone)]
    private void ActivateEquipmentRpc(ulong playerNetworkObjectId)
    {
        if (IsOwner && NetworkObjectId == playerNetworkObjectId) { return; }

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerNetworkObjectId, out NetworkObject playerNetObj))
        {
            GameObject playerObject = playerNetObj.gameObject;

            HandEquipmentInventory handEquipmentInventory = playerObject.GetComponent<HandEquipmentInventory>();

            if (handEquipmentInventory == null)
            {
                Debug.LogError("Player does not have HandEquipmentInventory component.");
                return;
            }

            GameObject currentEquipment = handEquipmentInventory.ActiveHandEquipment();

            if (currentEquipment == null)
            {
                Debug.LogError("Player does not have active equipment.");
                return;
            }

            if (currentEquipment.TryGetComponent<IActivatable>(out var activatable))
            {
                activatable.Activate(playerNetworkObjectId);
            }
            else
            {
                Debug.LogError("Equipment does not implement IActivatable interface.");
            }
        }
        else
        {
            Debug.LogError("Could not find player with NetworkObjectId: " + playerNetworkObjectId);
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.Activate.performed -= ctx => ActivateEquipment();
    }
}
