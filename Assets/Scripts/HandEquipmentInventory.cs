using Unity.Netcode;
using UnityEngine;

public class HandEquipmentInventory : NetworkBehaviour
{
    private PlayerControls playerControls;

    private int selectedEquipmentIndex = 0;
    private int maxNumberOfEquipments = 3;
    private int currentlySelectedEquipmentIndex;
    private Transform handEquipmentTransform;

    [SerializeField] private Transform handEquipmentFirstPersonTransform;
    [SerializeField] private Transform handEquipmentThirdPersonTransform;


    public override void OnNetworkSpawn()
    {
        playerControls.Player.EquipmentSwich.performed += ctx => SelectEquipment(playerControls.Player.EquipmentSwich.ReadValue<float>());
    }

    public override void OnNetworkDespawn()
    {
        playerControls.Player.EquipmentSwich.performed -= ctx => SelectEquipment(playerControls.Player.EquipmentSwich.ReadValue<float>());
    }


    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        selectedEquipmentIndex = 0;

        if (IsOwner)
        {
            handEquipmentTransform = handEquipmentFirstPersonTransform;
        }
        else
        {
            handEquipmentTransform = handEquipmentThirdPersonTransform;
        }
    }

    public void SelectEquipment(float selectedEquipmentChangeDirectionValue)
    {
        if (!IsOwner) { return; }

        if (handEquipmentTransform.childCount == 0) { return; }
        ActivateSelectedEquipment(selectedEquipmentChangeDirectionValue);
        ActivateSelectedEquipmentRpc(selectedEquipmentChangeDirectionValue);
    }

    private void ActivateSelectedEquipment(float selectedEquipmentChangeDirectionValue)
    {
        int i = 0;
        currentlySelectedEquipmentIndex = GetSelectedEquipmentIndex(selectedEquipmentChangeDirectionValue);

        foreach (Transform equipment in handEquipmentTransform)
        {
            equipment.gameObject.SetActive(i == currentlySelectedEquipmentIndex);
            i++;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ActivateSelectedEquipmentRpc(float selectedEquipmentChangeDirectionValue)
    {
        if (IsOwner) { return; }
        ActivateSelectedEquipment(selectedEquipmentChangeDirectionValue);
    }

    public GameObject ActiveHandEquipment()
    {
        foreach (Transform equipment in handEquipmentTransform)
        {
            if (equipment?.gameObject.activeSelf == true)
            {
                return equipment.gameObject;
            }
        }

        return null;
    }

    private int GetSelectedEquipmentIndex(float selectedEquipmentChangeDirectionValue)
    {
        //round float to integer and then show change direction so the float value doesn't matter 
        int equipmentIndexChange = Mathf.RoundToInt(f: Mathf.Sign(f: selectedEquipmentChangeDirectionValue));
        
        selectedEquipmentIndex += equipmentIndexChange;

        maxNumberOfEquipments = handEquipmentTransform.childCount;

        if (selectedEquipmentIndex >= maxNumberOfEquipments)
        {
            selectedEquipmentIndex = 0;
        }
        else if (selectedEquipmentIndex < 0)
        {
            selectedEquipmentIndex = maxNumberOfEquipments - 1;
        }

        Debug.Log(message: selectedEquipmentIndex);
        return selectedEquipmentIndex;
    }

    private void OnEnable()
    {
        playerControls.Enable();

    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
