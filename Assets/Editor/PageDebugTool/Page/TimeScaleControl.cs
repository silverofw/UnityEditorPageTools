using UnityEngine;

namespace cardooo.editor.pagetool
{
    public class TimeScaleControl : PageDebugBase
    {
        protected override int DefaultWidth { get; set; } = 350;
        public override string CurPageName() { return "阿迦莫多之眼\nTimeScaleControl"; }

        float timeScale = 1f;
        float MAX_TIME_SCALE = 5f;

        public override void ShowGUI()
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
            base.ShowGUI();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"時空 = 1"))
            {
                Time.timeScale = 1;
            }
            if (GUILayout.Button($"時空 = 0"))
            {
                Time.timeScale = 0;
            }
            if (GUILayout.Button($"時空 = 2"))
            {
                Time.timeScale = 2;
            }
            if (GUILayout.Button($"時空 = 3"))
            {
                Time.timeScale = 3;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"設定時空: {Time.timeScale} >> {timeScale}"))
            {
                Time.timeScale = timeScale;
            }
            GUILayout.EndHorizontal();
            timeScale = GUILayout.HorizontalSlider(timeScale, 0.0f, MAX_TIME_SCALE, GUILayout.Height(20));

            GUILayout.EndScrollView();
        }
    }
}
