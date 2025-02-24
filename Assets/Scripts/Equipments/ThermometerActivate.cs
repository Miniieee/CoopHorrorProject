using System;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace Equipments
{
    public class ThermometerActivate : MonoBehaviour, IActivatable
    {
        private const string TemperatureString = "Temperature";

        [Title("Indicator Mesh Renderer", "Drag the mesh renderer for the Temperature sensor here")] [SerializeField]
        private MeshRenderer tempSensorScreenMeshRenderer;

        [Title("Indicator Mesh Renderer", "Drag the mesh renderer for the Temperature sensor here")] [SerializeField]
        private Material tempSensorScreenMaterial;

        [Title("Indicator Text", "Drag the TextMeshProUGUI for the Temperature sensor here")] [SerializeField]
        private TextMeshProUGUI temperatureText;

        [FormerlySerializedAs("cDegreeText")] [SerializeField]
        private TextMeshProUGUI celsiusDegreeText;

        [SerializeField] private float updateInterval = 1f;
        [SerializeField] private float initialInterval = 0.1f;

        private Material tempSensorScreenMaterialInstance;
        private bool temperatureToggle = false;
        private SphereCollider thermometerCollider;
        private float temperature;
        private int temperatureLayer;
        
        private float updateTimer;
        
        private AreaTemperature currentAreaTemp;

        private void Awake()
        {
            thermometerCollider = GetComponent<SphereCollider>();
            thermometerCollider.isTrigger = true;
            
            temperatureLayer = LayerMask.NameToLayer(TemperatureString);
            
            updateTimer = updateInterval;
        }

        private void Start()
        {
            thermometerCollider.enabled = false;

            tempSensorScreenMaterialInstance = new Material(tempSensorScreenMaterial);
            tempSensorScreenMeshRenderer.material = tempSensorScreenMaterialInstance;
        }

        public void Activate(ulong networkObjectId)
        {
            temperatureToggle = !temperatureToggle;

            if (temperatureToggle)
            {
                Debug.Log("Thermometer is on player ID: " + networkObjectId);
                thermometerCollider.enabled = true;
                tempSensorScreenMaterialInstance.EnableKeyword("_EMISSION");

                updateTimer = initialInterval;
            }
            else
            {
                Debug.Log("Thermometer is off player ID: " + networkObjectId);
                thermometerCollider.enabled = false;
                temperatureText.text = string.Empty;
                celsiusDegreeText.text = string.Empty;
                currentAreaTemp = null;
                tempSensorScreenMaterialInstance.DisableKeyword("_EMISSION");
            }

            // If off, we can also reset the timer
            if (!temperatureToggle)
            {
                updateTimer = initialInterval;
            }
        }

        private void Update()
        {
            if (temperatureToggle && currentAreaTemp != null)
            {
                updateTimer -= Time.deltaTime;
                if (updateTimer <= 0f)
                {
                    // Reset the timer
                    updateTimer = updateInterval;

                    // Get base temperature from area
                    float baseTemp = currentAreaTemp.GetTemperature();

                    // Add random variation between 0 and 2 degrees
                    float variation = UnityEngine.Random.Range(0f, 2f);
                    temperature = baseTemp + variation;

                    // Update UI
                    temperatureText.text = $"{temperature:0.0}";
                    celsiusDegreeText.text = "°C";
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Check if the collider is on the Temperature layer
            if (other.gameObject.layer == temperatureLayer && temperatureToggle)
            {
                // Attempt to get the AreaTemperature component
                if (other.TryGetComponent<AreaTemperature>(out AreaTemperature areaTemp))
                {
                    // Store this as the current temperature area
                    currentAreaTemp = areaTemp;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // If we leave the temperature area, clear the reference
            if (other.gameObject.layer == temperatureLayer)
            {
                currentAreaTemp = null;
            }
        }
    }
}
