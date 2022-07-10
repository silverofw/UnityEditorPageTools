using UnityEngine;

namespace cardooo.editor.pagetool
{
    public class PageDebugBase
    {
        protected virtual int DefaultWidth { get; set; } = 300;
        protected Vector2 m_scrollPosition = Vector2.zero;
        protected float CurWidth = 300;
        protected float hSbarValue = 150f;

        public PageDebugBase()
        {
            CurWidth = DefaultWidth;
        }
        public virtual void ShowGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button($"還原", GUILayout.Width(50)))
            {
                CurWidth = DefaultWidth;
            }
            GUILayout.Label($"[{CurPageName()}]");
            GUILayout.Label($"[目前寬度: {CurWidth.ToString("0.0")}]", GUILayout.Width(100));
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                PageEditorWindow.Inst.Remove(this);                
            }

            GUILayout.EndHorizontal();
            CurWidth = GUILayout.HorizontalSlider(CurWidth, 150f, 1000f, GUILayout.Height(20));
        }

        public virtual void OnClose()
        { 
            
        }

        public virtual string CurPageName() { return "基底頁面"; }
    }
}