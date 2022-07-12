using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Rendering;

namespace cardooo.editor.pagetool
{
    public partial class GeneralPreviewScene
    {
        /// <summary>
        /// 视图工具
        /// </summary>
        protected enum ViewTool
        {
            None,
            /// <summary>
            /// 平移
            /// </summary>
            Pan,
            /// <summary>
            /// 缩放
            /// </summary>
            Zoom,
            /// <summary>
            /// 旋转
            /// </summary>
            Orbit
        }
        protected ViewTool m_ViewTool = ViewTool.None;
        protected ViewTool viewTool
        {
            get
            {
                Event evt = Event.current;
                if (m_ViewTool == ViewTool.None)
                {
                    bool controlKeyOnMac = (evt.control && Application.platform == RuntimePlatform.OSXEditor);

                    // actionKey could be command key on mac or ctrl on windows
                    bool actionKey = EditorGUI.actionKey;

                    bool noModifiers = (!actionKey && !controlKeyOnMac && !evt.alt);

                    if ((evt.button <= 0 && noModifiers) || (evt.button <= 0 && actionKey) || evt.button == 2)
                        m_ViewTool = ViewTool.Pan;
                    else if ((evt.button <= 0 && controlKeyOnMac) || (evt.button == 1 && evt.alt))
                        m_ViewTool = ViewTool.Zoom;
                    else if (evt.button <= 0 && evt.alt || evt.button == 1)
                        m_ViewTool = ViewTool.Orbit;
                }
                return m_ViewTool;
            }
        }

        protected MouseCursor currentCursor
        {
            get
            {
                switch (m_ViewTool)
                {
                    case ViewTool.Orbit: return MouseCursor.Orbit;
                    case ViewTool.Pan: return MouseCursor.Pan;
                    case ViewTool.Zoom: return MouseCursor.Zoom;
                    default: return MouseCursor.Arrow;
                }
            }
        }

        void HandleView(Rect drawRect)
        {
            Event evt = Event.current;
            int previewSceneID = GUIUtility.GetControlID(m_PreviewSceneHint, FocusType.Passive);
            EventType type = evt.GetTypeForControl(previewSceneID);
            HandleViewTool(evt, type, previewSceneID, drawRect);

            // Apply the current cursor
            if (evt.type == EventType.Repaint)
                EditorGUIUtility.AddCursorRect(drawRect, currentCursor);
        }


        protected void HandleViewTool(Event evt, EventType eventType, int id, Rect previewRect)
        {
            switch (eventType)
            {
                case EventType.ScrollWheel: DoAvatarPreviewZoom(evt, HandleUtility.niceMouseDeltaZoom * (evt.shift ? 2.0f : 0.5f)); break;
                case EventType.MouseDown: HandleMouseDown(evt, id, previewRect); break;
                case EventType.MouseUp: HandleMouseUp(evt, id); break;
                case EventType.MouseDrag: HandleMouseDrag(evt, id, previewRect); break;
            }
        }

        protected void HandleMouseDown(Event evt, int id, Rect previewRect)
        {
            if (viewTool != ViewTool.None && previewRect.Contains(evt.mousePosition))
            {
                EditorGUIUtility.SetWantsMouseJumping(1);
                evt.Use();
                GUIUtility.hotControl = id;
            }
        }

        protected void HandleMouseUp(Event evt, int id)
        {
            if (GUIUtility.hotControl == id)
            {
                m_ViewTool = ViewTool.None;

                GUIUtility.hotControl = 0;
                EditorGUIUtility.SetWantsMouseJumping(0);
                evt.Use();
            }
        }

        protected void HandleMouseDrag(Event evt, int id, Rect previewRect)
        {
            if (GUIUtility.hotControl == id)
            {
                switch (m_ViewTool)
                {
                    case ViewTool.Orbit: DoAvatarPreviewOrbit(evt, previewRect); break;
                    case ViewTool.Pan: DoAvatarPreviewPan(evt); break;

                    // case 605415 invert zoom delta to match scene view zooming
                    case ViewTool.Zoom: DoAvatarPreviewZoom(evt, -HandleUtility.niceMouseDeltaZoom * (evt.shift ? 2.0f : 0.5f)); break;
                    default: Debug.Log("Enum value not handled"); break;
                }
            }
        }

        public void DoAvatarPreviewZoom(Event evt, float delta)
        {
            float zoomDelta = -delta * 0.05f;
            m_ZoomFactor += m_ZoomFactor * zoomDelta;

            // zoom is clamp too 10 time closer than the original zoom
            m_ZoomFactor = Mathf.Max(m_ZoomFactor, m_AvatarScale / 10.0f);
            evt.Use();
        }

        public void DoAvatarPreviewOrbit(Event evt, Rect previewRect)
        {
            //Reset 2D on Orbit
            if (is2D)
            {
                is2D = false;
            }
            m_PreviewDir -= evt.delta * (evt.shift ? 3 : 1) / Mathf.Min(previewRect.width, previewRect.height) * 140.0f;
            m_PreviewDir.y = Mathf.Clamp(m_PreviewDir.y, -90, 90);
            evt.Use();
        }

        public void DoAvatarPreviewPan(Event evt)
        {
            Camera cam = CurPreviewRenderUtility.camera;
            Vector3 screenPos = cam.WorldToScreenPoint(m_PivotPositionOffset);
            Vector3 delta = new Vector3(-evt.delta.x, evt.delta.y, 0);
            // delta panning is scale with the zoom factor to allow fine tuning when user is zooming closely.
            screenPos += delta * Mathf.Lerp(0.25f, 2.0f, m_ZoomFactor * 0.5f);
            Vector3 worldDelta = cam.ScreenToWorldPoint(screenPos) - (m_PivotPositionOffset);
            m_PivotPositionOffset += worldDelta;
            evt.Use();
        }

    }
}