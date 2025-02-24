using UnityEngine;
using System.Collections;
using UnityEditor;
using Sirenix.OdinInspector.Editor;


namespace NOTLonely_MCS
{
    [CustomEditor(typeof(MaterialSetup))]
    public class MaterialSetupEditor : OdinEditor
    {
        MtlEditor editor;

        bool isExpanded;

        private void OnEnable()
        {
            editor = new MtlEditor(target as MaterialSetup);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Setup"))
            {
                (target as MaterialSetup).SetupData();
            }
            isExpanded = EditorGUILayout.Foldout(isExpanded, "Materials");

            if(isExpanded)
            {
                EditorGUI.indentLevel++;
                editor.OnGUI();
                EditorGUI.indentLevel--;
            }
        }
    }
}