#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NOTLonely_MCS
{
    [ExecuteInEditMode]
    public class MaterialSetup : MonoBehaviour
    {
        [HideInInspector] public MaterialSlot[] Data;

        private void OnEnable()
        {
            SetupData();
            if (Data != null && Data.Length != 0)
                return;

            
        }

        [ContextMenu("Setup data")]
        public void SetupData()
        {
            Dictionary<Material, MaterialSlot> data = new Dictionary<Material, MaterialSlot>();
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    Material mat = renderers[i].sharedMaterials[j];
                    MaterialSlot dataInstance = getOrCreateMaterialDataByMaterial(data, mat);

                    dataInstance.UniqueIndexies.Add(j);
                    dataInstance.UniqueRenderers.Add(renderers[i]);
                }
            }

            Data = data.Values.ToArray();
        }

        MaterialSlot getOrCreateMaterialDataByMaterial(Dictionary<Material, MaterialSlot> data, Material mat)
        {
            foreach(var kv in data)
            {
                if(kv.Key == mat)
                {
                    return kv.Value;
                }
            }

            MaterialSlot instance = new MaterialSlot(mat);
            data.Add(mat, instance);
            return instance;
        }
    }
    
    [System.Serializable]
    public class MaterialSlot
    {
        public Material Material;
        public List<int> UniqueIndexies;
        public List<Renderer> UniqueRenderers;

        public MaterialSlot(Material mat)
        {
            Material = mat;
            UniqueIndexies = new List<int>();
            UniqueRenderers = new List<Renderer>();
        }

        public void UpdateMaterial()
        {
            for (int i = 0; i < UniqueRenderers.Count; i++)
            {
                Material[] sharedMaterials = UniqueRenderers[i].sharedMaterials;
                sharedMaterials[UniqueIndexies[i]] = Material;
                UniqueRenderers[i].sharedMaterials = sharedMaterials;
            }
        }
    }
}
#endif