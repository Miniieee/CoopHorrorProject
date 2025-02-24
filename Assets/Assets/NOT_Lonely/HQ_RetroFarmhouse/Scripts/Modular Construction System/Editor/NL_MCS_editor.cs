using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NOTLonely_MCS;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(NL_MCS))]
[RequireComponent (typeof (MaterialSetup))]
public class NL_MCS_editor : OdinEditor
{

    private NL_MCS script;

    private MtlEditor MtlEditor;

    SerializedProperty rainPipeVariationsUnbroken;
    SerializedProperty rainPipeVariationsBroken;
    SerializedProperty exteriorVariationsBroken;
    SerializedProperty exteriorVariationsUnbroken;
    SerializedProperty interiorVariationsBroken;
    SerializedProperty interiorVariationsUnbroken;

    SerializedProperty gutterContainers;
    SerializedProperty guttersBroken;
    SerializedProperty guttersUnbroken;

    SerializedProperty isShadows2Sided;

    SerializedProperty windowLights;
    SerializedProperty lightIntensity;
    SerializedProperty lightIndirect;
    SerializedProperty lightColor;

    private Component[] transformsArray;
    private MeshRenderer[] meshRndrs;
    private Dictionary<Material, Material> oldNewMtls = new Dictionary<Material, Material>();


    [SerializeField] private bool objectsFoldout;

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

    

    protected virtual void OnEnable()
    {
        script = (NL_MCS)target;

        MtlEditor = new MtlEditor((target as NL_MCS).GetComponent<MaterialSetup>());

        MaterialSetup mtlSetup = (target as NL_MCS).GetComponent<MaterialSetup>();

        mtlSetup.hideFlags = HideFlags.HideInInspector;
        mtlSetup.hideFlags = HideFlags.NotEditable;

        //mtlSetup.hideFlags = HideFlags.None;

        isShadows2Sided = serializedObject.FindProperty("isShadows2Sided");

        gutterContainers = serializedObject.FindProperty("gutterContainers");

        guttersBroken = serializedObject.FindProperty("guttersBroken");

        guttersUnbroken = serializedObject.FindProperty("guttersUnbroken");

        exteriorVariationsUnbroken = serializedObject.FindProperty("exteriorTypes");

        exteriorVariationsBroken = serializedObject.FindProperty("exteriorTypesBroken");

        interiorVariationsUnbroken = serializedObject.FindProperty("interiorTypes");

        interiorVariationsBroken = serializedObject.FindProperty("interiorTypesBroken");

        rainPipeVariationsBroken = serializedObject.FindProperty("rainPipeVariationsBroken");

        rainPipeVariationsUnbroken = serializedObject.FindProperty("rainPipeVariationsUnbroken");

        windowLights = serializedObject.FindProperty("windowLights");

        lightIntensity = serializedObject.FindProperty("lightIntensity");

        lightIndirect = serializedObject.FindProperty("lightIndirect");

        lightColor = serializedObject.FindProperty("lightColor");

        if (script.exteriorTypes != null)
        {
            if (script.exteriorTypes.Length > 0)
            {
                script.exteriorTypesNames = new string[script.exteriorTypes.Length];
                for (int i = 0; i < script.exteriorTypesNames.Length; i++)
                {
                    script.exteriorTypesNames[i] = script.exteriorTypes[i].name;
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

        Undo.undoRedoPerformed += UndoRedoCallback;

        script.RotateTexture();
        script.CollectUniqueMtls();
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
                if (renderer.sharedMaterial.shader == Shader.Find("NOT_Lonely/NOT_Lonely_RotateUVs")) {
                    
                    if (renderer.GetComponentInParent<NOTLonely_MCS.NL_MCS>())
                    {
                        renderer.GetComponentInParent<NOTLonely_MCS.NL_MCS>().RotateTexture();
                    }
                }
            }
        }
    }
    */

    private void OnDisable()
    {
        if (script != null)
        {
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
        GUI.color = new Color(0, 0, 0, 0.1f);

        Rect topLine = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
        GUI.Box(topLine, "", EditorStyles.miniButton);

        GUI.color = Color.white;
    }

    void UndoRedoCallback()
    {
        if (script != null && script.gameObject != null)
        {
            if (script.exteriorBroken != null)
                script.ExteriorBrokenVersionSwitch(script.isExteriorBroken);

            if (script.interiorBroken != null)
                script.InteriorBrokenVersionSwitch(script.isInteriorBroken);

            if (script.rainPipeBroken != null)
            {
                script.RainPipeBrokenVersionSwitch(script.isRainPipeBroken);
            }

            if (script.rainPipe != null)
            {
                script.RainPipeChangeSide(script.isTurn);
                script.RainPipeMirror(script.isMirror);
                script.RainPipeSwitch(script.isRainPipe);
            }

            if (script.gutterContainers != null)
                script.GuttersSwitch(script.isGutters);

            if (script.guttersBroken != null)
                script.GuttersBrokenSwitch(script.isGuttersBroken);

            if (script.interior != null)
                script.InteriorSwitch(script.isInterior);

            if (script.windowLights != null)
            {
                script.WindowLightsSwitch(script.isWindowLights);
                script.WindowLightsPropertiesUpdate(script.lightIntensity, script.lightIndirect, script.lightColor);
            }

            script.RotateTexture();

            UpdateMtls();
        }
    }

    private void UpdateMtls()
    {
        foreach (var kv in oldNewMtls)
        {
            List<Renderer> rndrs = script.uniqueMtls[kv.Key];
            script.uniqueMtls.Remove(kv.Key);

            List<Renderer> alreadyExistRndrs;

            if (script.uniqueMtls.TryGetValue(kv.Value, out alreadyExistRndrs))
            {
                alreadyExistRndrs.AddRange(rndrs);
            }
            else
            {
                script.uniqueMtls.Add(kv.Value, rndrs);
            }
        }

        //Undo.RecordObjects(meshRndrs, "Change Material");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        //exterior

        if (script.exteriorUnbroken != null && script.exteriorTypes != null && script.exteriorTypes.Length > 0)
        {
            GUI.color = new Color(0, 0.656f, 0.9f, backAlpha);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("-EXTERIOR-", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Change Module Type", "Use this button to quickly change the module type or use the popup to the right of this button."), EditorStyles.miniButton, GUILayout.Width(130)))
            {
                script.ModuleIndexChange();
            }

            script.moduleTypeIndex = EditorGUILayout.Popup(script.moduleTypeIndex, script.exteriorTypesNames);

            GUILayout.EndHorizontal();

            if (script.exteriorBroken != null)
            {
                GUILayout.BeginHorizontal();
                if (!script.isExteriorBroken)
                {
                    GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                    SwitchGUIColor(false);

                    if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the exterior module"), EditorStyles.label, GUILayout.MaxWidth(72)))
                    {
                        script.ExteriorBrokenVersionSwitch(true);
                        script.isExteriorBroken = true;
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                    SwitchGUIColor(true);

                    if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the exterior module"), EditorStyles.label, GUILayout.MaxWidth(72)))
                    {
                        script.isExteriorBroken = false;
                        script.ExteriorBrokenVersionSwitch(false);
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();

                GUI.color = Color.white;
            }

            if (script.exteriorUnbroken != null)
            {
                script.ModuleVariationSwitch(script.moduleTypeIndex);
                HideTransfroms();
                UnhideTransforms();
            }
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        //interior

        if (script.interior != null)
        {
            GUI.color = new Color(0, 0.656f, 0.9f, backAlpha);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            if (script.isInterior)
            {
                GUI.color = new Color(0, 0, 0, 0.1f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;
            }

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (!script.isInterior)
            {
                GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                SwitchGUIColor(false);

                if (GUILayout.Button(new GUIContent("-INTERIOR-", "Enable the interior module"), EditorStyles.boldLabel))
                {
                    script.InteriorSwitch(true);
                    script.isInterior = true;
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                SwitchGUIColor(true);

                if (GUILayout.Button(new GUIContent("-INTERIOR-", "Enable the interior module"), EditorStyles.boldLabel))
                {
                    script.isInterior = false;
                    script.InteriorSwitch(false);
                }
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUI.color = Color.white;

            GUILayout.EndHorizontal();


            if (script.isInterior)
            {
                if (script.interiorBroken != null && script.interiorUnbroken != null && script.isInterior)
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
                            script.isInteriorBroken = false;
                            script.InteriorBrokenVersionSwitch(false);
                        }
                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();

                    GUI.color = Color.white;
                }

                ///////////////////////////// Lights
                if (script.exteriorTypes.Length > 0) {
                    if (script.exteriorTypes[script.moduleTypeIndex].name.Contains("Window") || script.exteriorTypes[script.moduleTypeIndex].name.Contains("Door Wide"))
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
            }

            if (script.isInterior)
            {
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        if (script.rainPipe != null)
        {
            GUI.color = new Color(0, 0.656f, 0.9f, backAlpha);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            if (script.isRainPipe)
            {
                GUI.color = new Color(0, 0, 0, 0.1f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;
            }

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (!script.isRainPipe)
            {
                GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                SwitchGUIColor(false);

                if (GUILayout.Button(new GUIContent("-RAIN PIPE-", "Enable the rain pipe module"), EditorStyles.boldLabel))
                {
                    script.RainPipeSwitch(true);
                    script.isRainPipe = true;
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                SwitchGUIColor(true);

                if (GUILayout.Button(new GUIContent("-RAIN PIPE-", "Enable the rain pipe module"), EditorStyles.boldLabel))
                {
                    script.isRainPipe = false;
                    script.RainPipeSwitch(false);
                }
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (script.isRainPipe)
            {
                if (script.isRainPipe && script.rainPipeVariationsUnbroken.Length > 1)
                {
                    if (GUILayout.Button("Change variation", EditorStyles.miniButton, GUILayout.Width(110)))
                    {
                        script.RainPipeVariationSwitch();
                    }
                }

                GUILayout.Space(2);

                if (script.rainPipeBroken && script.isRainPipe)
                {

                    GUILayout.BeginHorizontal();
                    if (!script.isRainPipeBroken)
                    {
                        GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                        SwitchGUIColor(false);

                        if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the rain pipe module"), EditorStyles.label, GUILayout.MaxWidth(72)))
                        {
                            script.RainPipeBrokenVersionSwitch(true);
                            script.isRainPipeBroken = true;
                        }
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                        SwitchGUIColor(true);

                        if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the rain pipe module"), EditorStyles.label, GUILayout.MaxWidth(72)))
                        {
                            script.RainPipeBrokenVersionSwitch(false);
                            script.isRainPipeBroken = false;
                        }
                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();

                    GUI.color = Color.white;
                }

                if (script.isRainPipe)
                {
                    GUILayout.BeginHorizontal();

                    if (!script.isTurn)
                    {
                        GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                        SwitchGUIColor(false);

                        if (GUILayout.Button(new GUIContent("Turn", turnIcon, "Rotate the object on 90 degrees."), EditorStyles.label, GUILayout.MaxWidth(72)))
                        {
                            script.RainPipeChangeSide(true);
                            script.isTurn = true;
                        }
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                        SwitchGUIColor(true);

                        if (GUILayout.Button(new GUIContent("Turn", turnIcon, "Rotate the object on 90 degrees."), EditorStyles.label, GUILayout.MaxWidth(72)))
                        {
                            script.isTurn = false;
                            script.RainPipeChangeSide(false);
                        }
                        GUILayout.EndVertical();
                    }

                    GUI.color = Color.white;

                    if (!script.isMirror)
                    {
                        GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                        SwitchGUIColor(false);

                        if (GUILayout.Button(new GUIContent("Mirror", mirrorIcon, "Mirror the object along the X axis."), EditorStyles.label, GUILayout.MaxWidth(72)))
                        {
                            script.RainPipeMirror(true);
                            script.isMirror = true;
                        }
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                        SwitchGUIColor(true);

                        if (GUILayout.Button(new GUIContent("Mirror", mirrorIcon, "Mirror the object along the X axis."), EditorStyles.label, GUILayout.MaxWidth(72)))
                        {
                            script.isMirror = false;
                            script.RainPipeMirror(false);
                        }
                        GUILayout.EndVertical();
                    }


                    GUI.color = Color.white;

                    GUILayout.EndHorizontal();
                }
            }

            if (script.rainPipe != null && script.isRainPipe)
            {
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        if (script.gutterContainers != null)
        {
            if (script.gutterContainers.Length > 0)
            {
                GUI.color = new Color(0, 0.656f, 0.9f, backAlpha);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;

                if (script.isGutters)
                {
                    GUI.color = new Color(0, 0, 0, 0.1f);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.color = Color.white;
                }

                GUILayout.BeginHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                if (!script.isGutters)
                {
                    GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    SwitchGUIColor(false);

                    if (GUILayout.Button(new GUIContent("-GUTTERS-", "Enable the gutter module"), EditorStyles.boldLabel))
                    {
                        script.GuttersSwitch(true);
                        script.isGutters = true;
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    SwitchGUIColor(true);

                    if (GUILayout.Button(new GUIContent("-GUTTERS-", "Enable the gutter module"), EditorStyles.boldLabel))
                    {
                        script.isGutters = false;
                        script.GuttersSwitch(false);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUI.color = Color.white;

                GUILayout.EndHorizontal();

                if (script.isGutters)
                {
                    if (script.guttersBroken != null && script.isGutters)
                    {
                        if (script.guttersBroken.Length > 0)
                        {

                            GUILayout.BeginHorizontal();
                            if (!script.isGuttersBroken)
                            {
                                GUI.color = new Color(0.9f, 0.9f, 0.9f, 0.7f);
                                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                                SwitchGUIColor(false);

                                if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the gutter modules"), EditorStyles.label, GUILayout.MaxWidth(72)))
                                {
                                    script.GuttersBrokenSwitch(true);
                                    script.isGuttersBroken = true;
                                }
                                GUILayout.EndVertical();
                            }
                            else
                            {
                                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(72));
                                SwitchGUIColor(true);

                                if (GUILayout.Button(new GUIContent("Broken", brokenIcon, "Enables a broken version of the gutter modules"), EditorStyles.label, GUILayout.MaxWidth(72)))
                                {
                                    script.isGuttersBroken = false;
                                    script.GuttersBrokenSwitch(false);
                                }
                                GUILayout.EndVertical();
                            }

                            GUILayout.EndHorizontal();

                            GUI.color = Color.white;
                        }
                    }
                }

                if (script.gutterContainers != null && script.gutterContainers.Length > 0 && script.isGutters)
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

        GUILayout.EndHorizontal();



        //Input objects section
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

            GUILayout.Label(new GUIContent("Broken exterior container:", "Select the broken objects parent gameObject."));
            script.exteriorBroken = (GameObject)EditorGUILayout.ObjectField(script.exteriorBroken, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Unbroken exterior container:", "Select the unbroken objects parent gameObject."));
            script.exteriorUnbroken = (GameObject)EditorGUILayout.ObjectField(script.exteriorUnbroken, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Interior objects container:", "Select the unbroken objects parent gameObject."));
            script.interior = (GameObject)EditorGUILayout.ObjectField(script.interior, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Broken interior container:", "Select the broken objects parent gameObject."));
            script.interiorBroken = (GameObject)EditorGUILayout.ObjectField(script.interiorBroken, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Unbroken interior container:", "Select the unbroken objects parent gameObject."));
            script.interiorUnbroken = (GameObject)EditorGUILayout.ObjectField(script.interiorUnbroken, typeof(GameObject), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(interiorVariationsBroken, new GUIContent("Interior broken Variations", "Interior broken variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(interiorVariationsUnbroken, new GUIContent("Interior Unbroken Variations", "Interior Unbroken variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(windowLights, new GUIContent("Window Ligths", "Window Lights array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(exteriorVariationsBroken, new GUIContent("Exterior broken Variations", "Exterior broken variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(exteriorVariationsUnbroken, new GUIContent("Exterior Unbroken Variations", "Exterior Unbroken variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Rain pipe container:", "Select the rain pipe objects parent gameObject."));
            script.rainPipe = (GameObject)EditorGUILayout.ObjectField(script.rainPipe, typeof(GameObject), true);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Broken Rain pipe container:", "Select the rain pipe objects parent gameObject."));
            script.rainPipeBroken = (GameObject)EditorGUILayout.ObjectField(script.rainPipeBroken, typeof(GameObject), true);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(rainPipeVariationsBroken, new GUIContent("Broken Rain Pipe Variations", "Rain pipe variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(rainPipeVariationsUnbroken, new GUIContent("Unbroken Rain Pipe Variations", "Rain pipe variations array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(gutterContainers, new GUIContent("Gutter Containers", "Gutter containers array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(guttersUnbroken, new GUIContent("Gutters Unbroken", "Gutter unbroken array"), true);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(guttersBroken, new GUIContent("Gutters Broken", "Gutter broken array"), true);

            GUILayout.EndHorizontal();

        }
        else
        {
            for (int i = 0; i < transformsArray.Length; i++)
            {
                if (transformsArray[i].gameObject != this.script.gameObject && !transformsArray[i].gameObject.activeSelf)
                {
                    transformsArray[i].hideFlags = HideFlags.HideInHierarchy;
                }
            }
        }

        GUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.FindProperty("moduleTypeIndex").intValue = script.moduleTypeIndex;

            serializedObject.ApplyModifiedProperties();

            if (script.windowLights != null && script.windowLights.Length > 0)
            {
                script.WindowLightsPropertiesUpdate(script.lightIntensity, script.lightIndirect, script.lightColor);
            }

            if (script.interior != null)
            {
                script.InteriorSwitch(script.isInterior);
            }

            if (script.exteriorBroken != null && script.exteriorUnbroken != null)
            {
                script.ExteriorBrokenVersionSwitch(script.isExteriorBroken);
            }

            if (script.rainPipe != null)
            {
                script.RainPipeSwitch(script.isRainPipe);
            }

            if (script.rainPipeBroken != null)
            {
                script.RainPipeBrokenVersionSwitch(script.isRainPipeBroken);
            }

            if (script.rainPipe != null)
            {
                script.RainPipeChangeSide(script.isTurn);
            }
            if (script.rainPipe != null)
            {
                script.RainPipeMirror(script.isMirror);
            }

            if (script.interiorBroken != null && script.interiorUnbroken != null)
            {
                script.InteriorBrokenVersionSwitch(script.isInteriorBroken);
            }

            if (script.guttersBroken != null && script.guttersUnbroken != null)
            {
                if (script.guttersBroken.Length > 0 && script.guttersUnbroken.Length > 0)
                {
                    script.GuttersBrokenSwitch(script.isGuttersBroken);
                }
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

            if (script.gutterContainers != null)
            {
                if (script.gutterContainers.Length > 0)
                {
                    script.GuttersSwitch(script.isGutters);
                }
            }
        }
    }
}
