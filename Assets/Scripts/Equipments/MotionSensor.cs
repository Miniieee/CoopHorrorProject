using Interfaces;
using UnityEngine;

public class MotionSensor : MonoBehaviour, IActivatable
{
    public void Activate(ulong networkObjectId)
    {
        Debug.LogWarning("Motion sensor activated.");
    }
}