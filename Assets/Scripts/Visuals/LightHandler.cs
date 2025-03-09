using Sirenix.OdinInspector;
using UnityEngine;

public class LightHandler : MonoBehaviour
{
    [Title("Light Container")]
    [SerializeField] private GameObject lightsContainer;
    
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
        if(lightsContainer == null)
        {
            Debug.LogError("Lights container is not assigned.");
            return;
        }
        if(meshRenderer == null)
        {
            Debug.LogError("MeshRenderer is not found.");
            return;
        }
        
        Debug.LogError("Light Container is: " + lightsContainer.activeSelf + "Item is: " + gameObject.name);
        
        if (lightsContainer.activeSelf == false)
        {
            currentMaterial = new Material(unlitMaterial);
        }
        else
        {
            currentMaterial = new Material(litMaterial);
        }
        
        meshRenderer.material = currentMaterial;
    }

}
