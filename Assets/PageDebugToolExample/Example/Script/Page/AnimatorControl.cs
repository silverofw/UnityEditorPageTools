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

        private double m_PreviousTime;
        private float m_RunningTime;
        private float pre_RunningTime;

        private bool pause = false;

        Animator animator;
        AnimationClip clip;

        float maxDuring = 0f;
        string clipName = "WAIT01";

        public AnimatorControl()
        {
            m_PreviousTime = EditorApplication.timeSinceStartup;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            float delta = (float)(EditorApplication.timeSinceStartup - m_PreviousTime);
            if (!pause)
            {
                m_RunningTime += delta;
            }

            if (maxDuring != 0)
            {
                m_RunningTime %= maxDuring;
            }


            foreach (var animator in GeneralPreviewScene.Inst.AnimatorList)
            {
                animator.Update(m_RunningTime - pre_RunningTime);
            }

            foreach (var peartical in GeneralPreviewScene.Inst.ParticleSystemList)
            {
                peartical.Simulate(m_RunningTime, true, true);
            }

            PageEditorWindow.Inst.Repaint();


            m_PreviousTime = EditorApplication.timeSinceStartup;
            pre_RunningTime = m_RunningTime;
        }

        public override void ShowGUI()
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
            base.ShowGUI();

            GUILayout.Label($"當前模擬時間: {m_RunningTime}");
            GUILayout.Label($"目前動畫數量: {GeneralPreviewScene.Inst.AnimatorList.Count}");
            GUILayout.Label($"目前粒子數量: {GeneralPreviewScene.Inst.ParticleSystemList.Count}");
            if (GUILayout.Button("Reset"))
            {
                m_RunningTime = 0;
            }

            if (GUILayout.Button(pause?"復原":"暫停"))
            {
                pause = !pause;
            }

            maxDuring = EditorGUILayout.FloatField("最大週期:", maxDuring);

            if (maxDuring != 0)
            {
                m_RunningTime = GUILayout.HorizontalSlider(m_RunningTime, 0f, maxDuring, GUILayout.Height(20));
            }
            else
            {
                GUILayout.HorizontalSlider(CurWidth, 0f, maxDuring, GUILayout.Height(20));
            }
            if (GeneralPreviewScene.Inst.AnimatorList.Count > 0)
            {
                var clip = GeneralPreviewScene.Inst.AnimatorList[0].GetCurrentAnimatorClipInfo(0)[0].clip;
                if (GUILayout.Button($"設定目前動畫長度: {clip.length}s"))
                {
                    maxDuring = clip.length;
                }

                clipName = EditorGUILayout.TextField("動畫名稱:", clipName);
                if (GUILayout.Button("PLAY"))
                {
                    GeneralPreviewScene.Inst.AnimatorList[0].Play(clipName);
                }
            }

            /* todo
            clip = EditorGUILayout.ObjectField("產出物件: ", clip, typeof(AnimationClip), true) as AnimationClip;
            if (GUILayout.Button("Add"))
            {
                if (clip == null)
                {
                    EditorUtility.DisplayDialog("ERROR",
                    "No object select!",
                    "OK");
                }
                else
                {
                    AnimationEvent evt = new AnimationEvent();
                    evt.functionName = "OXOX";
                    evt.time = 0.2f;

                    AnimationEvent[] animationEvents = new AnimationEvent[] { evt };
                    AnimationUtility.SetAnimationEvents(clip, animationEvents);

                    animationEvents = AnimationUtility.GetAnimationEvents(clip);

                    foreach (var e in animationEvents)
                    {
                        Debug.Log($"{e.functionName}");
                    }
                }
            }

            animator = EditorGUILayout.ObjectField("產出物件: ", animator, typeof(Animator), true) as Animator;
            if (GUILayout.Button("Add"))
            {
                var infos = animator.GetCurrentAnimatorClipInfo(0);
                foreach (var info in infos)
                {
                    Debug.Log($"[clip: {info.clip.name}]");
                }
            }
            */
            GUILayout.EndScrollView();
        }
    }
}
