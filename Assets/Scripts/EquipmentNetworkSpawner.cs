using System.Collections.Generic;
using ScriptableObjectsScripts;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class EquipmentNetworkSpawner : NetworkBehaviour
{
    [Title("Scene objects only")]
    [SerializeField] private EquipmentListSO equipmentList;
    [SerializeField] private List<Transform> equipmentListTransform;
    private List<EquipmentSO> equipmentSOs = new List<EquipmentSO>();
    public override void OnNetworkSpawn()
    {
        foreach (var equipmentSo in equipmentList.equipmentSOList)
        {
            equipmentSOs.Add(equipmentSo);
        }

        if (!IsServer) return;

        for (int i = 0; i < equipmentSOs.Count; i++)
        {
            GameObject spawnedObject = Instantiate(equipmentSOs[i].equipmentNetworkPrefab.gameObject, equipmentListTransform[i].position, equipmentListTransform[i].rotation);
            spawnedObject.GetComponent<ObjectGrabbable>().equipmentSO = equipmentSOs[i];
            spawnedObject.GetComponent<NetworkObject>().Spawn();
        }

    }
}
