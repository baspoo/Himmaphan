using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SourceFinding : MonoBehaviour
{

}





#if UNITY_EDITOR
public class EditorSourceFinding : EditorWindow
{
    static int selected = 0;
    Vector2 ScrollView;
    [MenuItem("Utility/AssetsBundle/SourceFinding")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorSourceFinding), false, "SourceFinding");
    }
    void Update()
    {
        Repaint();
    }



    public static string path = "";
    GameObject root;
    void OnGUI()
    {
        if (Selection.activeObject == null)
        {
            clear();
            return;
        }


        if (!(Selection.activeObject is GameObject))
        {
            clear();
            EditorGUILayout.TextField(FindPath(Selection.activeObject));
            return;
        }


        root = (GameObject)Selection.activeObject;
        root = (GameObject)EditorGUILayout.ObjectField(root, typeof(GameObject));


      


        if (root != null)
        {
            if (GUILayout.Button("Find"))
            {
                FIND();
            }




            if (Selection.objects.Length > 1)
            {
                var obj = Selection.objects[1];
                if (obj is UnityEditor.DefaultAsset) 
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    path = EditorGUILayout.TextField(FindPath(obj));
                    GUI.backgroundColor = Color.yellow;
                    if (path.notnull() && GUILayout.Button("Move To Path"))
                    {
                        MoveAll(path);
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
            }



        }
        else 
        {
            clear();
        }
        EditorGUILayout.Space(20);


        if (dictValidatePath.Count == 0)
            return;

        ScrollView = EditorGUILayout.BeginScrollView(ScrollView);
       
       
        foreach (var dict in dictValidatePath) 
        {
            EditorGUIService.BeginContents(false);
            if (GUILayout.Button(dict.Key)) 
            {
                MoveAll(dict.Key);
            }
            foreach (var data in dict.Value)
                View(data);
            EditorGUIService.EndContents();
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.Space(20);
        EditorGUILayout.EndScrollView();
       
    }




    void clear() {
        dictValidatePath = new Dictionary<string, List<Object>>();
        selects = new List<string>();
    }


    string FindPath(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        return path;
    }
    string FindFolder(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        var splitName = path.Split('/');
        var name = splitName[splitName.Length - 1];
        path = path.Replace($"/{name}","");
        return path;
    }
    string PathToFileName(string path)
    {
        var splitName = path.Split('/');
        var name = splitName[splitName.Length - 1];
        return name;
    }
    void Ping(Object obj)
    {
        EditorGUIUtility.PingObject(obj);
    }

    void Move(Object obj , string toPath ) 
    {
        FileUtil.MoveFileOrDirectory(FindPath(obj), toPath);
    }

    void Move(string obj, string toPath)
    {
        FileUtil.MoveFileOrDirectory(obj, toPath);
    }
    void MoveAll( string toPath)
    {

        string files = "";
        selects.ForEach(p => {
            var fullpath = $"{toPath}/{PathToFileName(p)}";
            files += $"{p} => {fullpath}\n";
        });


        EditorGUIService.OpenDialog("แน่น๊ะ!", files , () => {
            selects.ForEach(p => {
                var fullpath = $"{toPath}/{PathToFileName(p)}";
                var log = AssetDatabase.MoveAsset(p, fullpath);
                Debug.Log($"{p} => { fullpath } : {log}");
            });
            FIND();
        });
    }


    List<string> selects = new List<string>();
    void View(Object obj) 
    {
        EditorGUILayout.BeginHorizontal();

        var path = FindPath(obj);
        bool isChecking = selects.Contains(path);

        GUI.backgroundColor = isChecking ? Color.green : Color.white;
        if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Valid"), GUILayout.Width(25))) 
        {
            if (!isChecking) selects.Add(path);
            else selects.RemoveAll(x=>x == path);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.ObjectField(obj, obj.GetType());
        EditorGUILayout.TextField(path);
        EditorGUILayout.EndHorizontal();
    }






    void FIND() {
        clear();
        var let = new List<Object>();
        let.AddRange(GetLink(root, let));
        foreach (var g in root.transform.GetAllNode())
            let.AddRange(GetLink(g, let));
        dictValidatePath = OnValidatePath(let);
    }

    Dictionary<string, List<Object>> dictValidatePath = new Dictionary<string, List<Object>>();
    private Dictionary<string, List<Object>> OnValidatePath(List<Object> objs)
    {
        Dictionary<string, List<Object>> dict = new Dictionary<string, List<Object>>();
        foreach (var obj in objs) 
        {
            var path = FindFolder(obj);
            if (!dict.ContainsKey(path))
            {
                dict[path] = new List<Object>() { obj };
            }
            else 
            {
                dict[path].Add(obj);
            }
        }
        return dict;
    }















    List<Object> GetLink( GameObject gameobject, List<Object> stocklet)
    {
        List<Object> let = new List<Object>();
        foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { gameobject }))
        {
            if (obj is Texture || obj is Material || obj is Mesh) 
            {
                if (!stocklet.Contains(obj))
                    if (!let.Contains(obj))
                        let.Add(obj);
            }

        }
        return let;
    }
    IEnumerable<Texture> GetTextures(Renderer renderer)
    {
        foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { renderer }))
        {
            if (obj is Texture)
            {
                yield return obj as Texture;
            }
        }
    }




 













}
#endif






