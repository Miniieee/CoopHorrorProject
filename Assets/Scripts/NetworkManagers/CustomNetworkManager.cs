using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CustomNetworkManager : NetworkManager
{
    private SpawnPoints spawnPoints;

    private void Start()
    {   
        this.NetworkConfig.ConnectionApproval = true;
        this.NetworkConfig.ConnectionData = new byte[0];

        spawnPoints = GetComponent<SpawnPoints>();

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;

        Vector3 spawnPosition = spawnPoints.GetSpawnPoint((int)request.ClientNetworkId);
        response.Position = spawnPosition;

        Debug.Log("Player " + request.ClientNetworkId + " position is: " + spawnPosition);
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }
}
