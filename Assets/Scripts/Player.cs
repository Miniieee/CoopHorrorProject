using System.Collections.Generic;
using ScriptableObjectsScripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    [Title("Player Data Scriptable Object", "Set the player data here")]
    [SerializeField] private PlayerData playerData;
    
    [Title("Internal objects only", "These objects found inside the player prefab")]
    [SerializeField] private Transform firstPersonTarget;
    [SerializeField] private Transform thirdPersonTarget;
    [SerializeField] private SetObjectPosition refEquipmentRig;

    private CameraRotationFollower playerVisuals;

    [SerializeField] private List<Vector3> spawnpoints = new List<Vector3>();

    private void Awake()
    {
        playerVisuals = GetComponentInChildren<CameraRotationFollower>();
    }

    private void Start()
    {
        if (IsOwner)
        {
            int layerNumber = LayerMask.NameToLayer("Hide");
            SetLayerRecursively(playerVisuals.gameObject, layerNumber);
            refEquipmentRig.SetTargetPosition(firstPersonTarget);
        }
        else
        {
            int layerNumber = LayerMask.NameToLayer("Player");
            SetLayerRecursively(playerVisuals.gameObject, layerNumber);

            refEquipmentRig.SetTargetPosition(thirdPersonTarget);
        }
    }

    public override void OnNetworkSpawn()
    {
        transform.position = spawnpoints[(int)OwnerClientId];
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
        {
            return;
        }

        // Set the layer of the current object
        obj.layer = newLayer;

        // Recursively set the layer on all child objects
        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
    }
    
    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}
