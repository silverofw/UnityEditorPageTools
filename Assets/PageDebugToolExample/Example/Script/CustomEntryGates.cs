using System.Collections.Generic;
using UnityEngine;
using cardooo.editor.pagetool;

public class CustomEntryGates : EntryGates
{
    protected override void ShowBtns()
    {
        base.ShowBtns();
        PageDebugBase page = new CustomPage();
        ShowBotton(new List<PageDebugBase>() { page }, page.CurPageName());
        page = new TimeScaleControl();
        ShowBotton(new List<PageDebugBase>() { page }, page.CurPageName());
        page = new GeneralPreviewScene();
        ShowBotton(new List<PageDebugBase>() { new SceneObjectControl(), page, new AnimatorControl() }, page.CurPageName(), true);
    }
}