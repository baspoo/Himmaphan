using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ResourcesHandle
{
    public enum LoadType
    {
        Editor , LocalFile , CloudFile , StreamingAssets , MockupResources
    }
    public enum FileType
    {
       img, png, jpg, prefab , txt , json , mp3 , mp4 
    }

    static LoadType GetLoadType 
    {
        get 
        {
            //#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return (AssetsBundleHandle.instance != null)?  AssetsBundleHandle.instance.LoadType : LoadType.Editor ;
            }
            else
            {
                return AssetsBundleHandle.assetsbundleHandle.LoadType == LoadType.MockupResources? LoadType.MockupResources : LoadType.Editor;
            }
            //#endif
            //return LoadType.CloudFile;
        }
    }


    public static void Init( System.Action<bool> done )
    {
        var assetsbundle = AssetsBundleHandle.Init();
        if (GetLoadType == LoadType.Editor || GetLoadType == LoadType.MockupResources) 
        {
            done?.Invoke(true);
        }
        else
        {
            //bool islocal = GetLoadType == LoadType.LocalFile;
            assetsbundle.OnStartDownloading(GetLoadType, ()=> { 
                // Start Download.
                //InitializePage.OpenPage().OpenDownloadPage();
                }, 
                (r)=> {
                    // Done Download.
                    //if (!r) 
                       // InitializePage.Instant.OpenWarringPage("err_downloadassetsbundle");
                    done(r); 
                });
        }
    }








    public static Object Load(string bundle, string path, FileType filetype = FileType.img) => Load(GetLoadType , bundle, path, filetype );
    public static Object LoadEditor(string bundle, string path, FileType filetype = FileType.img) => Load(LoadType.Editor, bundle, path, filetype);
    static Object Load(LoadType loadtype , string bundle , string path , FileType filetype = FileType.img ) 
    {

        //Debug.Log($"Load : {loadtype} / {bundle} / {path} / {filetype} ");

        if (loadtype == LoadType.Editor) 
        {
            #if UNITY_EDITOR
            Object Obj = null;
            if (filetype == FileType.img)
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.png}") }";
                Obj = AssetDatabase.LoadMainAssetAtPath(finalpath);
                if (Obj == null) 
                {
                    finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.jpg}") }";
                    Obj = AssetDatabase.LoadMainAssetAtPath(finalpath);
                }
                if (Obj == null) Debug.LogWarning($"NotFound : {finalpath}");
            }
            else 
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{filetype}") }";
                Obj = AssetDatabase.LoadMainAssetAtPath(finalpath);
                if (Obj == null) Debug.LogWarning($"NotFound : {finalpath}");
            }
           
            return Obj;
            #endif
        }
        if (loadtype == LoadType.MockupResources)
        {
            var fullpath = $"AssetsBundle/{bundle}/{path}";
            ///Debug.LogError(fullpath);
            return Resources.Load(fullpath);
        }

         




        if (!Application.isPlaying)
            return null;

        if (loadtype != LoadType.Editor && loadtype != LoadType.MockupResources)
        {
            Object Obj = null;
            if (filetype == FileType.img)
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.png}") }";
                Obj = AssetsBundleHandle.instance.OnLoadAsset(bundle, finalpath);
                if (Obj == null)
                {
                    finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.jpg}") }";
                    Obj = AssetsBundleHandle.instance.OnLoadAsset(bundle, finalpath);
                }
            }
            else
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{filetype}") }";
                Obj = AssetsBundleHandle.instance.OnLoadAsset(bundle, finalpath);
                if (Obj!=null && filetype == FileType.prefab)
                {
                    var gameobject = (GameObject)Obj;
                }
            }
            return Obj;
        }

        return null;
    }





    public static Object[] LoadAll(string bundle, string path) => LoadAll(GetLoadType, bundle, path);
    public static Object[] LoadAllEditor(string bundle, string path) => LoadAll(LoadType.Editor, bundle, path);
    static Object[] LoadAll(LoadType loadtype , string bundle, string path)
    {
        if (loadtype == LoadType.Editor)
        {
            #if UNITY_EDITOR
            List<Object> let = new List<Object>();
            var finalpath = $"{AssetsBundlePath.FullPathInput}/{bundle}";
            if (!string.IsNullOrEmpty(path)) finalpath += "/" + path;

            var dir = new DirectoryInfo(finalpath);
            if(dir.Exists)
            foreach (var file in dir.GetFiles()){
                if (!file.Name.Contains(".meta")) { 
                    var source = Load(bundle, $"{path}/{file.Name}");
                    if (source != null)
                    {
                        let.Add(source);
                    }
                }
            }
            Debug.Log(finalpath + " // "+ let.Count.ToString());
            return let.ToArray();
            #endif
        }
        if (loadtype == LoadType.MockupResources)
        {
            return Resources.LoadAll($"AssetsBundle/{bundle}/{path}");
        }

        if (!Application.isPlaying)
            return null;

        if (loadtype == LoadType.LocalFile || loadtype == LoadType.CloudFile)
        {
            List<Object> let = new List<Object>();
            var assetsbandle = AssetsBundleHandle.instance.GetAssetBundle(bundle);
            if (assetsbandle != null) 
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}";
                List<string> files = new List<string>();
                foreach (var str in assetsbandle.GetAllAssetNames()) 
                {
                    if (str.ToLower().Contains(finalpath.ToLower())) 
                    {
                        var file = AssetsBundleHandle.instance.OnLoadAsset( bundle , str );
                        let.Add(file);
                    }
                }
            }
            return let.ToArray();
        }


        return null;
    }


}
