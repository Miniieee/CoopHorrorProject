
using ScriptableObjectsScripts;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PlayerPickup : NetworkBehaviour
{
    private PlayerControls playerControls;
    private HandEquipmentInventory handEquipmentInventory;
    private int selectedEquipmentIndex;

    private GameObject pickedUpObject;
    private GameObject spawnedObject;

    [Title("Intrenal transform references")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform objectGrabPointFirstPersonTransform;
    [SerializeField] private Transform objectGrabPointThirdPersonTransform;
    
    [Title("Settings")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private int maxNumberOfEquipments = 3;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float dropStrength = 1f;


    public override void OnNetworkSpawn()
    {
        playerControls.Player.Interact.performed += ctx => OnPickup();
        playerControls.Player.Drop.performed += ctx => OnDrop();

        selectedEquipmentIndex = 0;
    }

    public override void OnNetworkDespawn()
    {
        playerControls.Player.Interact.performed -= ctx => OnPickup();
        playerControls.Player.Drop.performed -= ctx => OnDrop();
    }


    private void OnPickup()
    {
        if (!IsOwner) return;
        if (IsInventoryFull()) return;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit raycastHit, pickupRange, pickupLayer))
        {
            if (raycastHit.transform.TryGetComponent(out ObjectGrabbable interactableObject))
            {
                Debug.Log("Pickup");

                NetworkObject pickedupNetworkObject = interactableObject.gameObject.GetComponent<NetworkObject>();

                EquipmentSO pickedUpEquipmentSO = interactableObject.equipmentSO;
                GameObject objectToPickup = pickedUpEquipmentSO.equipmentFirstPersonPrefab;


                if (pickedUpEquipmentSO == null)
                {
                    Debug.LogError("EquipmentSO is null");
                    return;
                }

                ulong pickedUpObjectNetworkID = pickedupNetworkObject.NetworkObjectId;

                SpawnPlaceholderObjectServerRpc(pickedUpObjectNetworkID);
                SpawnPlaceholderObject(objectToPickup);
                AddToHandInventory();
            }
        }
    }

    private void SpawnPlaceholderObject(GameObject objectToPickup)
    {
        if (IsOwner)
        {
            pickedUpObject = Instantiate(objectToPickup, objectGrabPointFirstPersonTransform.position, objectGrabPointFirstPersonTransform.rotation, objectGrabPointFirstPersonTransform);
        }
        else
        {
            pickedUpObject = Instantiate(objectToPickup, objectGrabPointThirdPersonTransform.position, objectGrabPointThirdPersonTransform.rotation, objectGrabPointThirdPersonTransform);
        }
    }

    [ServerRpc]
    public void SpawnPlaceholderObjectServerRpc(ulong pickedUpNetworkObjectId)
    {
        NetworkObject pickedUpNetworkObjectOnServer = NetworkManager.SpawnManager.SpawnedObjects[pickedUpNetworkObjectId];
        SpawnPlaceholderObjectClientRpc(pickedUpNetworkObjectOnServer);

        //despawn at last to have a reference in ClientRpc
        pickedUpNetworkObjectOnServer.Despawn();
    }

    [ClientRpc]
    public void SpawnPlaceholderObjectClientRpc(NetworkObjectReference pickedUpNetworkObjectReference)
    {
        if (IsOwner) return;

        pickedUpNetworkObjectReference.TryGet(out NetworkObject objectToSpawnNetworkObject);

        GameObject objectToSpawnGameObject = objectToSpawnNetworkObject.GetComponent<NetworkObject>().gameObject;
        GameObject objectToSpawnPrefabSO = objectToSpawnGameObject.GetComponent<ObjectGrabbable>().equipmentSO.equipmentThirdPersonPrefab;

        SpawnPlaceholderObject(objectToSpawnPrefabSO);
        AddToHandInventory();
    }

    private void AddToHandInventory()
    {
        handEquipmentInventory.SelectEquipment(selectedEquipmentIndex);
        selectedEquipmentIndex++;
    }

    private void OnDrop()
    {
        if (!IsOwner || selectedEquipmentIndex < 0) return;

        selectedEquipmentIndex--;

        pickedUpObject = handEquipmentInventory.ActiveHandEquipment();
        if (pickedUpObject == null) return;

        OnDropServerRpc();
        Destroy(pickedUpObject);
        handEquipmentInventory.SelectEquipment(selectedEquipmentIndex);
    }

    [Rpc(SendTo.Server)]
    public void OnDropServerRpc()
    {
        pickedUpObject = handEquipmentInventory.ActiveHandEquipment();
        GameObject objectToSpawn = pickedUpObject.GetComponent<ObjectGrabbable>().equipmentSO.equipmentNetworkPrefab;
        spawnedObject = Instantiate(objectToSpawn, objectGrabPointFirstPersonTransform.position, objectGrabPointFirstPersonTransform.rotation, objectGrabPointFirstPersonTransform);

        spawnedObject.GetComponent<NetworkObject>().Spawn();
        spawnedObject.GetComponent<Rigidbody>().AddForce(spawnedObject.transform.forward * dropStrength, ForceMode.Impulse);

        OnDropClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnDropClientRpc()
    {
        if (IsOwner) return;

        pickedUpObject = handEquipmentInventory.ActiveHandEquipment();
        Destroy(pickedUpObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * pickupRange);
    }


    private void Awake()
    {
        playerControls = new PlayerControls();
        handEquipmentInventory = GetComponent<HandEquipmentInventory>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private bool IsInventoryFull()
    {
        return selectedEquipmentIndex >= maxNumberOfEquipments;
    }

}
