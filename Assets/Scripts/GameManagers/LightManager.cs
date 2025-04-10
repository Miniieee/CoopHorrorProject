using System;
using ScriptableObjectsScripts;
using UnityEngine;
using UnityEngine.Serialization;

public class LightManager : MonoBehaviour
{
    [SerializeField] private LightEventBusSO player1LightEventBus;
    [SerializeField] private LightEventBusSO player2LightEventBus;
    
    public void TogglePlayer1Light(bool state)
    {
        player1LightEventBus.ChangeLightState(state);
    }
    
    public void TogglePlayer2Light(bool state)
    {
        player2LightEventBus.ChangeLightState(state);
    }
}
