using System.Collections.Generic;
using UnityEngine;

namespace cardooo.editor.pagetool
{
    public partial class GeneralPreviewScene
    {
        List<GameObject> gameobjecList = new List<GameObject>();
        public List<Animator> AnimatorList { get; private set; } = new List<Animator>();
        public List<ParticleSystem> ParticleSystemList { get; private set; } = new List<ParticleSystem>();
        public void AddSingleGO(GameObject gobj)
        {
            gameobjecList.Add(gobj);
            AnimatorList.AddRange(gobj.GetComponents<Animator>());

            var plist = gobj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in plist)
            {
                p.useAutoRandomSeed = false;
                p.Play();
            }

            ParticleSystemList.AddRange(plist);

            CurPreviewRenderUtility.AddSingleGO(gobj);
        }

        public void ObjectClear()
        {
            foreach (GameObject go in gameobjecList)
            {                
                Object.DestroyImmediate(go);
            }
            AnimatorList.Clear();
            ParticleSystemList.Clear();
            gameobjecList.Clear();
        }

        public Camera GetCamera()
        {
            return CurPreviewRenderUtility.camera;
        }
    }
}