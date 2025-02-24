using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaTemperature : MonoBehaviour
{
    [SerializeField] private float temperature = 20.0f;
    public void SetTemperature(float newTemperature)
    {
        temperature = newTemperature;
    }

    #region Getters

    public float GetTemperature() => temperature;

    #endregion
}