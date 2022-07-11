using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Rendering;
namespace cardooo.editor.pagetool
{

    public class AnimatorControl : PageDebugBase
    {
        public static string PageName = "動畫控制";

        private float m_RunningTime;
        private double m_PreviousTime;

        private bool pause = false;
        public override void OnUpdate()
        {
            base.OnUpdate();
            float delta = (float)(EditorApplication.timeSinceStartup - m_PreviousTime);
            m_PreviousTime = EditorApplication.timeSinceStartup;
            if (pause)
                delta = 0;
            m_RunningTime += delta;

            foreach (var animator in GeneralPreviewScene.Inst.AnimatorList)
            {
                animator.Update(delta);
            }

            foreach (var peartical in GeneralPreviewScene.Inst.ParticleSystemList)
            {
                //var time = m_RunningTime % peartical.main.duration;
                //var time = m_RunningTime % 100;
                peartical.Simulate(m_RunningTime, true, true);
            }

            PageEditorWindow.Inst.Repaint();
        }

        public override void ShowGUI()
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
            base.ShowGUI();

            GUILayout.Label($"當前模擬時間: {m_RunningTime}");
            if (GUILayout.Button("Reset"))
            {
                m_RunningTime = 0;
            }

            if (GUILayout.Button("PAUSE"))
            {
                pause = !pause;
            }

            GUILayout.Label($"目前動畫數量: {GeneralPreviewScene.Inst.AnimatorList.Count}");
            GUILayout.Label($"目前粒子數量: {GeneralPreviewScene.Inst.ParticleSystemList.Count}");

            GUILayout.EndScrollView();
        }
    }
}
