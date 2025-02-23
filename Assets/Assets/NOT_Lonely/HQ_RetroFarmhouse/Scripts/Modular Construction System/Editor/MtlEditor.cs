    using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace NOTLonely_MCS
{
    public class MtlEditor
    {
        MaterialSetup setup;
        private int ID;

        public MtlEditor(MaterialSetup setup)
        {
            this.setup = setup;
        }

        public void OnGUI()
        {

            for (int i = 0; i < setup.Data.Length; i++)
            {
                drawData(i, setup.Data[i]);
            }
        }

        void drawData(int id, MaterialSlot data)
        {
            EditorGUI.BeginChangeCheck();
            if (id % 2 == 0)
            {
                GUI.color = new Color(0, 0, 0, 0.08f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = new Color(0, 0, 0, 0.0f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;
            }
             Material mat = EditorGUILayout.ObjectField(id.ToString(), data.Material, typeof(Material), false) as Material;

            if (id % 2 == 0)
            {
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                if(mat != data.Material)
                {
                    Object[] objects = new Object[data.UniqueRenderers.Count + 1];
                    for (int i = 0; i < objects.Length - 1; i++)
                    {
                        objects[i] = data.UniqueRenderers[i];
                    }
                    objects[objects.Length - 1] = setup;
                    Undo.RecordObjects(objects, "Material changed");
                    data.Material = mat;
                    data.UpdateMaterial();
                }
            }
        }
    }
}