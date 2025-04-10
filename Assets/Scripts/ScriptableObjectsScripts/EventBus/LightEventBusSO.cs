using System;
using UnityEngine;

namespace ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "LightEventSO", menuName = "EventBuses/LightEventSO")]
    public class LightEventBusSO : ScriptableObject
    {
        public event Action<bool> OnLightStateChanged;
    
        public void ChangeLightState( bool state)
        {
            OnLightStateChanged?.Invoke(state);
        }
    }
}
