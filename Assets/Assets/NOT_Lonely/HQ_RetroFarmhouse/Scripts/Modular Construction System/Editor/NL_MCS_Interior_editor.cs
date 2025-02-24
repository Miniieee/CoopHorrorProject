using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(NOTLonely_MCS.NL_MCS_Interior))]
public class NL_MCS_Interior_editor : OdinEditor {

    private NOTLonely_MCS.NL_MCS_Interior script;
    private NOTLonely_MCS.MtlEditor MtlEditor;

    SerializedProperty interiorUnbrokenTypes;
    SerializedProperty interiorBrokenTypes;
    SerializedProperty doorsWindowsArray;
    SerializedProperty isShadows2Sided;

    SerializedProperty windowLights;
    SerializedProperty lightIntensity;
    SerializedProperty lightIndirect;
    SerializedProperty lightColor;

    private Component[] transformsArray;
    private MeshRenderer[] meshRndrs;

    private bool objectsFoldout;

    public static string MCSFolder;
    private Texture2D turnIcon;
    private Texture2D mirrorIcon;
    private Texture2D brokenIcon;
    private Texture2D lightIcon;
    private float backAlpha;

    private Color iconsColorDisableDark = new Color(0.7f, 0.7f, 0.7f, 1);
    private Color iconsColorEnableDark = new Color(0, 0.656f, 0.9f, 1);

    private Color iconsColorDisableLight = new Color(0.1f, 0.1f, 0.1f, 1);
    private Color iconsColorEnableLight = new Color(0, 0.628f, 0.882f, 1);

    private void OnEnable()
    {
        script = (NOTLonely_MCS.NL_MCS_Interior)target;
        MtlEditor = new NOTLonely_MCS.MtlEditor((target as NOTLonely_MCS.NL_MCS_Interior).GetComponent<NOTLonely_MCS.MaterialSetup>());

        NOTLonely_MCS.MaterialSetup mtlSetup = (target as NOTLonely_MCS.NL_MCS_Interior).GetComponent<NOTLonely_MCS.MaterialSetup>();

        mtlSetup.hideFlags = HideFlags.HideInInspector;
        mtlSetup.hideFlags = HideFlags.NotEditable;

        //mtlSetup.hideFlags = HideFlags.None;

        interiorUnbrokenTypes = serializedObject.FindProperty("interiorUnbrokenTypes");
        interiorBrokenTypes = serializedObject.FindProperty("interiorBrokenTypes");
        doorsWindowsArray = serializedObject.FindProperty("doorsWindowsArray");

        windowLights = serializedObject.FindProperty("windowLights");

        lightIntensity = serializedObject.FindProperty("lightIntensity");
        lightIndirect = serializedObject.FindProperty("lightIndirect");
        lightColor = serializedObject.FindProperty("lightColor");

        isShadows2Sided = serializedObject.FindProperty("isShadows2Sided");

        if (script.interiorUnbrokenTypes != null)
        {
            if (script.interiorUnbrokenTypes.Length > 0)
            {
                script.interiorTypesNames = new string[script.interiorUnbrokenTypes.Length];
                for (int i = 0; i < script.interiorTypesNames.Length; i++)
                {
                    script.interiorTypesNames[i] = script.interiorUnbrokenTypes[i].name;
                }
            }
        }

        transformsArray = script.gameObject.GetComponentsInChildren(typeof(Transform), true);

        HideTransfroms();

        meshRndrs = script.gameObject.GetComponentsInChildren<MeshRenderer>(true);

        var thisScript = MonoScript.FromScriptableObject(this);
        MCSFolder = AssetDatabase.GetAssetPath(thisScript);
        MCSFolder = MCSFolder.Replace('\\', '/');
        MCSFolder = MCSFolder.Replace("Editor/" + thisScript.name, "");
        MCSFolder = MCSFolder.Replace(".cs", "");
        MCSFolder = MCSFolder + "Icons/";

        turnIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(MCSFolder + "TurnIcon.png", typeof(Texture2D));

        mirrorIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(MCSFolder + "MirrorIcon.png", typeof(Texture2D));

        brokenIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(MCSFolder + "BrokenIcon.png", typeof(Texture2D));

        lightIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(MCSFolder + "LightIcon.png", typeof(Texture2D));

        if (EditorGUIUtility.isProSkin)
        {
            backAlpha = 0.5f;
        }
        else
        {
            backAlpha = 0.3f;
        }
        script.RotateTexture();

        Undo.undoRedoPerformed += UndoRedoCallback;
    }

    /*
    [InitializeOnLoadMethod]
    static void Init()
    {
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnSceneLoaded;
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnSceneLoaded;
    }

    static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        foreach (MeshRenderer renderer in Resources.FindObjectsOfTypeAll(typeof(MeshRenderer)) as MeshRenderer[])
        {
            if (renderer.gameObject.activeInHierarchy)
            {
                if (renderer.sharedMaterial.shader == Shader.Find("NOT_Lonely/NOT_Lonely_RotateUVs"))
                {
                    if (renderer.GetComponentInParent<NOTLonely_MCS.NL_MCS_Interior>())
                    {
                        renderer.GetComponentInParent<NOTLonely_MCS.NL_MCS_Interior>().RotateTexture();
                    }
                }
            }
        }
    }
    */

    private void OnDisable()
    {
        if (script != null) {
            int counter = 0;

            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (Selection.activeGameObject != transformsArray[i].gameObject)
                {
                    counter++;
                }
                else
                {
                    counter--;
                }
            }

            if (counter == transformsArray.Length)
            {
                HideTransfroms();
            }
        }

        Undo.undoRedoPerformed -= UndoRedoCallback;

    }

    void UndoRedoCallback()
    {
        if (script != null && script.gameObject != null)
        {
            script.RotateTexture();

            if (script.interiorBrokenContainer != null && script.interiorBrokenTypes != null && script.interiorBrokenTypes.Length > 0) {
                script.InteriorBrokenVersionSwitch(script.isInteriorBroken);
            }

            if (script.doorsWindowsArray != null && script.doorsWindowsArray.Length > 0)
            {
                script.DoorWindowSwitch(script.isDoorsWindows);
                script.DoorWindowRotate(script.isDoorWindowRotate);
                script.DoorWindowMirror(script.isDoorWindowMirror);
            }
            if (script.windowLights != null && script.windowLights.Length > 0)
            {
                script.WindowLightsSwitch(script.isWindowLights);
                script.WindowLightsPropertiesUpdate(script.lightIntensity, script.lightIndirect, script.lightColor);
            }
        }
    }

    private void HideTransfroms()
    {
        for (int i = 0; i < transformsArray.Length; i++)
        {
            if (transformsArray[i].gameObject != this.script.gameObject && !transformsArray[i].gameObject.activeSelf)
            {
                transformsArray[i].hideFlags = HideFlags.HideInHierarchy;
            }
        }
    }

    private void UnhideTransforms()
    {
        for (int i = 0; i < transformsArray.Length; i++)
        {
            if (transformsArray[i].gameObject != this.script.gameObject && transformsArray[i].gameObject.activeSelf)
            {
                transformsArray[i].hideFlags = HideFlags.None;
            }
        }
    }

    void SwitchGUIColor(bool value)
    {
        if (EditorGUIUtility.isProSkin)
        {
            if (!value)
            {
                GUI.color = iconsColorDisableDark;
            }
            else
            {
                GUI.color = iconsColorEnableDark;
            }
        }
        else
        {
            if (!value)
            {
                GUI.color = iconsColorDisableLight;
            }
            else
            {
                GUI.color = iconsColorEnableLight;
            }
        }
    }

    void DrawLine()
    {
        GUI.color = new Color(1, 1, 1, 0.2f);

        Rect topLine = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
        GUI.Box(topLine, "");

        GUI.color = Color.white;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        if (script.interiorUnbrokenContainer != null && script.interiorUnbrokenTypes != null && script.interiorUnbrokenTypes.Length > 0)
        {
            GUI.color = new Color(0, 0.656f, 0.9f, backAlpha);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("-INTERIOR-", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Change Module Type", "Use this button to quickly change the module type or use the popup to the right of this button."), EditorStyles.miniButton, GUILayout.Width(130)))
            {
                script.ModuleIndexChange();
            }

            script.moduleTypeIndex = EditorGUILayout.Popup(script.moduleTypeIndex, script.interiorTypesNames);

            GUILayout.EndHorizontal();

            script.ModuleVariationSwitch(script.moduleTypeIndex);
            HideTransfroms();
            UnhideTransforms();

            if (script.interiorBrokenContainer != null)
            {
                GUILayout.BeginHorizontal();
                if (!script.isInteriorBroken)
                {
                    GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                    SwitchGUIColor(false);

                    if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the interior module"), EditorStyles.label, GUILayout.MaxWidth(72)))
                    {
                        script.InteriorBrokenVersionSwitch(true);
                        script.isInteriorBroken = true;
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                    SwitchGUIColor(true);

                    if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the interior module"), EditorStyles.label, GUILayout.MaxWidth(72)))
                    {
                        script.InteriorBrokenVersionSwitch(false);
                        script.isInteriorBroken = false;
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();

                GUI.color = Color.white;
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        if (script.doorsWindowsArray != null && script.doorsWindowsArray.Length > 0) {


            if (script.interiorUnbrokenTypes[script.moduleTypeIndex].name.Contains("Door") || script.interiorUnbrokenTypes[script.moduleTypeIndex].name.Contains("Window"))
            {

                GUI.color = new Color(0, 0.656f, 0.9f, backAlpha);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;

                if (script.isDoorsWindows)
                {
                    GUI.color = new Color(0, 0, 0, 0.1f);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.color = Color.white;
                }

                GUILayout.BeginHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                if (!script.isDoorsWindows)
                {
                    GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    SwitchGUIColor(false);

                    if (GUILayout.Button(new GUIContent("-DOORS AND WINDOWS-", "Enable the door/window model"), EditorStyles.boldLabel))
                    {
                        script.DoorWindowSwitch(true);
                        script.isDoorsWindows = true;
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    SwitchGUIColor(true);

                    if (GUILayout.Button(new GUIContent("-DOORS AND WINDOWS-", "Enable the door/window model"), EditorStyles.boldLabel))
                    {
                        script.isDoorsWindows = false;
                        script.DoorWindowSwitch(false);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUI.color = Color.white;

                GUILayout.EndHorizontal();

                if (script.isDoorsWindows)
                {
                    GUILayout.Space(2);

                        GUILayout.BeginHorizontal();

                        if (!script.isDoorWindowRotate)
                        {
                            GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                            SwitchGUIColor(false);

                            if (GUILayout.Button(new GUIContent("Turn", turnIcon, "Rotate the door/window on 180 degrees."), EditorStyles.label, GUILayout.MaxWidth(72)))
                            {
                                script.DoorWindowRotate(true);
                                script.isDoorWindowRotate = true;
                            }
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                            SwitchGUIColor(true);

                            if (GUILayout.Button(new GUIContent("Turn", turnIcon, "Rotate the door/window on 180 degrees."), EditorStyles.label, GUILayout.MaxWidth(72)))
                            {
                                script.isDoorWindowRotate = false;
                                script.DoorWindowRotate(false);
                            }
                            GUILayout.EndVertical();
                        }

                        GUI.color = Color.white;

                        if (!script.isDoorWindowMirror)
                        {
                            GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                            SwitchGUIColor(false);

                            if (GUILayout.Button(new GUIContent("Mirror", mirrorIcon, "Mirror the door window along its local X axis to change the knob side."), EditorStyles.label, GUILayout.MaxWidth(72)))
                            {
                                script.DoorWindowMirror(true);
                                script.isDoorWindowMirror = true;
                            }
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                            SwitchGUIColor(true);

                            if (GUILayout.Button(new GUIContent("Mirror", mirrorIcon, "Mirror the door window along its local X axis to change the knob side."), EditorStyles.label, GUILayout.MaxWidth(72)))
                            {
                                script.isDoorWindowMirror = false;
                                script.DoorWindowMirror(false);
                            }
                            GUILayout.EndVertical();
                        }


                        GUI.color = Color.white;

                        GUILayout.EndHorizontal();

                    ///////////////////////////// Lights
                    if (script.interiorUnbrokenTypes[script.moduleTypeIndex].name.Contains("Window") || script.interiorUnbrokenTypes[script.moduleTypeIndex].name.Contains("Door Wide"))
                    {
                        if (script.windowLights != null && script.windowLights.Length > 0)
                        {
                            if (script.isWindowLights)
                            {
                                GUI.color = new Color(0, 0, 0, 0.1f);
                                GUILayout.BeginVertical(EditorStyles.helpBox);
                                GUI.color = Color.white;
                            }

                            GUILayout.BeginHorizontal();
                            if (!script.isWindowLights)
                            {
                                GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                                SwitchGUIColor(false);

                                if (GUILayout.Button(new GUIContent("Lights", lightIcon, "Enable the light source (Area Light) at the interior side of the window. Significantly improves interior lighting quality and realism when using baked lighting (have nor visual effect, nor performance impact when real time lighting in use). ATTENTION: Area Lights have a big impact on the light baking time."), EditorStyles.label, GUILayout.MaxWidth(72)))
                                {
                                    script.WindowLightsSwitch(true);
                                    script.isWindowLights = true;
                                }
                                GUILayout.EndVertical();
                            }
                            else
                            {
                                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(68));
                                SwitchGUIColor(true);

                                if (GUILayout.Button(new GUIContent("Lights", lightIcon, "Enable the light source (Area Light) at the interior side of the window. Significantly improves interior lighting quality and realism when using baked lighting (have nor visual effect, nor performance impact when real time lighting in use). ATTENTION: Area Lights have a big impact on the light baking time."), EditorStyles.label, GUILayout.MaxWidth(68)))
                                {
                                    script.WindowLightsSwitch(false);
                                    script.isWindowLights = false;
                                }
                                GUILayout.EndVertical();
                            }

                            GUILayout.EndHorizontal();

                            GUI.color = Color.white;
                        }

                        GUILayout.Space(4);

                        //////////////////////////////Lights properties

                        if (script.windowLights != null && script.windowLights.Length > 0 && script.isWindowLights)
                        {
                            EditorGUILayout.PropertyField(lightIntensity, new GUIContent("Intensity", "Controls the brightness of the light."));

                            EditorGUILayout.PropertyField(lightIndirect, new GUIContent("Indirect Multiplier", "Controls the intensity of indirect light being contributed to the scene."));

                            EditorGUILayout.PropertyField(lightColor, new GUIContent("Color", "Controls the color being emitted from the light."));

                            GUI.color = Color.white;
                        }

                        /////////////////////////////////

                        if (script.windowLights != null && script.windowLights.Length > 0 && script.isWindowLights)
                        {
                            GUILayout.EndVertical();
                        }
                    }

                }

                if (script.isDoorsWindows)
                {
                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }
        }

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("-MESH SETTINGS-", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(isShadows2Sided, new GUIContent("2-Sided Shadow", "Two sided shadow casting mode. Enable if you are using realtime lighting to avoid the light leaking artifacts in some of performance cost."));

            if (script.rotatableCount > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Rotate Texure", EditorStyles.miniButton, GUILayout.Width(100)))
                {
                    script.CalculateTextureRotation();
                    script.RotateTexture();
                }

                GUILayout.Label("current degrees = " + script.UVsAngle);

                GUILayout.EndHorizontal();
            }

        //////Mtl array section
        GUILayout.BeginVertical(EditorStyles.helpBox);
        script.mtlsFoldout = EditorGUILayout.Foldout(script.mtlsFoldout, "Materials Array");

        if (script.mtlsFoldout)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("You can re-assign materials in this array to batch change them on the all variations of this module", ""), EditorStyles.helpBox);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            MtlEditor.OnGUI();
        }

        GUILayout.EndVertical();
        /////////////


        GUILayout.EndVertical();

        GUI.color = new Color(1, 1, 1, 0.3f);

        GUILayout.BeginVertical(EditorStyles.helpBox);
        objectsFoldout = EditorGUILayout.Foldout(objectsFoldout, "Input Objects (don't modify)");

        GUI.color = Color.white;

        if (objectsFoldout)
        {

            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (transformsArray[i].gameObject != this.script.gameObject)
                {
                    transformsArray[i].hideFlags = HideFlags.None;
                }
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Unbroken Interior container:", "Select the unbroken objects parent gameObject."));
            script.interiorUnbrokenContainer = (GameObject)EditorGUILayout.ObjectField(script.interiorUnbrokenContainer, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Broken Interior container:", "Select the unbroken objects parent gameObject."));
            script.interiorBrokenContainer = (GameObject)EditorGUILayout.ObjectField(script.interiorBrokenContainer, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(interiorUnbrokenTypes, new GUIContent("Interior Unbroken Variations", "Interior unbroken variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(interiorBrokenTypes, new GUIContent("Interior Broken Variations", "Interior broken variations array"), true);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(doorsWindowsArray, new GUIContent("Doors and Windows", "Doors and windows array"), true);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(windowLights, new GUIContent("Window Ligths", "Window Lights array"), true);

            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.FindProperty("moduleTypeIndex").intValue = script.moduleTypeIndex;

            serializedObject.ApplyModifiedProperties();

            if (script.doorsWindowsArray != null && script.doorsWindowsArray.Length > 0)
            {
                script.DoorWindowSwitch(script.isDoorsWindows);
                script.DoorWindowRotate(script.isDoorWindowRotate);
                script.DoorWindowMirror(script.isDoorWindowMirror);
            }

            if (script.windowLights != null && script.windowLights.Length > 0)
            {
                script.WindowLightsPropertiesUpdate(script.lightIntensity, script.lightIndirect, script.lightColor);
            }

            if (meshRndrs != null)
            {

                if (meshRndrs.Length > 0)
                {
                    for (int i = 0; i < meshRndrs.Length; i++)
                    {
                        if (!meshRndrs[i].GetComponent<NL_MCS_IgnoreShadowCastingSwitch>())
                        {
                            if (script.isShadows2Sided)
                            {
                                meshRndrs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                            }
                            else
                            {
                                meshRndrs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                            }
                        }
                    }
                }
            }
        }

    }
}
