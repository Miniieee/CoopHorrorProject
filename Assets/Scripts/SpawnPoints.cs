using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private List<Vector3> spawnpoints = new List<Vector3>();
    
    public Vector3 GetSpawnPoint(int index)
    {
        return spawnpoints[index];
    }
}
