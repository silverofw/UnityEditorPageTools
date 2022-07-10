using UnityEngine;
using cardooo.editor.pagetool;

public class CostomPage : PageDebugBase
{
    public static string PageName = "課製分頁";
    public override void ShowGUI()
    {
        base.ShowGUI();

        GUILayout.Label($"課製分頁內容");
    }
}
