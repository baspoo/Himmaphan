using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHandle : MonoBehaviour
{

    [SerializeField] Transform trans;
    [SerializeField] UITexture texturescale;
    [SerializeField] HandleData portrait, landscape;
    [System.Serializable]
    public class HandleData {
        public Vector3 position;
        public Vector3 scale;
        public UIWidget.AspectRatioSource keepAspectRatio;
    }



    public static bool isLandscape => Screen.width > Screen.height;


    float run = 0.0f;
    float max = 0.1f;
    void Update()
    {
        if (run < max) run += Time.deltaTime;
        else 
        {
            run = 0.0f;
            Handle(isLandscape? landscape : portrait);
        }
    }

    HandleData mhandle;
    void Handle(HandleData handle)
    {
        if (mhandle == handle)
            return;

        if (trans != null) 
        {
            trans.localPosition = handle.position;
            trans.localScale = handle.scale;
        }
        if (texturescale != null)
        {
            texturescale.keepAspectRatio = handle.keepAspectRatio;
        }
        mhandle = handle;
    }




}
