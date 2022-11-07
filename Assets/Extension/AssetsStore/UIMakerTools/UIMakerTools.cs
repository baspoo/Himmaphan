using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

#if UNITY_EDITOR
public class UIMakerToolsWindowEditor : EditorWindow
{
    [MenuItem("Utility/UIMaker")]
    public static void ShowWindow()
    {
        GetWindow(typeof(UIMakerToolsWindowEditor));
    }
    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        UIMakerToolsEditor.Gui();
    }
}
#endif



public class UIMakerTools : MonoBehaviour
{
#if UNITY_EDITOR
    public static UIMakerTools getmaker(string name) 
    {
        var load = AssetDatabase.LoadMainAssetAtPath($"Assets/Extension/AssetsStore/UIMakerTools/Editor/UITemplate/UIMakerTools/{name}.prefab");
        if (load != null)
        {
            return  ((GameObject)load).GetComponent<UIMakerTools>();
        }
        else {
            return null;
        }
    }

#endif


    public List<Note> Notes = new List<Note>();
    [System.Serializable]
    public class Note
    {
        public enum Type { message,color,vector,prefab,texture}
        public Type type;
        public string Message;
        public Color Color;
        public Vector3 Vector;
        public GameObject GObj;
        public Texture Texture;
    }


    public List<ColorData> ColorDatas = new List<ColorData>();

    [System.Serializable]
    public class ColorData {
        public string colorName;
        public Color color;
    }

    public List<UITemplateData> UITemplateDatas = new List<UITemplateData>();

    [System.Serializable]
    public class UITemplateData
    {
        public string uiName;
        public GameObject template;
        public bool view;
    }

    public List<LayoutData> LayoutDatas = new List<LayoutData>();

    [System.Serializable]
    public class LayoutData
    {
        public string name;
        public GameObject root;
        public List<Node> nodes = new List<Node>();
        [System.Serializable]
        public class Node
        {
            public string name;
            public int index;
            public Vector3 scale;
            public Vector3 position;
            public Quaternion rotate;
        }
    }


}




#if UNITY_EDITOR
public class UIMakerToolsEditor
{

    public static void Save()
    {
        PrefabUtility.MergeAllPrefabInstances(tools);
        PrefabUtility.SavePrefabAsset(tools.gameObject );
       
    }
    static Vector2 scrollPos;
  
    static GameObject selete => (Selection.activeObject!=null && Selection.activeObject is GameObject) ? (GameObject)Selection.activeObject : null;
    static UIMakerTools tools => UIMakerTools.getmaker(makerName);
    static string makerName => EditorPrefs.GetString("UIMakerToolsEditor.makerName");


    public static void Gui() 
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();


        var name = makerName;
        name = EditorGUILayout.TextField("Maker Name ",name);
        EditorPrefs.SetString("UIMakerToolsEditor.makerName", name);

        EditorGUI.BeginDisabledGroup(tools == null);
        if (GUILayout.Button("UIMakerTools"))
        {
            Selection.activeObject = tools.gameObject;
        }
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save", GUILayout.Width(40)))
        {
            Save();
        }
        GUI.backgroundColor = Color.white;
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();


        if (tools != null) 
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Note(tools.Notes);
            ColorPage();
            CloneStyle();
            Layout();
            Template();
            EditorGUILayout.EndScrollView();
        }
    }


    public static void Note(List<UIMakerTools.Note> notes )
    {

        if (EditorGUIService.DrawHeader("Note", "Note UIMakerToolsEditor", false, false))
        {
            EditorGUIService.BeginContents(false);

            EditorGUILayout.Space();
            foreach (var c in notes)
            {
                EditorGUILayout.BeginHorizontal();
                c.type = (UIMakerTools.Note.Type)EditorGUILayout.EnumPopup(c.type, GUILayout.Width(80));
               if (c.type == UIMakerTools.Note.Type.message)
                    c.Message = EditorGUILayout.TextArea(c.Message,GUILayout.Height(60));
                if (c.type == UIMakerTools.Note.Type.color) 
                    c.Color = EditorGUILayout.ColorField(c.Color);
                if (c.type == UIMakerTools.Note.Type.vector) 
                    c.Vector = EditorGUILayout.Vector3Field("",c.Vector);
                if (c.type == UIMakerTools.Note.Type.prefab)
                    c.GObj = (GameObject)EditorGUILayout.ObjectField(c.GObj, typeof(GameObject) );
                if (c.type == UIMakerTools.Note.Type.texture)
                    c.Texture = (Texture)EditorGUILayout.ObjectField(c.Texture, typeof(Texture));

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("✘",GUILayout.Width(25)))
                {
                    tools.Notes.Remove(c);
                    return;
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("✚ Add"))
            {
                tools.Notes.Add(new UIMakerTools.Note());
                return;
            }
            EditorGUILayout.Space();
            EditorGUIService.EndContents();
        }
    }

    public static void Layout()
    {

        if (EditorGUIService.DrawHeader("Layout", "Layout UIMakerToolsEditor", false, false))
        {
            EditorGUIService.BeginContents(false);

            EditorGUILayout.Space();
            foreach (var c in tools.LayoutDatas)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ToggleLeft(c.name,c.nodes.Count > 0);
                c.root = (GameObject)EditorGUILayout.ObjectField(c.root, typeof(GameObject));

                EditorGUI.BeginDisabledGroup(c.root == null);
                if (GUILayout.Button("Copy Layout"))
                {
                    if (c.root != null)
                    {
                        c.name = c.root.name;
                        c.nodes = new List<UIMakerTools.LayoutData.Node>();
                        int index = 0;
                        foreach (var g in Service.GameObj.GetAllNode(c.root.transform)) 
                        {
                            c.nodes.Add(new UIMakerTools.LayoutData.Node()
                            {
                                index = index,
                                name = g.name,
                                scale = g.transform.localScale,
                                position = g.transform.localPosition,
                                rotate = g.transform.localRotation
                            }) ;
                            index++;
                        }
                        Save();
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(selete == null);
                if (GUILayout.Button("Apply Layout"))
                {
                    if (selete != null)
                    {
                        int index = 0;
                        foreach (var g in Service.GameObj.GetAllNode(selete.transform))
                        {
                            var node = c.nodes[index];
                            if(index == node.index && node.name == g.name) 
                            { 
                                g.transform.localScale = node.scale;
                                g.transform.localPosition = node.position;
                                g.transform.localRotation = node.rotate;
                            }
                            index++;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();



                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("✘", GUILayout.Width(25)))
                {
                    tools.LayoutDatas.Remove(c);
                    return;
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("✚ Add"))
            {
                tools.LayoutDatas.Add(new UIMakerTools.LayoutData());
                return;
            }
            EditorGUILayout.Space();
            EditorGUIService.EndContents();
        }
    }

    public static void ColorPage() 
    {

        if (EditorGUIService.DrawHeader("Color", "Color UIMakerToolsEditor", false, false))
		{
            EditorGUIService.BeginContents(false);

            EditorGUILayout.Space();
            foreach (var c in tools.ColorDatas)
            {
                EditorGUILayout.BeginHorizontal();
                c.color = EditorGUILayout.ColorField(c.color);
                c.colorName = EditorGUILayout.TextField(c.colorName);

                if (GUILayout.Button("Apply"))
                {
                    if (selete != null) 
                    {
                        if (selete.GetComponent<UITexture>() != null) 
                        {
                            selete.GetComponent<UITexture>().color = c.color;
                            selete.GetComponent<UITexture>().Update();
                        }
                        if (selete.GetComponent<UILabel>() != null)
                        {
                            selete.GetComponent<UILabel>().color = c.color;
                            selete.GetComponent<UILabel>().Update();
                        }


                        selete.gameObject.SetActive(false);
                        selete.gameObject.SetActive(true);
                    }

                   
                    RefreshMonitor();
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("✘", GUILayout.Width(25)))
                {
                    tools.ColorDatas.Remove(c);
                    return;
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("✚ Add"))
            {
                tools.ColorDatas.Add(new UIMakerTools.ColorData());
                return;
            }
            EditorGUILayout.Space();
            EditorGUIService.EndContents();
        }
    }




    static bool Texture, BtnColor, Label;
    public static void CloneStyle()
    {

        if (EditorGUIService.DrawHeader("CloneStyle", "CloneStyle UIMakerToolsEditor", false, false))
        {
            EditorGUIService.BeginContents(false);

            EditorGUILayout.Space();


            Texture = EditorGUILayout.ToggleLeft("Texture", Texture);
            BtnColor = EditorGUILayout.ToggleLeft("BtnColor", BtnColor);
            Label = EditorGUILayout.ToggleLeft("Label", Label);
            EditorGUILayout.Space(10);

            var Objs = Selection.objects.ToList();
            var isCanClone = Objs.Count >= 2;

            if (isCanClone)
            {
                var master = Objs[0];
                Objs.Remove(master);
                string to = "";
                foreach (var b in Objs)
                    to += $"{b.name},";
                EditorGUILayout.TextField($"From : ", master.name);
                EditorGUILayout.TextField($"To : ", to);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Clone"))
                {
                    foreach (var b in Objs)
                        Apply((GameObject)master, (GameObject)b , Texture, BtnColor, Label);
                }
            }
            EditorGUILayout.Space();
            EditorGUIService.EndContents();
        }
    }

    public static void Template()
    {

        if (EditorGUIService.DrawHeader("Template", "Template UIMakerToolsEditor", false, false))
        {
            EditorGUIService.BeginContents(false);

            EditorGUILayout.Space();
            foreach (var c in tools.UITemplateDatas)
            {
                EditorGUILayout.BeginHorizontal();


                if (c.template == null)
                    c.view = false;
                EditorGUI.BeginDisabledGroup(c.template == null);
                if (GUILayout.Button(c.view ? "▲" : "▼", GUILayout.Width(24)))
                {
                    c.view = !c.view;
                }
                EditorGUI.EndDisabledGroup();

                c.uiName = EditorGUILayout.TextField(c.uiName);
                c.template = (GameObject)EditorGUILayout.ObjectField(c.template, typeof(GameObject));



                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("✘", GUILayout.Width(25)))
                {
                    tools.UITemplateDatas.Remove(c);
                    return;
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();
                if (c.view)
                {
                    EditorGUILayout.BeginHorizontal();


                    Texture texture = null;
                    var textureui = c.template.GetComponent<UITexture>();
                    if (textureui == null)
                        texture = AssetPreview.GetAssetPreview(c.template);
                    else 
                    {
                        texture = textureui.mainTexture;
                    }

                    if (GUILayout.Button(texture, GUILayout.Width(120), GUILayout.Height(120))) {

                       // var path = AssetDatabase.GetAssetPath(c.template);
                        //PrefabUtility.LoadPrefabContentsIntoPreviewScene(path, new UnityEngine.SceneManagement.Scene());
                        AssetDatabase.OpenAsset(c.template);
                    }


                    EditorGUI.BeginDisabledGroup(selete == null || c.template == null);
                    if (GUILayout.Button("Create"))
                    {
                        var g = c.template.Create(selete.transform);
                        g.name = c.template.name;
                        Service.GameObj.ResetTransform(g.transform);
                    }
                    if (GUILayout.Button("Apply"))
                    {
                        Apply(c.template.gameObject,selete.gameObject);
                    }
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);
                }
            }
            if (GUILayout.Button("✚ Add"))
            {
                tools.UITemplateDatas.Add(new UIMakerTools.UITemplateData());
                return;
            }
            EditorGUILayout.Space();
            EditorGUIService.EndContents();
        }
    }


    static void Apply(GameObject by , GameObject to, bool _Texture = true, bool _BtnColor = true, bool _Label = true) 
    {
        if (_Label && by.GetComponent<UILabel>() != null && to.GetComponent<UILabel>()!=null) 
        {
            
            var label_by = by.GetComponent<UILabel>();
            var label_to = to.GetComponent<UILabel>();
            int depth = label_to.depth;
            EditorUtility.CopySerialized(label_by, label_to);
            label_to.depth = depth;
            label_to.Update();
        }
        if (_Texture && by.GetComponent<UITexture>() != null && to.GetComponent<UITexture>() != null)
        {
        
            var texture_by = by.GetComponent<UITexture>();
            var texture_to = to.GetComponent<UITexture>();
            int depth = texture_to.depth;
            bool isMathSize = texture_by.mainTexture == texture_to.mainTexture;

 
            int height = texture_to.height;
            int width = texture_to.width;
            var keepAspectRatio = texture_to.keepAspectRatio;

            EditorUtility.CopySerialized(texture_by, texture_to);

            if (isMathSize)
            {
                texture_to.keepAspectRatio = keepAspectRatio;
                texture_to.height = height;
                texture_to.width = width;
            }


            texture_to.depth = depth;
            texture_to.Update();
        }
        if (_BtnColor && by.GetComponent<UIButton>() != null && to.GetComponent<UIButton>() != null)
        {
            var ui_by = by.GetComponent<UIButton>();
            var ui_to = to.GetComponent<UIButton>();

            ui_to.duration = ui_by.duration;
            ui_to.defaultColor = ui_by.defaultColor;
            ui_to.hover = ui_by.hover;
            ui_to.pressed = ui_by.pressed;
            ui_to.disabledColor = ui_by.disabledColor;

            if (by.GetComponent<ServiceButtonTexture>() != null)
            {
                var serviceBtn = by.GetComponent<ServiceButtonTexture>();
                var serviceBtn_to = to.GetComponent<ServiceButtonTexture>();
                if (serviceBtn_to == null)
                    serviceBtn_to = to.gameObject.AddComponent<ServiceButtonTexture>();
                EditorUtility.CopySerialized(serviceBtn, serviceBtn_to);
                serviceBtn_to.uiBtn = ui_to;
                serviceBtn_to.uiTexture = ui_to.GetComponent<UITexture>();
            }
        }
        if (by.GetComponent<UIScrollBar>() != null && to.GetComponent<UIScrollBar>() != null)
        {
            var ui_by = by.GetComponent<UIButton>();
            var ui_to = to.GetComponent<UIButton>();

            ui_to.duration = ui_by.duration;
            ui_to.defaultColor = ui_by.defaultColor;
            ui_to.hover = ui_by.hover;
            ui_to.pressed = ui_by.pressed;
            ui_to.disabledColor = ui_by.disabledColor;


            var texture_by = by.GetComponent<UITexture>();
            var texture_to = to.GetComponent<UITexture>();
            int depth = texture_to.depth;
            EditorUtility.CopySerialized(texture_by, texture_to);
            texture_to.depth = depth;
            texture_to.Update();


            Apply(by.transform.GetChild(0).gameObject, to.transform.GetChild(0).gameObject);
            

        }
        RefreshMonitor();
    }





    private static void RefreshMonitor()
    {

        Canvas.ForceUpdateCanvases();
        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor")).Repaint();
        SceneView.RepaintAll();
        EditorWindow.GetWindow<SceneView>().Repaint();
        HandleUtility.Repaint();
    }



}
#endif