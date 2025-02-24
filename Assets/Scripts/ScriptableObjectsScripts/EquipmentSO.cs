using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "EquipmentSO", menuName = "ScriptableObjects/EquipmentSO")]
    public class EquipmentSO : ScriptableObject
    {
        public GameObject equipmentNetworkPrefab;
        public GameObject equipmentThirdPersonPrefab;
        public GameObject equipmentFirstPersonPrefab;
        public string equipmentName;

        [FormerlySerializedAs("detectionRange")]
        public float range;
        public float activeTime;
        public float lerpSpeed;
    }
}
