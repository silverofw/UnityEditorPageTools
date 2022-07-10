using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace cardooo.editor.pagetool
{
	internal class PageEditorWindow : EditorWindow
	{
		public static PageEditorWindow Inst = null;

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
			staticlist.Clear();
			list.Clear();
			willAddlist.Clear();

			staticlist.Add(new EntryGates());
		}

		[MenuItem("Tools/Editor Page Tools #m", false, 21640000)]
		public static void OpenWindow()
		{
			GetWindow(typeof(PageEditorWindow));
			Inst.minSize = new Vector2(640, 480);
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
				list.Remove(v);
			}

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