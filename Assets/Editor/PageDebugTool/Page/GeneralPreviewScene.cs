using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Rendering;

namespace cardooo.editor.pagetool
{
    /// <summary>
    /// 參考來源:
    /// 1. https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/AvatarPreview.cs
    /// 
    /// </summary>
    public partial class GeneralPreviewScene : PageDebugBase
    {
        public static GeneralPreviewScene Inst = null;
        
        protected override int DefaultWidth { get; set; } = 640;
        public override string CurPageName() { return "通用預覽場景"; }

        static PreviewRenderUtility CurPreviewRenderUtility = null;

        // 原点圆形标识
        private GameObject m_ReferenceInstance;
        // 方向箭头标识
        private GameObject m_DirectionInstance;
        // 轴三箭头标识
        private GameObject m_PivotInstance;
        // 根三箭头标识
        private GameObject m_RootInstance;
        private Vector2 m_PreviewDir = new Vector2(120f, -20f);
        private Mesh m_FloorPlane;
        private Texture2D m_FloorTexture;
        private Material m_FloorMaterial;

        private static int PreviewCullingLayer = 31;
        private float m_AvatarScale = 1f;

        const string s_PreviewStr = "Preview";
        int m_PreviewHint = s_PreviewStr.GetHashCode();

        const string s_PreviewSceneStr = "PreviewSene";
        int m_PreviewSceneHint = s_PreviewSceneStr.GetHashCode();

        public bool is2D
        {
            get { return m_2D; }
            set
            {
                m_2D = value;
                if (m_2D)
                {
                    m_PreviewDir = new Vector2();
                }
            }
        }

        bool m_2D;
        bool m_IsValid;

        private float m_ZoomFactor = 1.0f;
        private Vector3 m_PivotPositionOffset = Vector3.zero;


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

        public override void ShowGUI()
        {
            Inst = this;

            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
            base.ShowGUI();

            if (CurPreviewRenderUtility == null)
            {
                init();
            }

            Rect drawRect = new Rect(0, 40, CurWidth, PageEditorWindow.Inst.position.height);
            CurPreviewRenderUtility.BeginPreview(drawRect, GUIStyle.none);

            /*
            Quaternion identity = Quaternion.identity;
            Vector3 pos = new Vector3(0f, 0f, 0f);
            pos = m_ReferenceInstance.transform.position;
            Material floorMaterial = m_FloorMaterial;
            Matrix4x4 matrix2 = Matrix4x4.TRS(pos, identity, Vector3.one * 5f * m_AvatarScale);
            floorMaterial.mainTextureOffset = -new Vector2(pos.x, pos.z) * 5f * 0.08f * (1f / m_AvatarScale);
            floorMaterial.SetVector("_Alphas", new Vector4(0.5f * 1f, 0.3f * 1f, 0f, 0f));
            Graphics.DrawMesh(m_FloorPlane, matrix2, floorMaterial, PreviewCullingLayer, CurPreviewRenderUtility.camera, 0);
            */

            float tempZoomFactor = (is2D ? 1.0f : m_ZoomFactor);
            // Position camera
            CurPreviewRenderUtility.camera.orthographic = is2D;
            if (is2D)
                CurPreviewRenderUtility.camera.orthographicSize = 2.0f * m_ZoomFactor;
            CurPreviewRenderUtility.camera.nearClipPlane = 0.5f * tempZoomFactor;
            CurPreviewRenderUtility.camera.farClipPlane = 100.0f * m_AvatarScale;

            Quaternion camRot = Quaternion.Euler(-m_PreviewDir.y, -m_PreviewDir.x, 0);

            // Add panning offset
            Vector3 camPos = camRot * (Vector3.forward * -10.5f * tempZoomFactor) + m_PivotPositionOffset;
            CurPreviewRenderUtility.camera.transform.position = camPos;
            CurPreviewRenderUtility.camera.transform.rotation = camRot;


            //InternalEditorUtility.SetCustomLighting(CurPreviewRenderUtility.lights, new Color(0.6f, 0.6f, 0.6f, 1f));
            CurPreviewRenderUtility.camera.Render();
            var texture = CurPreviewRenderUtility.EndPreview();

            //InternalEditorUtility.RemoveCustomLighting();

            int previewID = GUIUtility.GetControlID(m_PreviewHint, FocusType.Passive, drawRect);
            Event evt = Event.current;
            EventType type = evt.GetTypeForControl(previewID);

            if (type == EventType.Repaint && m_IsValid)
            {
                //DoRenderPreview(drawRect, background);
                //CurPreviewRenderUtility.EndAndDrawPreview(drawRect);
            }

            //AvatarTimeControlGUI(rect);

            int previewSceneID = GUIUtility.GetControlID(m_PreviewSceneHint, FocusType.Passive);
            type = evt.GetTypeForControl(previewSceneID);

            //DoAvatarPreviewDrag(evt, type);
            HandleViewTool(evt, type, previewSceneID, drawRect);
            //DoAvatarPreviewFrame(evt, type, drawRect);

            if (!m_IsValid)
            {
                Rect warningRect = drawRect;
                warningRect.yMax -= warningRect.height / 2 - 16;
                EditorGUI.DropShadowLabel(
                    warningRect,
                    "No model is available for preview.\nPlease drag a model into this Preview Area.");
            }

            // Apply the current cursor
            if (evt.type == EventType.Repaint)
                EditorGUIUtility.AddCursorRect(drawRect, currentCursor);

            GUI.Box(drawRect, texture);

            GUILayout.EndScrollView();
        }

        public override void OnClose()
        {
            base.OnClose();

            ObjectClear();
            Object.DestroyImmediate(m_ReferenceInstance);
            Object.DestroyImmediate(m_DirectionInstance);
            Object.DestroyImmediate(m_PivotInstance);
            Object.DestroyImmediate(m_RootInstance);
            m_ReferenceInstance = null;
            m_DirectionInstance = null;
            m_PivotInstance = null;
            m_RootInstance = null;

            CurPreviewRenderUtility.Cleanup();
            CurPreviewRenderUtility = null;

            Debug.Log("OnClose");
        }

        void init()
        {
            Debug.Log("init");
            var preview = new PreviewRenderUtility();
            /*
            preview.camera.farClipPlane = 500;
            preview.camera.clearFlags = CameraClearFlags.SolidColor;
            preview.camera.transform.position = new Vector3(10, 4, -10);
            preview.camera.transform.eulerAngles = new Vector3(15, -45, 0);
            */

            preview.camera.cameraType = CameraType.SceneView;
            preview.camera.clearFlags = CameraClearFlags.Skybox;

            preview.camera.fieldOfView = 30.0f;
            preview.camera.allowHDR = false;
            preview.camera.allowMSAA = false;
            preview.ambientColor = new Color(.1f, .1f, .1f, 0);
            preview.lights[0].intensity = 1.4f;
            preview.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0);
            preview.lights[1].intensity = 1.4f;

            if (m_FloorPlane == null)
            {
                m_FloorPlane = (Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh);
            }
            if (m_FloorTexture == null)
            {
                m_FloorTexture = (Texture2D)EditorGUIUtility.Load("Avatar/Textures/AvatarFloor.png");
            }
            if (m_FloorMaterial == null)
            {
                Shader shader = null;
                if (!GraphicsSettings.currentRenderPipeline) // if we are in old classic renderer
                {
                    shader = EditorGUIUtility.Load("Previews/PreviewPlaneWithShadow.shader") as Shader;
                }
                else
                {
                    shader = GraphicsSettings.renderPipelineAsset.defaultShader;
                }
                Debug.Log($"m_FloorMaterial shader is {shader}");

                m_FloorMaterial = new Material(shader);
                m_FloorMaterial.mainTexture = m_FloorTexture;
                m_FloorMaterial.mainTextureScale = Vector2.one * 5f * 4f;
                m_FloorMaterial.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0f, 0f));
                m_FloorMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            if (m_ReferenceInstance == null)
            {
                GameObject original = (GameObject)EditorGUIUtility.Load("Avatar/dial_flat.prefab");
                m_ReferenceInstance = Object.Instantiate(original, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_ReferenceInstance);
                preview.AddSingleGO(m_ReferenceInstance);
            }
            if (m_DirectionInstance == null)
            {
                GameObject original2 = (GameObject)EditorGUIUtility.Load("Avatar/arrow.fbx");
                m_DirectionInstance = Object.Instantiate(original2, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_DirectionInstance);
                preview.AddSingleGO(m_DirectionInstance);
            }
            if (m_PivotInstance == null)
            {
                GameObject original3 = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
                m_PivotInstance = Object.Instantiate(original3, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_PivotInstance);
                preview.AddSingleGO(m_PivotInstance);
            }
            if (m_RootInstance == null)
            {
                GameObject original4 = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
                m_RootInstance = Object.Instantiate(original4, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_RootInstance);
                preview.AddSingleGO(m_RootInstance);
            }

            CurPreviewRenderUtility = preview;
            m_2D = false;
            m_IsValid = true;
        }

        private void InitInstantiatedPreviewRecursive(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;
            go.layer = PreviewCullingLayer;
            foreach (Transform transform in go.transform)
            {
                InitInstantiatedPreviewRecursive(transform.gameObject);
            }
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
