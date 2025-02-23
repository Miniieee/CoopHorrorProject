using System;
using ScriptableObjectsScripts;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class FlashlightToggle : NetworkBehaviour
{
    [Title("Spotlight reference")]
    [SerializeField] private Light flashlight;
    
    [Title("Event Bus")]
    [SerializeField] private FlashlightEventSO flashlightActivate;

    void Start()
    {
        flashlight.enabled = false;
        flashlightActivate.OnFlashlightEvent += ToggleFlashlight;
    }

    private void ToggleFlashlight(ulong networkID)
    {
        if (NetworkObjectId != networkID) { return; }
        flashlight.enabled = !flashlight.enabled;
    }

    public override void OnDestroy()
    {
        flashlightActivate.OnFlashlightEvent -= ToggleFlashlight;
    }

    private void OnDisable()
    {
        flashlightActivate.OnFlashlightEvent -= ToggleFlashlight;
    }
}
