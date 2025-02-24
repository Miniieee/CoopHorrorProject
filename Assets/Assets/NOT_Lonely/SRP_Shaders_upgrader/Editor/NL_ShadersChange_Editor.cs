#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class NL_ShadersChange_Editor : EditorWindow
{
    public Shader[] sourceShaders;
    public Shader[] targetShaders;

    public MeshRenderer[] allRenderers;

    public static NL_ShadersChange_Editor shaderFixWindow;

    private int changedCount = 0;

    [MenuItem("Window/NOT_Lonely/Shader Swapper", false)]
    public static void OpenWindow()
    {
        shaderFixWindow = GetWindow<NL_ShadersChange_Editor>();
        shaderFixWindow.titleContent = new GUIContent("Shader Swapper");

        shaderFixWindow.maxSize = new Vector2(480, 1000);
        shaderFixWindow.minSize = new Vector2(400, 330);
    }

    public void FixSRPShaders()
    {
        FindRenderers();

        for (int i = 0; i < allRenderers.Length; i++)
        {
            for (int j = 0; j < allRenderers[i].sharedMaterials.Length; j++)
            {
                if (allRenderers[i].sharedMaterials[j] == null)
                    continue;

                if (allRenderers[i].sharedMaterials[j].shader == Shader.Find("NOT_Lonely/MaskBlend3Colors_NL"))
                {
                    allRenderers[i].sharedMaterials[j].shader = Shader.Find("NOT_Lonely/MaskBlend3Colors_NL_graph");
                    changedCount++;
                }

                if(allRenderers[i].sharedMaterials[j].shader == Shader.Find("MaskBlend_NL"))
                {
                    allRenderers[i].sharedMaterials[j].shader = Shader.Find("NOT_Lonely/MaskBlend_NL_graph");
                    changedCount++;
                }

                if (allRenderers[i].sharedMaterials[j].shader == Shader.Find("NOT_Lonely/MaskBlend_NL"))
                {
                    allRenderers[i].sharedMaterials[j].shader = Shader.Find("NOT_Lonely/MaskBlend_NL_graph");
                    changedCount++;
                }

                if (allRenderers[i].sharedMaterials[j].shader == Shader.Find("NOT_Lonely/MaskBlendSimple_NL"))
                {
                    allRenderers[i].sharedMaterials[j].shader = Shader.Find("NOT_Lonely/MaskBlendSimple_NL_graph");
                    changedCount++;
                }

                if (allRenderers[i].sharedMaterials[j].shader == Shader.Find("NOT_Lonely/NOT_Lonely_RotateUVs"))
                {
                    allRenderers[i].sharedMaterials[j].shader = Shader.Find("NOT_Lonely/NOT_Lonely_RotateUVs_graph");
                    changedCount++;
                }
                if(allRenderers[i].sharedMaterials[j].shader == Shader.Find("NOT_Lonely/TranslucentAnim_NL"))
                {
                    allRenderers[i].sharedMaterials[j].shader = Shader.Find("NOT_Lonely/TranslucentAnim_NL_graph");
                    changedCount++;
                }   
            }

        }
        Debug.Log("Shaders changed successfully for " + changedCount + " materials.");
    }

    void FindRenderers()
    {
        allRenderers = FindObjectsOfType<MeshRenderer>();
    }

    private void OnGUI()
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty _sourceShaders = so.FindProperty("sourceShaders");
        SerializedProperty _targetShaders = so.FindProperty("targetShaders");

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Drop the shaders you want to be replaced from your Project window \n to the 'Source Shaders' list bellow.", EditorStyles.centeredGreyMiniLabel);
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(_sourceShaders, true);

        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Drop the shaders you want replace to from your Project window \n to the 'Target Shaders' list bellow. \n These shaders must be added accordingly to the 'Source Shaders'.", EditorStyles.centeredGreyMiniLabel);
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(_targetShaders, true);
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Swap shaders from Source to Target", "All materials in the scene will be scanned for the 'Source Shaders'. All found 'Source Shaders' will be replaced with the 'Target Shaders'."), GUILayout.Width(230)))
        {
            SwapShaders(sourceShaders, targetShaders);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Swap shaders from Target to Source", "All materials in the scene will be scanned for the 'Target Shaders'. All found 'Target Shaders' will be replaced with the 'Source Shaders'."), GUILayout.Width(230)))
        {
            SwapShaders(targetShaders, sourceShaders);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.color = Color.red;
        if (GUILayout.Button(new GUIContent("FIX SRP SHADERS"), GUILayout.Width(230)))
        {
            FixSRPShaders();
        }
        GUI.color = Color.white;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        so.ApplyModifiedProperties();
    }

    void SwapShaders(Shader[] from, Shader[] to)
    {
        changedCount = 0;
        FindRenderers();
        if (sourceShaders != null && targetShaders != null)
        {
            if (sourceShaders.Length == 0)
            {
                Debug.LogError("The 'Source Shaders' list is empty! Add some shaders first.");
                return;
            }

            if (targetShaders.Length == 0)
            {
                Debug.LogError("The 'Target Shaders' list is empty! Add some shaders first.");
                return;
            }

            if (targetShaders.Length != sourceShaders.Length)
            {
                Debug.LogError("The 'Source Shaders' and the 'Target Shaders' lists have different numbers of shaders. Make sure to fill these lists accordingly to each other.");
                return;
            }

            for (int i = 0; i < from.Length; i++)
            {
                for (int j = 0; j < allRenderers.Length; j++)
                {
                    for (int k = 0; k < allRenderers[j].sharedMaterials.Length; k++)
                    {
                        if (allRenderers[j].sharedMaterials[k] == null)
                            continue;

                        if (allRenderers[j].sharedMaterials[k].shader == from[i])
                        {
                            if (to[i] == null)
                            {
                                Debug.LogError("Source and Target shaders lists filled wrong. There's no Target for this Source shader.");

                                continue;
                            }
                            allRenderers[j].sharedMaterials[k].shader = to[i];
                            changedCount++;
                        }
                    }
                }
            }
            Debug.Log("Shaders changed successfully for " + changedCount + " materials.");
        }
    }
}
#endif
