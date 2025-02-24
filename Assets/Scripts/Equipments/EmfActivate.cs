using System;
using Interfaces;
using ScriptableObjectsScripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Equipments
{
    public class EmfActivate : MonoBehaviour, IActivatable
    {
        private bool emfToggle = false;
        private SphereCollider sphereCollider;
        private float detectionRange;

        // We now have a current and target EMF value
        private float currentEmfValue = 0f;
        private float targetEmfValue = 0f;
        private float emfLerpSpeed = 2f;


        private Material[] indicatorMaterialsInstance;

        [Title("Equipment Data", "Drag the EMF Reader scriptable object here")] [SerializeField]
        private EquipmentSO emfEquipmentSo;

        [Title("Indicator Materials", "Drag the materials for the EMF reader here")] [SerializeField]
        private Material[] indicatorMaterials;

        [Title("Indicator Mesh Renderers", "Drag the mesh renderers for the EMF reader here")] [SerializeField]
        private MeshRenderer[] indicatorMeshRenderers;

        
        private void Start()
        {
            detectionRange = emfEquipmentSo.range;
            
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = detectionRange;
            sphereCollider.enabled = false;

            indicatorMaterialsInstance = new Material[indicatorMaterials.Length];
            for (int i = 0; i < indicatorMaterials.Length; i++)
            {
                // Instantiate a new material so it doesn't affect other devices
                indicatorMaterialsInstance[i] = new Material(indicatorMaterials[i]);
                indicatorMeshRenderers[i].material = indicatorMaterialsInstance[i];
            }
            
            emfLerpSpeed = emfEquipmentSo.lerpSpeed;
        }

        public void Activate(ulong networkObjectId)
        {
            emfToggle = !emfToggle;

            if (emfToggle)
            {
                Debug.Log("Emf is on player ID: " + networkObjectId);
                sphereCollider.enabled = true;

                // When turned on, default target EMF value to 1 (first light on)
                targetEmfValue = 1f;

                indicatorMaterialsInstance[0].EnableKeyword("_EMISSION");
            }
            else
            {
                Debug.Log("Emf is off player ID: " + networkObjectId);
                sphereCollider.enabled = false;

                // When turned off, target EMF value to 0 (all lights off)
                targetEmfValue = 0f;

                for (int i = 0; i < indicatorMaterialsInstance.Length; i++)
                {
                    indicatorMaterialsInstance[i].DisableKeyword("_EMISSION");
                }
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            // If device is off, do nothing
            if (!emfToggle) return;

            if (!other.gameObject.CompareTag("Ghost")) return;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            // Attempt to get the Ghost component
            if (other.TryGetComponent<Ghost>(out Ghost ghost))
            {
                if (distance < detectionRange)
                {
                    // Ghost is in range, set target EMF to ghost's EMF value
                    float ghostEmfValue = ghost.GetEmfValue();
                    int emfLevelInt = Mathf.RoundToInt(ghostEmfValue);
                    emfLevelInt = Mathf.Clamp(emfLevelInt, 0, indicatorMaterialsInstance.Length);

                    targetEmfValue = emfLevelInt;
                }
                else
                {
                    // Ghost out of range, revert target EMF to 1 (just first light)
                    targetEmfValue = 1f;
                }
            }
        }

        private void Update()
        {
            // Smoothly lerp currentEmfValue towards targetEmfValue
            currentEmfValue = Mathf.Lerp(currentEmfValue, targetEmfValue, Time.deltaTime * emfLerpSpeed);

            // Round to nearest integer to decide how many lights are on
            int currentEmfLevelInt = Mathf.RoundToInt(currentEmfValue);
            currentEmfLevelInt = Mathf.Clamp(currentEmfLevelInt, 0, indicatorMaterialsInstance.Length);

            // Apply emission based on currentEmfLevelInt
            for (int i = 1; i < indicatorMaterialsInstance.Length; i++)
            {
                if (i < currentEmfLevelInt)
                {
                    indicatorMaterialsInstance[i].EnableKeyword("_EMISSION");
                }
                else
                {
                    indicatorMaterialsInstance[i].DisableKeyword("_EMISSION");
                }
            }
        }
        
        
    }
}
