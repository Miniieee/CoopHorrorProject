namespace NOTLonely_MCS
{
    using UnityEngine;
    using System.Collections.Generic;
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class NL_MCS : MonoBehaviour
    {
        public GameObject exteriorUnbroken;
        public GameObject exteriorBroken;

        public GameObject[] exteriorTypes;
        public GameObject[] exteriorTypesBroken;
        public string[] exteriorTypesNames;

        public GameObject[] interiorTypes;
        public GameObject[] interiorTypesBroken;

        public GameObject interior;
        public GameObject interiorUnbroken;
        public GameObject interiorBroken;

        public GameObject rainPipe;
        public GameObject rainPipeBroken;
        public GameObject[] rainPipeVariationsUnbroken;
        public GameObject[] rainPipeVariationsBroken;

        public GameObject[] gutterContainers;
        public GameObject[] guttersBroken;
        public GameObject[] guttersUnbroken;

        public Dictionary<Material, List<Renderer>> uniqueMtls = new Dictionary<Material, List<Renderer>>();

        public Light[] windowLights;
        public int rotatableCount = 0;
        [SerializeField] public MaterialPropertyBlock mtlPropertyBlock;

        [SerializeField] public bool mtlsFoldout;

        [SerializeField] public int rainPipeCount = 0;

        [SerializeField] public bool isExteriorBroken;
        [SerializeField] public bool isInterior;
        [SerializeField] public bool isInteriorBroken;
        [SerializeField] public bool isRainPipe;
        [SerializeField] public bool isRainPipeBroken;
        [SerializeField] public bool isGutters;
        [SerializeField] public bool isGuttersBroken;
        [SerializeField] public int moduleTypeIndex;
        [SerializeField] public bool isTurn;
        [SerializeField] public bool isMirror;
        [SerializeField] public bool isShadows2Sided;
        [SerializeField] public bool isWindowLights;
        [SerializeField] public float lightIntensity = 2.5f;
        [SerializeField] public float lightIndirect = 1.2f;
        [SerializeField] public Color lightColor = new Color32(250, 255, 255, 255);
        [SerializeField] public float UVsAngle = 0; 

        [SerializeField] public bool rainPipeFoldout = true;

        private Component[] transformsArray;


        private void Start()
        {
            mtlPropertyBlock = new MaterialPropertyBlock();

            transformsArray = gameObject.GetComponentsInChildren(typeof(Transform), true);

            RotateTexture();

            if (Application.isPlaying) {

                for (int i = 0; i < transformsArray.Length; i++)
                {
                    if (transformsArray[i].gameObject != this.gameObject && !transformsArray[i].gameObject.activeSelf) {
                        Destroy(transformsArray[i].gameObject);
                    }
                }
                Destroy(this);
            }
        }

        public void CollectUniqueMtls()
        {
            MeshRenderer[] meshRndrs = gameObject.GetComponentsInChildren<MeshRenderer>(true);

            for (int i = 0; i < meshRndrs.Length; i++)
            {
                for (int j = 0; j < meshRndrs[i].sharedMaterials.Length; j++)
                {
                    List<Renderer> rendererWithMtl;

                    if (!uniqueMtls.TryGetValue(meshRndrs[i].sharedMaterials[j], out rendererWithMtl))
                    {
                        rendererWithMtl = new List<Renderer>();
                        uniqueMtls.Add(meshRndrs[i].sharedMaterials[j], rendererWithMtl);
                    }

                    rendererWithMtl.Add(meshRndrs[i]);
                }
            }
        }

        public void CalculateTextureRotation()
        {
            if (UVsAngle < 270)
            {
                UVsAngle += 90;
            }
            else
            {
                UVsAngle = 0;
            }
        }

        public void RotateTexture()
        {
            rotatableCount = 0;

            mtlPropertyBlock = new MaterialPropertyBlock();

            transformsArray = gameObject.GetComponentsInChildren(typeof(Transform), true);

            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (transformsArray[i].GetComponent<MeshRenderer>())
                {
                    if (transformsArray[i].GetComponent<MeshRenderer>().sharedMaterial.shader == Shader.Find("NOT_Lonely/NOT_Lonely_RotateUVs"))
                    {
                        rotatableCount++;

                        MeshRenderer rotatableUVsRenderer = transformsArray[i].GetComponent<MeshRenderer>();

                        mtlPropertyBlock.SetFloat("_Angle", UVsAngle);

                        rotatableUVsRenderer.SetPropertyBlock(mtlPropertyBlock);
                    }
                }
            }
        }

#if UNITY_EDITOR

        public void WindowLightsSwitch(bool value)
        {
            for (int i = 0; i < windowLights.Length; i++)
            {
                windowLights[i].gameObject.SetActive(value);
            }
        }

        public void WindowLightsPropertiesUpdate(float intensity, float indirectMultiplier, Color32 color)
        {
            for (int i = 0; i < windowLights.Length; i++)
            {
                windowLights[i].intensity = intensity;
                windowLights[i].bounceIntensity = indirectMultiplier;
                windowLights[i].color = color;
            }
        }

        public void ExteriorBrokenVersionSwitch(bool value)
        {
            exteriorBroken.SetActive(value);
            exteriorUnbroken.SetActive(!value);
        }

        public void ModuleVariationSwitch(int index)
        {
            if (exteriorTypes != null)
            {
                UnityEditor.Undo.RecordObject(this, "NL_MCS: Exterior Type");

                for (int i = 0; i < exteriorTypes.Length; i++)
                {
                    if (exteriorTypes != null && exteriorTypes.Length > 0)
                    {
                        exteriorTypes[i].SetActive(false);
                    }

                    if (exteriorTypesBroken != null && exteriorTypesBroken.Length > 0)
                    {
                        exteriorTypesBroken[i].SetActive(false);
                    }

                    if (interiorTypes != null && interiorTypes.Length > 0)
                    {
                        interiorTypes[i].SetActive(false);
                    }
                    if (interiorTypesBroken != null && interiorTypesBroken.Length > 0)
                    {
                        interiorTypesBroken[i].SetActive(false);
                    }
                }
            }

            if (exteriorTypes != null && exteriorTypes.Length > 0)
            {
                exteriorTypes[index].SetActive(true);
            }
            if (exteriorTypesBroken != null && exteriorTypesBroken.Length > 0)
            {
                exteriorTypesBroken[index].SetActive(true);
            }

            if (interiorTypes != null && interiorTypes.Length > 0)
            {
                interiorTypes[index].SetActive(true);
            }
            if (interiorTypesBroken != null && interiorTypesBroken.Length > 0)
            {
                interiorTypesBroken[index].SetActive(true);
            }
        }

        public void ModuleIndexChange()
        {

            if (moduleTypeIndex < exteriorTypes.Length - 1)
            {
                moduleTypeIndex++;
            }
            else
            {
                moduleTypeIndex = 0;
            }
        }

        public void InteriorSwitch(bool value)
        {
            interior.SetActive(value);
        }

        public void InteriorBrokenVersionSwitch(bool value)
        {
            interiorBroken.SetActive(value);
            interiorUnbroken.SetActive(!value);
        }

        public void GuttersSwitch(bool value)
        {
            for (int i = 0; i < gutterContainers.Length; i++)
            {
                gutterContainers[i].SetActive(value);
            }
        }

        public void GuttersBrokenSwitch(bool value)
        {
            for (int i = 0; i < guttersBroken.Length; i++)
            {
                guttersBroken[i].SetActive(value);
                guttersUnbroken[i].SetActive(!value);
            }
        }

        public void RainPipeSwitch(bool value)
        {
            if (isRainPipeBroken) {
                rainPipeBroken.SetActive(value);
            }
            if (!isRainPipeBroken) {
                rainPipe.SetActive(value);
            }
        }

        public void RainPipeBrokenVersionSwitch(bool value)
        {
            if (isRainPipe) {
                rainPipe.SetActive(!value);
                rainPipeBroken.SetActive(value);
            }
        }

        public void RainPipeVariationSwitch()
        {
            UnityEditor.Undo.RecordObjects(rainPipeVariationsUnbroken, "NL_MCS: Rain Pipe variation change");
            UnityEditor.Undo.RecordObjects(rainPipeVariationsBroken, "NL_MCS: Rain Pipe variation change");

            for (int i = 0; i < rainPipeVariationsUnbroken.Length; i++)
            {
                rainPipeVariationsUnbroken[i].SetActive(false);
                rainPipeVariationsBroken[i].SetActive(false);
            }


            if (rainPipeCount < rainPipeVariationsUnbroken.Length-1)
            {
                rainPipeCount++;
            }
            else
            {
                rainPipeCount = 0;
            }
            rainPipeVariationsUnbroken[rainPipeCount].SetActive(true);
            rainPipeVariationsBroken[rainPipeCount].SetActive(true);

        }

        public void RainPipeChangeSide(bool value)
        {
            if (value)
            {
                rainPipe.transform.localEulerAngles = new Vector3(0, -90, 0);
            }
            else
            {
                rainPipe.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            rainPipeBroken.transform.localEulerAngles = rainPipe.transform.localEulerAngles;
        }

        public void RainPipeMirror(bool value)
        {
            if (value) {
                rainPipe.transform.localScale = new Vector3(-1, rainPipe.transform.localScale.y, rainPipe.transform.localScale.z);
            }
            else
            {
                rainPipe.transform.localScale = new Vector3(1, rainPipe.transform.localScale.y, rainPipe.transform.localScale.z);
            }
            rainPipeBroken.transform.localScale = rainPipe.transform.localScale;
        }
#endif
    }
}
