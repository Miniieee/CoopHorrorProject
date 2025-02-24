namespace NOTLonely_MCS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class NL_MCS_Interior : MonoBehaviour
    {
        public GameObject interiorUnbrokenContainer;
        public GameObject interiorBrokenContainer;
        public GameObject[] interiorUnbrokenTypes;
        public GameObject[] interiorBrokenTypes;
        public GameObject[] doorsWindowsArray;
        public Light[] windowLights;
        public string[] interiorTypesNames;

        [SerializeField] public int moduleTypeIndex;

        [SerializeField] public bool mtlsFoldout;

        public int rotatableCount = 0;
        [SerializeField] public MaterialPropertyBlock mtlPropertyBlock;

        [SerializeField] public bool isInteriorBroken;
        [SerializeField] public bool isDoorsWindows = true;
        [SerializeField] public bool isDoorWindowRotate;
        [SerializeField] public bool isDoorWindowMirror;
        [SerializeField] public bool isShadows2Sided;
        [SerializeField] public float UVsAngle = 0;

        [SerializeField] public bool isWindowLights;
        [SerializeField] public float lightIntensity = 2.5f;
        [SerializeField] public float lightIndirect = 1.2f;
        [SerializeField] public Color lightColor = new Color32(250, 254, 255, 255);

        private Component[] transformsArray;


        private void Start()
        {
            mtlPropertyBlock = new MaterialPropertyBlock();

            transformsArray = gameObject.GetComponentsInChildren(typeof(Transform), true);

            RotateTexture();

            if (Application.isPlaying)
            {
                transformsArray = gameObject.GetComponentsInChildren(typeof(Transform), true);

                for (int i = 0; i < transformsArray.Length; i++)
                {
                    if (transformsArray[i].gameObject != this.gameObject && !transformsArray[i].gameObject.activeSelf)
                    {
                        Destroy(transformsArray[i].gameObject);
                    }
                }

                Destroy(this);
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

        public void ModuleVariationSwitch(int index)
        {
            if (interiorUnbrokenTypes != null)
            {
                UnityEditor.Undo.RecordObject(this, "NL_MCS: Interior Type");

                for (int i = 0; i < interiorUnbrokenTypes.Length; i++)
                {
                    if (interiorUnbrokenTypes != null && interiorUnbrokenTypes.Length > 0)
                    {
                        interiorUnbrokenTypes[i].SetActive(false);
                    }

                    if (interiorBrokenTypes != null && interiorBrokenTypes.Length > 0)
                    {
                        interiorBrokenTypes[i].SetActive(false);
                    }
                }

                if (interiorUnbrokenTypes != null && interiorUnbrokenTypes.Length > 0)
                {
                    interiorUnbrokenTypes[index].SetActive(true);
                }
                if (interiorBrokenTypes != null && interiorBrokenTypes.Length > 0)
                {
                    interiorBrokenTypes[index].SetActive(true);
                }
            }           
        }

        public void ModuleIndexChange()
        {

            if (moduleTypeIndex < interiorUnbrokenTypes.Length - 1)
            {
                moduleTypeIndex++;
            }
            else
            {
                moduleTypeIndex = 0;
            }
        }

        public void InteriorBrokenVersionSwitch(bool value)
        {
            interiorBrokenContainer.SetActive(value);
            interiorUnbrokenContainer.SetActive(!value);
        }

        public void DoorWindowSwitch(bool value)
        {
            for (int i = 0; i < doorsWindowsArray.Length; i++)
            {
                if (doorsWindowsArray[i] == null)
                {
                    Debug.LogError("The slot number " + i + " in the Doors and Windows array is empty. Please fill it with an appropriate object.");
                }
                else
                {
                    doorsWindowsArray[i].SetActive(value);
                }
            }
        }

        public void DoorWindowRotate(bool value)
        {
            UnityEditor.Undo.RecordObjects(doorsWindowsArray, "Doors Rotate");
            if (value)
            {
                for (int i = 0; i < doorsWindowsArray.Length; i++)
                {
                    if (doorsWindowsArray[i].name.Contains("Door")) {
                        doorsWindowsArray[i].transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                }
            }
            else
            {
                for (int i = 0; i < doorsWindowsArray.Length; i++)
                {
                    if (doorsWindowsArray[i].name.Contains("Door"))
                    {
                        doorsWindowsArray[i].transform.localEulerAngles = Vector3.zero;
                    }
                }
            }
        }

        public void DoorWindowMirror(bool value)
        {
            UnityEditor.Undo.RecordObjects(doorsWindowsArray, "Doors Mirror");
            if (value)
            {
                for (int i = 0; i < doorsWindowsArray.Length; i++)
                {
                    if (doorsWindowsArray[i].name.Contains("Door"))
                    {
                        doorsWindowsArray[i].transform.localScale = new Vector3(-1, doorsWindowsArray[i].transform.localScale.y, doorsWindowsArray[i].transform.localScale.z);
                    }
                }
            }
            else
            {
                for (int i = 0; i < doorsWindowsArray.Length; i++)
                {
                    if (doorsWindowsArray[i].name.Contains("Door"))
                    {
                        doorsWindowsArray[i].transform.localScale = new Vector3(1, doorsWindowsArray[i].transform.localScale.y, doorsWindowsArray[i].transform.localScale.z);
                    }
                }
            }
        }

#endif
    }
}
