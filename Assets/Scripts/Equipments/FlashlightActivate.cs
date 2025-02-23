using Interfaces;
using ScriptableObjectsScripts;
using UnityEngine;

namespace Equipments
{
    public class FlashlightActivate : MonoBehaviour, IActivatable
    {
        [SerializeField] private FlashlightEventSO flashlightEvent;

        public void Activate(ulong networkObjectId)
        {
            flashlightEvent.ToggleFlashlight(networkObjectId);
        }

    }
}