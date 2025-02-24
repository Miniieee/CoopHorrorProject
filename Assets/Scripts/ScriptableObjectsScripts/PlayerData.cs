using Sirenix.OdinInspector;
using UnityEngine;
using Enums;

namespace ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerDataSO", order = 0)]
    public class PlayerData : SerializedScriptableObject
    {
        [Space(10)]
        [Title("Player Type", "Choose player or ghost to set specific properties")]
        [EnumToggleButtons] // Adds toggle buttons for the enum in the inspector
        public PlayerType playerType;

        [Space(10)]
        [Title("Movement Settings", "Shared settings for all player types")]
        public float playerSpeed = 2.0f;
        public float sprintSpeed = 5.0f;
        public float crouchSpeed = 1.0f;
        
        [Space(10)]
        [Title("Ghost Types", "Choose the ghost type to set specific properties")]
        [ShowIf("playerType", PlayerType.Ghost)]
        public GhostType ghostType;
        
        [Title("Ghost Activity Levels", "Intensity values for ghost interactions")]
        [ShowIf("playerType", PlayerType.Ghost)]
        [TabGroup("Ghost Properties", "Activity Levels")]
        [Range(1f, 5f), Tooltip("Represents the EMF activity intensity detected by the reader.")]
        public float emfIntensity = 1f;

        [TabGroup("Ghost Properties", "Activity Levels")]
        [ShowIf("playerType", PlayerType.Ghost)]
        [Range(-10f, 5f), Tooltip("Temperature anomaly value measured in degrees Celsius.")]
        public float temperatureAnomaly = 0f;
        
        [ShowIf("playerType", PlayerType.Ghost)]
        [Title("Ghost Ability Toggles", "Enable or disable specific ghost abilities")]
        public bool isSpiritBoxToggle;
        [ShowIf("playerType", PlayerType.Ghost)]
        public bool isGhostWritingToggle;
        [ShowIf("playerType", PlayerType.Ghost)]
        public bool isFingerPrintsToggle;
    }
}
