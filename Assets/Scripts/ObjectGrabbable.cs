using ScriptableObjectsScripts;
using Unity.Netcode;
using UnityEngine;


public class ObjectGrabbable : MonoBehaviour, IInteractable
{
    public EquipmentSO equipmentSO;

    public EquipmentSO GetEquipmentSO()
    {
        return equipmentSO;
    }

}
