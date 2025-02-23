using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NOTLonely_MCS;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(NL_MCS_Manager))]
public class NL_MCS_Manager_editor : OdinEditor
{
    private NL_MCS_Manager manager;

    protected virtual void OnEnable()
    {
        manager = (NL_MCS_Manager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();


        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.Label("Use ONLY for MCS modules!");
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("Remove Unused Modules", "Remove all unused (hidden) modules from the hierarchy in optimization purposes. ATTENTION: this action cannot be undone!")))
        {
                bool option = EditorUtility.DisplayDialog("ATTENTION", "Are you sure, that you want to permanently remove all unused modules? ATTENTION: this action will break prefab connections and cannot be undone!", "Yes", "No");
                switch (option)
                {
                    case true:
                        manager.RemoveUnusedModules();
                        DestroyImmediate(manager);
                        break;
                    case false:
                        break;
                }
        }

        GUILayout.EndHorizontal();
    }
}
