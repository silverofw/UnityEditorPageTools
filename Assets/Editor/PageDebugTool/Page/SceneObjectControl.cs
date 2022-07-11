using UnityEngine;
using UnityEditor;

namespace cardooo.editor.pagetool
{
    public class SceneObjectControl : PageDebugBase
    {
        protected override int DefaultWidth { get; set; } = 350;
        public override string CurPageName() { return "場景物件控制"; }

        public Object source;

        public override void ShowGUI()
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
            base.ShowGUI();

            if (GUILayout.Button("清除所有物件"))
            {
                GeneralPreviewScene.Inst.ObjectClear();
            }

            if (GUILayout.Button("Add Cube"))
            {
                GeneralPreviewScene.Inst.AddSingleGO(GameObject.CreatePrimitive(PrimitiveType.Cube));
            }

            EditorGUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField("產出物件: ", source, typeof(Object), true);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Add"))
            {
                if (source == null)
                {
                    EditorUtility.DisplayDialog("ERROR",
                    "No object select!", 
                    "OK");
                }                
                else
                    GeneralPreviewScene.Inst.AddSingleGO(GameObject.Instantiate((GameObject)source));
            }


            GUILayout.EndScrollView();
        }
    }
}
