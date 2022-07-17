using UnityEngine;
using cardooo.editor.pagetool;

public class CustomPage : PageDebugBase
{
    public static string PageName = "課製分頁";
    public override void ShowGUI()
    {
        m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(CurWidth));
        base.ShowGUI();

        GUILayout.Label($"課製分頁內容");

        GUILayout.EndScrollView();
    }
}
