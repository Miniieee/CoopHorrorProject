using ScriptableObjectsScripts;
using UnityEngine;
using Enums;
public class Ghost : MonoBehaviour
{
    private PlayerData playerData;
    private Player player;
    private float emfValue;
    private float temperatureValue;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        playerData = player.GetPlayerData();

        if (playerData.playerType == PlayerType.Ghost)
        {
            this.enabled = true;
            emfValue = playerData.emfIntensity;
            temperatureValue = playerData.temperatureAnomaly;
        }
        else
        {
            this.enabled = false;
        }
    }

    public float GetEmfValue()
    {
        return emfValue;
    }
}