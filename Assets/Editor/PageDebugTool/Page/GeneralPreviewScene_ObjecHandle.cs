using System.Collections.Generic;
using UnityEngine;

namespace cardooo.editor.pagetool
{
    public partial class GeneralPreviewScene
    {
        List<GameObject> list = new List<GameObject>();
        public void AddSingleGO(GameObject gobj)
        {
            list.Add(gobj);
            CurPreviewRenderUtility.AddSingleGO(gobj);
        }

        public void ObjectClear()
        {
            foreach (GameObject go in list)
            {
                Object.DestroyImmediate(go);
            }
            list.Clear();
        }
    }
}