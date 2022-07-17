using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace cardooo.editor.pagetool
{
	public abstract class PageEditorWindow : EditorWindow
	{
		public static PageEditorWindow Inst = null;

		protected abstract PageDebugBase setStaticPageList();

		List<PageDebugBase> staticlist = new List<PageDebugBase>();
		bool clearTrigger = false;
		List<PageDebugBase> list = new List<PageDebugBase>();
		List<PageDebugBase> willAddlist = new List<PageDebugBase>();
		List<PageDebugBase> willRemovelist = new List<PageDebugBase>();

		private void Awake()
		{
			titleContent = new GUIContent("Editor Page Tools", "歡迎光臨~");
			autoRepaintOnSceneChange = true;

			Inst = this;
			foreach (PageDebugBase b in staticlist)
			{ 
				b.OnClose();
			}
			staticlist.Clear();
			foreach (PageDebugBase b in list)
			{
				b.OnClose();
			}
			list.Clear();
			willAddlist.Clear();

			staticlist.Add(setStaticPageList());
		}

        private void Update()
        {
			foreach (var v in list)
			{
				v.OnUpdate();
			}
		}

        private void OnDestroy()
        {
			Awake();
        }

        void OnGUI()
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("刷新", GUILayout.Width(300)))
			{
				Awake();
			}
			GUILayout.Label($"{Info()}]");
			GUILayout.EndHorizontal();
			DrawUILine(Color.gray);

			if (clearTrigger)
			{
				clearTrigger = false;
				list.Clear();
			}

			foreach (var v in willRemovelist)
			{
				v.OnClose();
				list.Remove(v);
			}
			willRemovelist.Clear();

			foreach (var v in willAddlist)
			{
				list.Add(v);
			}
			willAddlist.Clear();

			GUILayout.BeginHorizontal();
			foreach (var v in staticlist)
			{
				v.ShowGUI();
			}
			foreach (var v in list)
			{
				v.ShowGUI();
			}
			GUILayout.EndHorizontal();
		}

		public void Add(PageDebugBase entityDebugBase)
		{
			willAddlist.Add(entityDebugBase);
		}

		public void Remove(PageDebugBase entityDebugBase)
		{
			willRemovelist.Add(entityDebugBase);
		}

		public void Clear()
		{
			clearTrigger = true;
		}

		public string Info()
		{
			return $"歡迎光臨~很高興為您服務~當前分頁數量有 {list.Count} 個";
		}

		public static void DrawUILine(Color color, int thickness = 1, int padding = 5)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			r.height = thickness;
			r.y += padding / 2;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}
    }
}