using UnityEngine;
using System.Collections.Generic;

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

			var page = new TimeScaleControl();
			ShowBotton(new List<PageDebugBase>() { page }, page.CurPageName());

			GUILayout.EndScrollView();
		}

		public void ShowBotton(List<PageDebugBase> pages, string pageName, int height = oneFuncHeight, bool clear = false)
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
