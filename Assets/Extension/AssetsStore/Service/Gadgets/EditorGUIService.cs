#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.Linq;

public class EditorGUIService 
{

	public static GUIStyle lb_Bold => EditorStyles.boldLabel;
	public static GUIStyle lb_large => EditorStyles.largeLabel;


	[MenuItem("Edit/DeleteAll EditorPrefs")]
	public static void DeleteAllEditorPrefs()
	{
		EditorPrefs.DeleteAll();
	}

	//Label Style
	public static void LableBlod(string text) => EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
	public static void LableLarge(string text) => EditorGUILayout.LabelField(text, EditorStyles.largeLabel);






	public class EditorData { 
		string m_key;
		public EditorData(string key) {
			m_key = key;
		}
		public int Value
		{
			get { return EditorPrefs.GetInt(m_key); }
			set { EditorPrefs.SetInt(m_key,value); }
		}
		public string Text
		{
			get { return EditorPrefs.GetString(m_key); }
			set { EditorPrefs.SetString(m_key, value); }
		}
		public bool Bool
		{
			get { return EditorPrefs.GetBool(m_key); }
			set { EditorPrefs.SetBool(m_key, value); }
		}
		public bool isHave
		{
			get { return EditorPrefs.HasKey(m_key); }
		}
	}

	public static string Copyed => EditorGUIUtility.systemCopyBuffer;

	public static void Ping (Object ping)=> EditorGUIUtility.PingObject(ping);
	public static Texture2D BtnIcon(string str) => EditorGUIUtility.FindTexture(str);

	public class Popup : EditorWindow
	{
		Vector2 ScrollView;
		static System.Action Delegate = null;
		public static void ShowWindow(string popupname, System.Action data)
		{
			Delegate = data; 
			EditorWindow.GetWindow(typeof(Popup), true, popupname);
		}
		void OnGUI()
		{
			if (Delegate != null)
			{
				ScrollView = EditorGUILayout.BeginScrollView(ScrollView);
				Delegate();
				EditorGUILayout.EndScrollView();
			}
		}
	}


	


	public class AdjustClass : MonoBehaviour{

		/// <summary>
		/// Gets the internal class ID of the specified type.
		/// </summary>

		static public int GetClassID(System.Type type)
		{
			var go = EditorUtility.CreateGameObjectWithHideFlags("Temp", HideFlags.HideAndDontSave);
			var uiSprite = go.AddComponent(type);
			var ob = new SerializedObject(uiSprite);
			var classID = ob.FindProperty("m_Script").objectReferenceInstanceIDValue;
			MonoBehaviour.DestroyImmediate(go);
			return classID;
		}

		/// <summary>
		/// Gets the internal class ID of the specified type.
		/// </summary>

		static public int GetClassID<T>() where T : MonoBehaviour
		{
			return GetClassID(typeof(T));
		}

		/// <summary>
		/// Convenience function that replaces the specified MonoBehaviour with one of specified type.
		/// </summary>

		static public SerializedObject ReplaceClass(MonoBehaviour mb, System.Type type)
		{
			var id = GetClassID(type);
			var ob = new SerializedObject(mb);
			ob.Update();
			ob.FindProperty("m_Script").objectReferenceInstanceIDValue = id;
			ob.ApplyModifiedProperties();
			ob.Update();
			return ob;
		}

		/// <summary>
		/// Convenience function that replaces the specified MonoBehaviour with one of specified class ID.
		/// </summary>

		static public SerializedObject ReplaceClass(MonoBehaviour mb, int classID)
		{
			var ob = new SerializedObject(mb);
			ob.Update();
			ob.FindProperty("m_Script").objectReferenceInstanceIDValue = classID;
			ob.ApplyModifiedProperties();
			ob.Update();
			return ob;
		}

		/// <summary>
		/// Convenience function that replaces the specified MonoBehaviour with one of specified class ID.
		/// </summary>

		static public void ReplaceClass(SerializedObject ob, int classID)
		{
			ob.FindProperty("m_Script").objectReferenceInstanceIDValue = classID;
			ob.ApplyModifiedProperties();
			ob.Update();
		}

		/// <summary>
		/// Convenience function that replaces the specified MonoBehaviour with one of specified class ID.
		/// </summary>

		static public void ReplaceClass(SerializedObject ob, System.Type type)
		{
			ob.FindProperty("m_Script").objectReferenceInstanceIDValue = GetClassID(type);
			ob.ApplyModifiedProperties();
			ob.Update();
		}

		/// <summary>
		/// Convenience function that replaces the specified MonoBehaviour with one of specified type.
		/// </summary>

		static public T ReplaceClass<T>(MonoBehaviour mb) where T : MonoBehaviour
		{
			return ReplaceClass(mb, typeof(T)).targetObject as T;
		}

	}
	public class AdjustTexture {
		private static Texture2D mBackdropTex;
		private static Texture2D mContrastTex;
		private static Texture2D mGradientTex;
		private static Object mPrevious;

		/// <summary>
		/// Returns a blank usable 1x1 white texture.
		/// </summary>

		static public Texture2D blankTexture
		{
			get
			{
				return EditorGUIUtility.whiteTexture;
			}
		}

		/// <summary>
		/// Returns a usable texture that looks like a dark checker board.
		/// </summary>

		static public Texture2D backdropTexture
		{
			get
			{
				if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
					new Color(0.1f, 0.1f, 0.1f, 0.5f),
					new Color(0.2f, 0.2f, 0.2f, 0.5f));
				return mBackdropTex;
			}
		}

		/// <summary>
		/// Returns a usable texture that looks like a high-contrast checker board.
		/// </summary>

		static public Texture2D contrastTexture
		{
			get
			{
				if (mContrastTex == null) mContrastTex = CreateCheckerTex(
					new Color(0f, 0f, 0f, 0.5f),
					new Color(1f, 1f, 1f, 0.5f));
				return mContrastTex;
			}
		}

		/// <summary>
		/// Gradient texture is used for title bars / headers.
		/// </summary>

		static public Texture2D gradientTexture
		{
			get
			{
				if (mGradientTex == null) mGradientTex = CreateGradientTex();
				return mGradientTex;
			}
		}

		/// <summary>
		/// Create a white dummy texture.
		/// </summary>

		private static Texture2D CreateDummyTex()
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.name = "[Generated] Dummy Texture";
			tex.hideFlags = HideFlags.DontSave;
			tex.filterMode = FilterMode.Point;
			tex.SetPixel(0, 0, Color.white);
			tex.Apply();
			return tex;
		}

		/// <summary>
		/// Create a checker-background texture
		/// </summary>

		private static Texture2D CreateCheckerTex(Color c0, Color c1)
		{
			Texture2D tex = new Texture2D(16, 16);
			tex.name = "[Generated] Checker Texture";
			tex.hideFlags = HideFlags.DontSave;

			for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
			for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
			for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
			for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

			tex.Apply();
			tex.filterMode = FilterMode.Point;
			return tex;
		}

		/// <summary>
		/// Create a gradient texture
		/// </summary>

		private static Texture2D CreateGradientTex()
		{
			Texture2D tex = new Texture2D(1, 16);
			tex.name = "[Generated] Gradient Texture";
			tex.hideFlags = HideFlags.DontSave;

			Color c0 = new Color(1f, 1f, 1f, 0f);
			Color c1 = new Color(1f, 1f, 1f, 0.4f);

			for (int i = 0; i < 16; ++i)
			{
				float f = Mathf.Abs((i / 15f) * 2f - 1f);
				f *= f;
				tex.SetPixel(0, i, Color.Lerp(c0, c1, f));
			}

			tex.Apply();
			tex.filterMode = FilterMode.Bilinear;
			return tex;
		}
	}

	public static class GizmosUtils
	{
		public static void DrawText(GUISkin guiSkin, string text, Vector3 position, Color? color = null, int fontSize = 0, float yOffset = 0)
		{
#if UNITY_EDITOR
			var prevSkin = GUI.skin;
			if (guiSkin == null)
				Debug.LogWarning("editor warning: guiSkin parameter is null");
			else
				GUI.skin = guiSkin;

			GUIContent textContent = new GUIContent(text);

			GUIStyle style = (guiSkin != null) ? new GUIStyle(guiSkin.GetStyle("Label")) : new GUIStyle();
			if (color != null)
				style.normal.textColor = (Color)color;
			if (fontSize > 0)
				style.fontSize = fontSize;

			if (Camera.current == null)
				return;

			Vector2 textSize = style.CalcSize(textContent);
			Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);

			if (screenPoint.z > 0) // checks necessary to the text is not visible when the camera is pointed in the opposite direction relative to the object
			{
				var worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f, screenPoint.y + textSize.y * 0.5f + yOffset, screenPoint.z));
				UnityEditor.Handles.Label(worldPosition, textContent, style);
			}
			GUI.skin = prevSkin;
#endif
		}
	}


	public static void OpenDialog(string header ,string message, System.Action action) 
	{
		if(EditorUtility.DisplayDialog(header, message , "yes", "no"))action?.Invoke();
	}


	static public void RegisterUndo(string name, Object obj) { if (obj != null) UnityEditor.Undo.RecordObject(obj, name); }
	static public void RegisterUndo(string name, params Object[] objects) { if (objects != null && objects.Length > 0) UnityEditor.Undo.RecordObjects(objects, name); }

	public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
	{
		bool state = EditorPrefs.GetBool(key, true);

		if (!minimalistic) GUILayout.Space(3f);
		if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;

		if (minimalistic)
		{
			if (state) text = "\u25BC" + (char)0x200a + text;
			else text = "\u25BA" + (char)0x200a + text;

			GUILayout.BeginHorizontal();
			GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
			if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
			GUI.contentColor = Color.white;
			GUILayout.EndHorizontal();
		}
		else
		{
			text = "<b><size=11>" + text + "</size></b>";
			if (state) text = "\u25BC " + text;
			else text = "\u25BA " + text;
			if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
		}

		if (GUI.changed) EditorPrefs.SetBool(key, state);

		if (!minimalistic) GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!forceOn && !state) GUILayout.Space(3f);
		return state;
	}


	static bool mEndHorizontal = true;
	static public void BeginContents(bool minimalistic)
	{
		if (!minimalistic)
		{
			mEndHorizontal = true;
			GUILayout.BeginHorizontal();
			EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
		}
		else
		{
			mEndHorizontal = false;
			EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
			GUILayout.Space(10f);
		}
		GUILayout.BeginVertical();
		GUILayout.Space(2f);
	}
	static public void EndContents()
	{
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (mEndHorizontal)
		{
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
		}

		GUILayout.Space(3f);
	}


	static public void BeginEndnable(bool isEnable)
	{
		EditorGUI.BeginDisabledGroup(!isEnable);
	}
	static public void EndVisible()
	{
		EditorGUI.EndDisabledGroup();
	}



	
	public class SimpleViewObjectEdit : EditorWindow
	{
		static object m_data;
		static string m_raw;
		public static void ShowWindow(ref object data)
		{
			m_data = data;
			EditorWindow.GetWindow(typeof(SimpleViewObjectEdit), true, "Object Edit");
		}
		Vector2 ScrollView;
		void OnGUI()
		{
			 m_raw = JsonConvert.SerializeObject(m_data, Formatting.Indented);
			EditorStyles.textField.wordWrap = true;
			EditorGUILayout.TextArea(m_raw, GUILayout.Height(200.0f));
			if (GUILayout.Button("Convert"))
			{
				m_data = JsonConvert.DeserializeObject(m_raw);
				Close();
			}
		}
	}

	public static void Dict(Dictionary<string, int> dict){
		foreach (var d in dict)
			EditorGUILayout.IntField(d.Key, d.Value);
	}
	public static void Dict(Dictionary<string, long> dict){
		foreach (var d in dict)
			EditorGUILayout.LongField(d.Key, d.Value);
	}
	public static void Dict(Dictionary<string, double> dict){
		foreach (var d in dict)
			EditorGUILayout.DoubleField(d.Key, d.Value);
	}
	public static void Dict(Dictionary<string, float> dict){
		foreach (var d in dict)
			EditorGUILayout.FloatField(d.Key, d.Value);
	}
	public static void Dict(Dictionary<string, string> dict){
		foreach (var d in dict)
			EditorGUILayout.TextField(d.Key, d.Value);
	}





	public class ListView
	{


		static void head(string name, System.Action action, System.Action add)
		{
			if (EditorGUIService.DrawHeader(name, "ListView-" + name, false, false))
			{
				EditorGUIService.BeginContents(false);
				{
					action?.Invoke();
					if (GUILayout.Button("+"))
					{
						add?.Invoke();
					}
				}
				EditorGUIService.EndContents();
			}
		}
		static void content(System.Action content, System.Action remove)
		{
			EditorGUILayout.BeginHorizontal();
			content?.Invoke();
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("X", GUILayout.Width(40.0f)))
			{
				remove?.Invoke();
			}
			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndHorizontal();
		}




		//ตัวอย่างการใช้ Other Objects
		static void example()
		{

			List<GameObject> list = new List<GameObject>();

			EditorGUIService.ListView.Objects("test", list.Count, (i) => {
				//ในช่องแนวนอนนั้นอยากให้เก็บฟิลอะไรบ้าง ได้มากกว่า 1 คอลลั้ม.... 
				list[i] = (GameObject)EditorGUILayout.ObjectField(list[i], typeof(GameObject));
			}, () => {
				list.Add(null);
			}, (i) => {
				list[i] = null;
			});

		}

		public static void Objects(string name, int count, System.Action<int> contentlist, System.Action add, System.Action<int> remove)
		{
			head(name, () => {
				for (int i = 0; i < count; i++)
				{
					content(() => {
						contentlist?.Invoke(i);
					}, () => { remove?.Invoke(i); });
				}
			}, () => { add?.Invoke(); });
		}


		public static void GameObjects(string name, List<GameObject> list)
		{
			head(name, () => {
				if (list != null)
					for (int i = 0; i < list.Count; i++)
					{
						content(() => {
							list[i] = (GameObject)EditorGUILayout.ObjectField(list[i], typeof(GameObject));
						}, () => { list.Remove(list[i]); });
					}
			}, () => { list.Add(null); });
		}
		public static void Transforms(string name, List<Transform> list)
		{
			head(name, () => {
				if (list != null)
					for (int i = 0; i < list.Count; i++)
					{
						content(() => {
							list[i] = (Transform)EditorGUILayout.ObjectField(list[i], typeof(Transform));
						}, () => { list.Remove(list[i]); });
					}
			}, () => { list.Add(null); });
		}
		public static void Ints(string name, List<int> list)
		{
			head(name, () => {
				if (list != null) for (int i = 0; i < list.Count; i++)
					{
						content(() => {
							list[i] = EditorGUILayout.IntField(list[i]);
						}, () => { list.Remove(list[i]); });
					}
			}, () => { list.Add(0); });
		}
		public static void Floats(string name, List<float> list)
		{
			head(name, () => {
				if (list != null) for (int i = 0; i < list.Count; i++)
					{
						content(() => {
							list[i] = EditorGUILayout.FloatField(list[i]);
						}, () => { list.Remove(list[i]); });
					}
			}, () => { list.Add(0); });
		}
		public static void Doubles(string name, List<double> list)
		{
			head(name, () => {
				if (list != null) for (int i = 0; i < list.Count; i++)
					{
						content(() => {
							list[i] = EditorGUILayout.DoubleField(list[i]);
						}, () => { list.Remove(list[i]); });
					}
			}, () => { list.Add(0); });
		}
		public static void Strings(string name, List<string> list)
		{
			head(name, () => {
				if (list != null) for (int i = 0; i < list.Count; i++)
					{
						content(() => {
							list[i] = EditorGUILayout.TextField(list[i]);
						}, () => { list.Remove(list[i]); });
					}
			}, () => { list.Add(string.Empty); });
		}
	}









	static Dictionary<string, string> dict = new Dictionary<string, string>();
	public static string Popuplist(string header , string defaultdata , string[] datas)
	{
		string val = defaultdata;
		if (dict.ContainsKey(header))
			val = dict[header];


		var lit = datas.ToList();
		int indexOld = lit.FindIndex(x => x == val);
		if (indexOld == -1) indexOld = 0;
		indexOld = EditorGUILayout.Popup(indexOld, datas);


		val = datas[indexOld];
		if (dict.ContainsKey(header))
			dict[header] = val;
		else
			dict.Add(header, val);

		return val;
	}


	public static string Popuplist(string header, string defaultdata, List<Object> datas)
	{
		string[] str = new string[datas.Count];
		for (int i = 0; i < str.Length; i++) 
		{
			str[i] = (datas[i] == null)? string.Empty :  datas[i].name;
		}
		return Popuplist(header, defaultdata, str);
	}
	public static string Popuplist(string header, string defaultdata, Object[] datas)
	{
		string[] str = new string[datas.Length];
		for (int i = 0; i < str.Length; i++)
		{
			str[i] = (datas[i] == null) ? string.Empty : datas[i].name;
		}
		return Popuplist(header, defaultdata, str);
	}




	static Dictionary<string, Object[]> dictobjs = new Dictionary<string, Object[]>();
	static Dictionary<string, System.DateTime> dicttime = new Dictionary<string, System.DateTime>();
	public delegate Object[] callbackObjects();
	public static bool Delay(string header) => Delay(header, 3.0f, null);
	public static bool Delay(string header , System.Action action ) => Delay(header,3.0f, action);
	public static bool Delay(string header,float refresh = 3,System.Action action = null)
	{
		if (!dicttime.ContainsKey(header))
		{
			action?.Invoke();
			dicttime.Add(header, System.DateTime.Now);
			return true;
		}
		else
		{
			var lasttime = dicttime[header];
			bool isTimeout = System.DateTime.Now.AddSeconds(-refresh) > lasttime;
			if (isTimeout)
			{
				action?.Invoke();
				dicttime[header] = System.DateTime.Now;
			}
			return isTimeout;
		}
		return false;
	}

	public static string Popuplist(string header, string defaultdata, callbackObjects action)
	{
		Object[] datas = null;
		if (Delay(header))
		{
			datas = action();

			if (!dictobjs.ContainsKey(header))
				dictobjs.Add(header, datas);
			else
				dictobjs[header] = datas;
		}
		else
		{
			datas = dictobjs[header];
		}

		string[] str = new string[datas.Length];
		for (int i = 0; i < str.Length; i++)
		{
			str[i] = (datas[i] == null) ? string.Empty : datas[i].name;
		}
		return Popuplist(header, defaultdata, str);
	}


}

#endif