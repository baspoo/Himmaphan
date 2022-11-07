using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class CaptureImage : MonoBehaviour
{
    #if UNITY_EDITOR
    public RuntimeBtn Capture = new RuntimeBtn("Snap",(r)=> {
        r.Gameobject.GetComponent<CaptureImage>().Snap() ;
    });
    #endif


    public RenderTexture renderTexture;
    public void Snap()
    {
#if UNITY_EDITOR

        RenderTexture rt = renderTexture;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes;
        bytes = tex.EncodeToPNG();
        tex.Apply();
        string path = AssetDatabase.GetAssetPath(rt) + ".png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("Saved to " + path);
#endif
    }

}
