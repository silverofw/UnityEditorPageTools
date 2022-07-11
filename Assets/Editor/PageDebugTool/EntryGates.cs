using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace cardooo.editor.pagetool
{
	/// <summary>
	/// 主要功能入口列表
	/// Hint:需要新增課製分頁請繼承後複寫 ShowGUI()
	/// </summary>
	public class EntryGates : PageDebugBase
    {
		protected override int DefaultWidth { get; set; } = 150;
		const int oneFuncHeight = 40;

		public override void ShowGUI()
        {
			m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
			//base.ShowGUI();此頁面為固定視窗

			/*
            IEnumerable<Type> subclassTypes = Assembly
			   .GetAssembly(typeof(PageDebugBase))
			   .GetTypes()
			   .Where(t => t.IsSubclassOf(typeof(PageDebugBase)));

			foreach (Type subclassType in subclassTypes)
			{
				Debug.Log($"[PageDebugBase] >> {subclassType}");
			}
			*/
			

			PageDebugBase page = new TimeScaleControl();
			ShowBotton(new List<PageDebugBase>() { page }, page.CurPageName());
			page = new GeneralPreviewScene();
			ShowBotton(new List<PageDebugBase>() { new SceneObjectControl(), page , new AnimatorControl()}, page.CurPageName(), true);

			GUILayout.EndScrollView();
		}

		public void ShowBotton(List<PageDebugBase> pages, string pageName, bool clear = false, int height = oneFuncHeight)
		{
			if (GUILayout.Button($"{pageName}", GUILayout.Height(height)))
			{
				if (clear)
					PageEditorWindow.Inst.Clear();

				foreach (var item in pages)
                {
					PageEditorWindow.Inst.Add(item);
                }
			}
		}
	}
}
