using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class LightHandler : MonoBehaviour
{
    [FormerlySerializedAs("lightsContainer")]
    [Title("Light Container")]
    [SerializeField] private Light light;
    
    [Title("Item materials", "Lit and unlit variants")]
    [SerializeField] private Material litMaterial;
    [SerializeField] private Material unlitMaterial;
    
    private Material currentMaterial;
    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(light == null)
        {
            Debug.LogError("Lights container is not assigned.");
            return;
        }
        if(meshRenderer == null)
        {
            Debug.LogError("MeshRenderer is not found.");
            return;
        }
        
        if (light.enabled == false)
        {
            currentMaterial = new Material(unlitMaterial);
        }
        else
        {
            currentMaterial = new Material(litMaterial);
        }
        
        meshRenderer.material = currentMaterial;
    }
    
    // might need to make it more complex and set the lights individually, also might nod need to turn it off, just modify the intensity
    // need testing in a scenario
    
    public void SetLightState(bool state)
    {
        if (light == null)
        {
            Debug.LogError("Lights container is not assigned.");
            return;
        }
        
        light.enabled = state;
        
        if (state)
        {
            currentMaterial = new Material(litMaterial);
        }
        else
        {
            currentMaterial = new Material(unlitMaterial);
        }
        
        meshRenderer.material = currentMaterial;
    }

}
