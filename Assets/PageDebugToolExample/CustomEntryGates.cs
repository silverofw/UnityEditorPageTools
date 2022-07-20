using System.Collections.Generic;
using UnityEngine;
using cardooo.editor.pagetool;

public class CustomEntryGates : EntryGates
{
    protected override void ShowBtns()
    {
        base.ShowBtns();
        CustomPage page = new CustomPage();
        ShowBotton(new List<PageDebugBase>() { page }, page.CurPageName());
    }
}