using System.Collections;
using UnityEngine;
using UnityEditor;
using cardooo.editor.pagetool;

public class CustomPageEditorWindow : PageEditorWindow
{
    [MenuItem("Tools/CostomPageEditor Page Tools #m", false, 21640000)]
    public static void OpenWindow()
    {
        GetWindow(typeof(CustomPageEditorWindow));
        Inst.minSize = new Vector2(640, 480);
    }

    protected override PageDebugBase setStaticPageList()
    {
        return new CustomEntryGates();
    }
}