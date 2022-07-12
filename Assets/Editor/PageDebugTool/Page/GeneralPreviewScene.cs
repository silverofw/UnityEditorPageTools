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
        public bool m_IsValid { get; private set; } = true;

        private float m_ZoomFactor = 1.0f;
        private Vector3 m_PivotPositionOffset = Vector3.zero;

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

            if(m_IsValid)
                CurPreviewRenderUtility.camera.Render();
            var texture = CurPreviewRenderUtility.EndPreview();

            HandleView(drawRect);

            GUI.Box(drawRect, texture);

            GUILayout.EndScrollView();
        }

        public override void OnClose()
        {
            base.OnClose();

            ObjectClear();

            CurPreviewRenderUtility.Cleanup();
            CurPreviewRenderUtility = null;

            //Debug.Log("OnClose");
        }

        void init()
        {
            //Debug.Log("init");
            CurPreviewRenderUtility = new PreviewRenderUtility();

            CurPreviewRenderUtility.camera.cameraType = CameraType.SceneView;
            CurPreviewRenderUtility.camera.clearFlags = CameraClearFlags.Skybox;

            CurPreviewRenderUtility.camera.fieldOfView = 30.0f;
            CurPreviewRenderUtility.camera.allowHDR = false;
            CurPreviewRenderUtility.camera.allowMSAA = false;
            CurPreviewRenderUtility.ambientColor = new Color(.1f, .1f, .1f, 0);
            CurPreviewRenderUtility.lights[0].intensity = 1.4f;
            CurPreviewRenderUtility.lights[0].transform.rotation = Quaternion.Euler(30f, 100f, 0);
            CurPreviewRenderUtility.lights[1].intensity = 1.4f;

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
                AddSingleGO(m_ReferenceInstance);
            }

            if (m_DirectionInstance == null)
            {
                GameObject original2 = (GameObject)EditorGUIUtility.Load("Avatar/arrow.fbx");
                m_DirectionInstance = Object.Instantiate(original2, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_DirectionInstance);
                AddSingleGO(m_DirectionInstance);
            }
            if (m_PivotInstance == null)
            {
                GameObject original3 = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
                m_PivotInstance = Object.Instantiate(original3, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_PivotInstance);
                AddSingleGO(m_PivotInstance);
            }
            if (m_RootInstance == null)
            {
                GameObject original4 = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
                m_RootInstance = Object.Instantiate(original4, Vector3.zero, Quaternion.identity);
                InitInstantiatedPreviewRecursive(m_RootInstance);
                AddSingleGO(m_RootInstance);
            }
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

        public void ChangeSceneMode()
        {
            ObjectClear();
            m_IsValid = !m_IsValid;
        }
    }
}
