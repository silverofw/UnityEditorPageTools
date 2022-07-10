using UnityEngine;

namespace cardooo.editor.pagetool
{
    public class SceneObjectControl : PageDebugBase
    {
        protected override int DefaultWidth { get; set; } = 350;
        public override string CurPageName() { return "場景物件控制"; }


        public override void ShowGUI()
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
            base.ShowGUI();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("清除所有物件"))
            {
                GeneralPreviewScene.Inst.ObjectClear();
            }

            if (GUILayout.Button("Add Cube"))
            {
                GeneralPreviewScene.Inst.AddSingleGO(GameObject.CreatePrimitive(PrimitiveType.Cube));
            }

            GUILayout.EndHorizontal();


            GUILayout.EndScrollView();
        }
    }
}
