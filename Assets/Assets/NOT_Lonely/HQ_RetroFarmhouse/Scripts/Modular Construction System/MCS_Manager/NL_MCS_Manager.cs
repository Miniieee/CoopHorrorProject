# if UNITY_EDITOR
namespace NOTLonely_MCS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [ExecuteInEditMode]
    public class NL_MCS_Manager : MonoBehaviour
    {
        private Component[] transformsArray;
        private List<Component> mcsTransforms = new List<Component>();
        public int unusedObjectsCount;
        private GameObject tempParent;
        private GameObject mcsParent;
        private GameObject mcsInteiorParent;
        private GameObject mcsExteriorParent;
        private GameObject newParent;
        private List<Transform> interiorModules = new List<Transform>();
        private int count = 0;

        private void Start()
        {

        }


        public void RemoveEmptyObject()
        {
            Transform[] allTransfroms = mcsExteriorParent.GetComponentsInChildren<Transform>();

            for (int i = 0; i < allTransfroms.Length; i++)
            {
                if (allTransfroms[i] == null)
                    continue;

                if (allTransfroms[i].transform != mcsExteriorParent.transform)
                {
                    Component[] comps = allTransfroms[i].GetComponents<Component>();

                    if (comps.Length == 1 && allTransfroms[i].transform.childCount == 0)
                    {
                        if (allTransfroms[i] != null)
                        {
                            DestroyImmediate(allTransfroms[i].gameObject);
                            count++;
                        }
                    }

                }
                
            }
        }

        public void RemoveUnusedModules()
        {
            tempParent = new GameObject("TempParent");
            mcsParent = new GameObject("Modules");
            mcsInteiorParent = new GameObject("Interior");
            mcsExteriorParent = new GameObject("Exterior");
            newParent = new GameObject("Building");

            mcsInteiorParent.transform.SetParent(mcsParent.transform);
            mcsExteriorParent.transform.SetParent(mcsParent.transform);

            newParent.transform.position = transform.position;
            newParent.transform.rotation = transform.rotation;
            newParent.transform.localScale = transform.localScale;

            mcsParent.transform.position = transform.position;
            mcsParent.transform.rotation = transform.rotation;
            mcsParent.transform.localScale = transform.localScale;

            transformsArray = gameObject.GetComponentsInChildren(typeof(Transform), true);


            //fill the list of MCS objects
            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (transformsArray[i].GetComponent<NL_MCS>() != null)
                {
                    mcsTransforms.Add(transformsArray[i].transform);
                }
                if (transformsArray[i].GetComponent<NL_MCS_Interior>() != null)
                {
                    mcsTransforms.Add(transformsArray[i].transform);
                }
            }

            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (transformsArray[i] != null)
                {
                    if (transformsArray[i].GetComponent<NL_MCS>() != null)
                    {
                        PrefabUtility.UnpackPrefabInstance(transformsArray[i].gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                        transformsArray[i].transform.SetParent(mcsExteriorParent.transform);

                        if (transformsArray[i].GetComponent<NL_MCS>().interior != null && transformsArray[i].GetComponent<NL_MCS>().interior.activeInHierarchy)
                        {
                            transformsArray[i].GetComponent<NL_MCS>().interior.transform.SetParent(mcsInteiorParent.transform);
                        }
        
                        DestroyImmediate(transformsArray[i].GetComponent<NL_MCS>());

                    }
                    else if (transformsArray[i].GetComponent<NL_MCS_Interior>() != null)
                    {

                        PrefabUtility.UnpackPrefabInstance(transformsArray[i].gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                        transformsArray[i].transform.SetParent(mcsExteriorParent.transform);

                        if (transformsArray[i].GetComponent<NL_MCS_Interior>().interiorUnbrokenContainer.activeInHierarchy)
                        {
                            transformsArray[i].GetComponent<NL_MCS_Interior>().interiorUnbrokenContainer.transform.SetParent(mcsInteiorParent.transform);
                        }

                        if (transformsArray[i].GetComponent<NL_MCS_Interior>().interiorBrokenContainer != null && transformsArray[i].GetComponent<NL_MCS_Interior>().interiorBrokenContainer.activeInHierarchy)
                        {
                            transformsArray[i].GetComponent<NL_MCS_Interior>().interiorBrokenContainer.transform.SetParent(mcsInteiorParent.transform);
                        }
   
                        DestroyImmediate(transformsArray[i].GetComponent<NL_MCS_Interior>());
                    }

                    if (transformsArray[i].GetComponent<MaterialSetup>() != null)
                    {
                        DestroyImmediate(transformsArray[i].GetComponent<MaterialSetup>());
                    }

                }

                /*
                if (transformsArray[i] != this.transform)
                {
                    if (!transformsArray[i].gameObject.activeInHierarchy)
                    {
                        if (transformsArray[i] != null)
                        {
                            transformsArray[i].hideFlags = HideFlags.None;
                            transformsArray[i].transform.SetParent(this.transform);
                            transformsArray[i].gameObject.SetActive(false);
                        }
                    }
                }
                */
            }

            for (int i = 0; i < mcsTransforms.Count; i++)
            {
                Component[] mcsObjs = mcsTransforms[i].gameObject.GetComponentsInChildren(typeof(Transform), true);

                for (int j = 0; j < mcsObjs.Length; j++)
                {
                    if (mcsObjs[j].transform != this.transform)
                    {
                        if (!mcsObjs[j].gameObject.activeInHierarchy)
                        {
                            if (mcsObjs[j] != null)
                            {
                                mcsObjs[j].hideFlags = HideFlags.None;
                                mcsObjs[j].transform.SetParent(tempParent.transform);
                                //mcsObjs[j].transform.SetParent(this.transform);
                                //mcsObjs[j].gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }

            /*
            transformsArray = gameObject.GetComponentsInChildren(typeof(Transform), true);
            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (transformsArray[i] != this.transform)
                {
                    if (!transformsArray[i].gameObject.activeSelf)
                    {
                        DestroyImmediate(transformsArray[i].gameObject);
                        unusedObjectsCount++;
                    }
                }

            }
            */

            

            Component[] interiorObjs = mcsInteiorParent.GetComponentsInChildren(typeof(Transform), true);
            for (int i = 0; i < interiorObjs.Length; i++)
            {
                if (interiorObjs[i] != null)
                {
                    if (!interiorObjs[i].gameObject.activeSelf)
                    {
                        count++;
                        DestroyImmediate(interiorObjs[i].gameObject);
                    }
                }
            }

            unusedObjectsCount = tempParent.GetComponentsInChildren(typeof(Transform), true).Length - 1 + count;

            DestroyImmediate(tempParent);


            RemoveEmptyObject();

            transform.SetParent(newParent.transform);
            mcsParent.transform.SetParent(newParent.transform);
            newParent.name = gameObject.name;
            gameObject.name = "Props";

            if (unusedObjectsCount == 0)
            {
                print("Nothing to remove!");
            }
            else
            {
                print("Unused objects removed successfully! " + unusedObjectsCount + " objects removed.");
            }
        }
    }
}
#endif
