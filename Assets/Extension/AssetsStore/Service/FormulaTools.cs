using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
public class FormulaTools : MonoBehaviour
{
}
[CustomEditor(typeof(FormulaTools))]
public class FormulaToolsUI : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox("Custom Tools", MessageType.None);
	}
}

[System.Serializable]
public class FormulaDisplayData
{
	public string name;
	public bool isEnable;
}
#endif









//------------------------------------------------------------------------------------
// Wizard
#if UNITY_EDITOR
public class FormulaToolsWindows : EditorWindow
{
	[MenuItem("File/FormulaToolsWindows")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(FormulaToolsWindows));
	}


	public static void ShowWindow(Service.Formula fomula)
	{
		data = "";
		formulas.Clear();
		formulas.Add(fomula);
		ShowWindow();
	}
	public static void ShowWindow(string json)
	{
		data = json;
		Service.Formula fomula = FormulaToolsService.Json.JsonToJFormula(json);
		formulas.Clear();
		formulas.Add(fomula);
		ShowWindow();
	}








	//--  Service
	int nextwidth = 0;
	void Reposition() { nextwidth = 0; }
	Rect Position(Rect position, int width)
	{
		Rect R = new Rect(position.x + nextwidth, position.y, width, position.height);
		nextwidth += width + 5;
		return R;
	}
	public static ToolsType toolsType
	{
		get { return (ToolsType)EditorPrefs.GetInt("formulatoolswindows_gui_toolsType"); }
		set { EditorPrefs.SetInt("formulatoolswindows_gui_toolsType", (int) value); }
	}
	public enum ToolsType
	{
		Convert, LocalStore
	}
	UnityEditor.AnimatedValues.AnimBool m_ShowExtraFields;
	string m_String;
	Color m_Color = Color.white;
	int m_Number = 0;
	bool isJsonArray;
	static Vector2 ScrollView;

	bool isEx = false;
	public static string data { 
		get { return EditorPrefs.GetString("formulatoolswindows_gui_text"); }
		set { EditorPrefs.SetString("formulatoolswindows_gui_text", value); }
	}
	public static List<Service.Formula> formulas = new List<Service.Formula>();

	void OnGUI()
	{

		toolsType = (ToolsType)EditorGUILayout.EnumPopup(toolsType);
		ScrollView = EditorGUILayout.BeginScrollView(ScrollView);


		if (toolsType == ToolsType.Convert)
		{
			EditorStyles.textField.wordWrap = true;
			data = EditorGUILayout.TextArea(data, GUILayout.Height(100.0f));
			if (data != string.Empty)
			{
				EditorGUILayout.BeginHorizontal();
				string text = "text :--> formula";
				string json = "json :--> formula";
				string jsonarray = "json-array :--> formula";
				string type = text;
				if (Service.String.Json.isJson(data,false)) 
				{
					type = json;
				}
				if (Service.String.Json.isJsonArray(data,false))
				{
					type = jsonarray;
				}

				GUI.backgroundColor = Color.black;
				EditorGUILayout.TextField(type);
				GUI.backgroundColor = Color.white;

				if (GUILayout.Button("Convert")) 
				{
					formulas.Clear();

					if (type == text) 
					{
						formulas.Add(Service.Formula.TextSetup(data));
					}
					if (type == json)
					{
						formulas.Add(Service.String.Json.JsonToFormula(data));
					}
					if (type == jsonarray)
					{
						formulas = Service.String.Json.JsonToFormulas(data);
					}
				}
				EditorGUILayout.EndHorizontal();


				//** Display
				foreach (var f in formulas) {
					FormulaToolsService.OnGUI.FormulaDisplay(null, f);
				}
			}



		}

		if (toolsType == ToolsType.LocalStore)
		{
			FormulaToolsService.OnGUI.GUILocalStore.Display();
		}
		EditorGUILayout.EndScrollView();
	}
}

#endif


























public class FormulaToolsService 
{

	#region FormulaToolsService
	public FormulaToolsService Clone
	{
		get { return (FormulaToolsService)this.MemberwiseClone(); }
	}
	#endregion


	//#region LocalPackageContent
	//[System.Serializable]
	//public class LocalPackageContent
	//{
	//	public bool Bol = new bool();
	//	public int Int = 0;
	//	public int[] Ints;
	//	public float Float;
	//	public float[] Floats;
	//	public string String = "";
	//	public Vector2 vecter2 = new Vector2();
	//	public Vector3 vecter3 = new Vector3();
	//	public Vector4 vecter4 = new Vector4();
	//	public Quaternion quaternion = new Quaternion();
	//	public Color color = new Color();
	//	public Rect rect = new Rect();
	//	public Texture texture = null;
	//	public Texture2D texture2D = null;
	//	public AnimationClip animclip = null;
	//	public AnimationCurve animationCurve = null;
	//	public Transform transform = null;
	//	public Material material = null;
	//	public Renderer renderer = null;
	//	public object obj;
	//}
	//#endregion










	#region OnGUI
#if UNITY_EDITOR
	public class OnGUI
	{
		public enum display
		{
			full, mini
		}
		public static void FormulaDisplay(string Name, Service.Formula f, display style = display.full)
		{
			//GUIStyle header = GUIstylePackage.Instant.Header;
			GUIFormulaSerialize.FormulaDataDisplay(f, style);
		}
		public class GUIFormulaSerialize : Service.Formula
		{
			public static string path = "";
			public static List<string> open = new List<string>();
			public static void FormulaDataDisplay(Service.Formula f , display style , string inheader = "", string key = "master", int lv = 0, bool isForce = false)
			{
				Service.Formula read = (string.IsNullOrEmpty(path)) ? f : f.GetSubPath(path).SubFormula;
				EditorGUILayout.BeginVertical();
				string tap = (lv == 0) ? "" : "\t";
				string header = tap + "[ Lv." + lv + "]" + inheader;

				//if (EditorGUIService.DrawHeader(path, key, isForce, false))
				//{
				//		//Back
				//		if (!string.IsNullOrEmpty(path))
				//		{
				//			int iTap = -1;
				//			GUI.backgroundColor = Color.yellow;
				//			iTap = GUILayout.SelectionGrid(iTap, new string[] { "< Back" }, 6);
				//			GUI.backgroundColor = Color.white;
				//			if (iTap != -1)
				//			{
				//				int r = 0;
				//				string newpath = "";
				//				foreach (var s in path.Split('/'))
				//				{
				//					string n = Service.String.AddCommarString(s, newpath).Replace(",", "/");
				//					if (n != path)
				//						newpath = n;
				//				}
				//				path = newpath;
				//			}
				//		}
				//		foreach (var s in read.GetFormulaDatas)
				//		{
				//			FormulaData(s, style, inheader, lv);
				//		}
				//		EditorGUILayout.Space();
				//		EditorGUILayout.Space();
				//}
				EditorGUILayout.EndVertical();
			}
			static void FormulaData(Service.Formula.FormulaData fd , display style , string key, int lv)
			{

				if (style == display.mini)
				{
					EditorGUILayout.LabelField(fd.FormulaName + "  :  " + (fd.GetDataByType()).ToString());
					return;
				}


				EditorGUILayout.BeginHorizontal();
				string Header = string.Empty;
				if (!string.IsNullOrEmpty(fd.GetTag()))
				{
					EditorGUILayout.TextField(fd.GetTag(), GUILayout.Width(40.0f));
				}
				if (!string.IsNullOrEmpty(fd.uniID))
				{
					EditorGUILayout.TextField(fd.uniID, GUILayout.Width(40.0f));
				}
				EditorGUILayout.LabelField(fd.FormulaName, GUILayout.Width(120.0f));
				var datatype = (Service.Formula.FormulaData.datatype)EditorGUILayout.EnumPopup(fd.GetDataType, GUILayout.Width(80.0f));
				if (fd.GetDataType == Service.Formula.FormulaData.datatype.formula)
				{
					GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
					EditorGUILayout.TextField(fd.SubFormula.PassToJson());
					GUI.backgroundColor = Color.white;
					if (GUILayout.Button("open", GUILayout.Width(40.0f)))
					{
						path += ((string.IsNullOrEmpty(path)) ? "" : "/") + fd.FormulaName;
					}
				}
				else if (fd.GetDataType == Service.Formula.FormulaData.datatype.list)
				{
					GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
					//EditorGUILayout.TextField(fd.ListFormulaDatas.PassToJson());
					GUI.backgroundColor = Color.white;
					if (GUILayout.Button("open list [ "+ fd.ListFormulaDatas.GetFormulaDatas .Count.ToString() + " ]"))
					{
						if (!open.Contains(fd.FormulaName))
							open.Add(fd.FormulaName);
						else
						{
							open.Remove(fd.FormulaName);
							return;
						}
					}
					if (open.Contains(fd.FormulaName))
					{
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("\t\t\t");
						EditorGUILayout.BeginVertical();
						EditorGUILayout.LabelField("List");
						foreach (var lit in fd.ListFormulaDatas.GetFormulaDatas)
						{
							EditorGUILayout.BeginHorizontal();
							var type = lit.GetDataType;
							EditorGUILayout.EnumPopup(type, GUILayout.Width(80.0f));
							if(type == Service.Formula.FormulaData.datatype.formula) 
								EditorGUILayout.TextField(lit.SubFormula.PassToJson());
							else if (type == Service.Formula.FormulaData.datatype.list)
								EditorGUILayout.TextField(lit.ListFormulaDatas.PassToJson());
							else 
								EditorGUILayout.TextField(lit.GetDataByType().ToString());
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
					}


				}
				else
				{
					var data = fd.GetDataByType();
					EditorGUILayout.TextField((data != null) ? data.ToString() : "");
					if (fd.SubFormula != null) EditorGUILayout.TextField(fd.SubFormula.PassToJson());
				}
				if (fd.args != null && fd.args.Count > 0)
				{
					GUI.backgroundColor = (!open.Contains(fd.FormulaName)) ? Color.white : new Color(0.0f, 0.0f, 0.0f, 0.1f);
					if (GUILayout.Button(" [" + fd.args.Count + "] "))
					{
						if (!open.Contains(fd.FormulaName))
							open.Add(fd.FormulaName);
						else
						{
							open.Remove(fd.FormulaName);
							return;
						}
					}
					GUI.backgroundColor = Color.white;
					if (open.Contains(fd.FormulaName))
					{
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("\t\t\t");
						EditorGUILayout.BeginVertical();
						EditorGUILayout.LabelField("Arguments");
						foreach (var a in fd.args)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.TextField(a.argName);
							EditorGUILayout.TextField(a.argData);
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
					}
				}
				EditorGUILayout.EndHorizontal();



			}
		}

		public class GUILocalStore
		{
		
			public static string formulaKey
			{
				get { return EditorPrefs.GetString("GUILocalStore:formulaKey"); }
				set { EditorPrefs.SetString("GUILocalStore:formulaKey", value); }
			}
			public static string formulaName
			{
				get { return EditorPrefs.GetString("GUILocalStore:formulaName"); }
				set { EditorPrefs.SetString("GUILocalStore:formulaName", value); }
			}
			public static void Display() { }
			/*
			public static void Display( )
			{
				//formulaKey = EditorGUILayout.TextArea(formulaKey);

				Color color_none = GUI.backgroundColor;
				EditorGUILayout.Space();
				EditorGUILayout.Space();
	
				if (string.IsNullOrEmpty(formulaKey))
				{
					EditorGUILayout.LabelField("["+ LocalStore.GetStoreIDs().Count + "] StoreIDs --------------------------------" );
					EditorGUILayout.Space();
					foreach (string store in LocalStore.GetStoreIDs())
					{
						if (GUILayout.Button(store))
						{
							formulaKey = store;
						}
					}
				}
				else
				{
					if (!LocalStore.IsHave(formulaKey))
					{
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("Not Found ID --------------------------------");
					}
					else
					{

						LocalStore local = new LocalStore(formulaKey);

						//formulaName = EditorGUILayout.TextField("Formula Name : ", formulaName);
						EditorGUILayout.LabelField("path:" + formulaKey +    ((string.IsNullOrEmpty(formulaName))? "" : "/" + formulaName));

						if (string.IsNullOrEmpty(formulaName))
						{
							foreach (string member in local.GetMemberIDs())
							{
								if (GUILayout.Button(member))
								{
									formulaName = member;
								}
							}
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							GUI.backgroundColor = Color.gray;
							if (GUILayout.Button("Back"))
							{
								formulaKey = "";
							}
							GUI.backgroundColor = Color.red;
							if (GUILayout.Button("Remove Store: " + formulaKey))
							{
								if (EditorUtility.DisplayDialog("LocalStore!", "Remove '" + formulaKey + "' Store", "Yes", "No"))
								{
									LocalStore.RemoveStore(formulaKey);
									formulaKey = string.Empty;
									formulaKey = string.Empty;
								}
							}
							GUI.backgroundColor = color_none;
						}
						else
						{
							Service.Formula F = local.GetFormula(formulaName);
							if (F == null)
							{
								EditorGUILayout.Space();
								EditorGUILayout.Space();
								EditorGUILayout.LabelField("Not Found ID --------------------------------");
							}
							else
							{
								OnGUI.FormulaDisplay(formulaName, F, OnGUI.display.full);
								EditorGUILayout.Space();
								EditorGUILayout.Space();
								EditorGUILayout.Space();
								EditorGUILayout.Space();
								EditorGUILayout.Space();
								EditorGUILayout.Space();
								GUI.backgroundColor = Color.gray;
								if (GUILayout.Button("Back"))
								{
									formulaName = "";
								}
								GUI.backgroundColor = Color.red;
								if (GUILayout.Button("Remove Formuala: " + formulaName))
								{
									if (EditorUtility.DisplayDialog("LocalStore!", "Remove '" + formulaName + "' Formula", "Yes", "No"))
									{
										local.Des(formulaName);
									}

								}
								GUI.backgroundColor = color_none;
							}
						}
					}

				}
				GUI.backgroundColor = color_none;
			}
			*/
		}
	}
#endif
	#endregion





	#region Json
	public class Json {

		public static Service.Formula.FormulaData AddSubPath(Service.Formula formula,string path , object value) {

			//	0	/ 1	   / 2 / 3
			// count = 4
			// match/cha_0/hit/kill  : 99
			List<string> s = path.Split('/').ToList<string>();
			Service.Formula root = formula;
			Service.Formula lastpath = formula;
			Service.Formula notfound = null;
			Service.Formula.FormulaData fd = null;
			for (int i = 0; i < s.Count-1; i++) 
			{
				var find = root.GetFormula(s[i]);
				if (find.isHave)
				{
					if (find.GetDataType != Service.Formula.FormulaData.datatype.formula)
					{
						Debug.LogError("invalid path!");
						return null;
					}
					root = find.SubFormula;
					lastpath = root;
				}
				else
				{
					if (notfound == null)
					{
						root = new Service.Formula();
						notfound = root;
						
					}
					root = root.AddFormula(s[i], new Service.Formula()).SubFormula;
				}
			}
			if (notfound != null)
			{
				lastpath.AddFormula(notfound.GetFormulaDatas[0].FormulaName, notfound.GetFormulaDatas[0].SubFormula);
			}

			var checking = root.GetFormula(s[s.Count - 1]);
			if (checking.isHave && checking.GetDataType == Service.Formula.FormulaData.datatype.formula) 
			{
				Debug.LogError("invalid path!");
				return null;
			}

			fd = root.AddOrUpdateFormula(s[s.Count-1], value);
			return fd;
		}
		public static Service.Formula.FormulaData GetSubPath(Service.Formula formula, string path)
		{

			if (string.IsNullOrEmpty(path))
				return (new Service.Formula()).AddFormula("", formula);

			Service.Formula find = formula;
			Service.Formula.FormulaData fd = null;
			List<string> s = path.Split('/').ToList<string>();
			while (s.Count > 0)
			{
				fd = find.GetFormula(s[0]);
				//Debug.Log(s[0]);
				s.RemoveAt(0);
				//next
				find = fd.SubFormula;
			}
			return fd;
		}



		public static string FormulaToJsonArray( Service.Formula formula )
		{
			string array = "";
			foreach (var f in formula.GetFormulaDatas)
				array = Service.String.AddCommarString( f.SubFormula.PassToJson() , array);
			return $"[{array}]";
		}
		public static string FormulasToJsonArray(List<Service.Formula> formulas, bool isMeta = false) {
			string array = "";
			foreach (var f in formulas)
				array = Service.String.AddCommarString(FormulaToJson(f, (isMeta) ? new Service.Formula() : null), array);
			return $"[{array}]";
		}

		public static string FormulaToJson(Service.Formula formula, Service.Formula meta = null)
		{
			string json = string.Empty;
			foreach (var fd in formula.GetFormulaDatas)
			{
				json = Service.String.AddCommarString(FormulaDataToJson(fd , meta), json);
			}
			
			if (formula!=null && formula.GetFormulaDatas.Count > 0 && meta != null)
			{
				json += "," + FormulaDataToJson(new Service.Formula.FormulaData(metakey, meta));
			}
			
			return "{" + json + "}";
		}
		public static string FormulaDataToJson(Service.Formula.FormulaData fd ,  Service.Formula meta = null)
		{

			// json "key" : value  || "key" : {   }
			string GetStringValueFormulaToJsan(Service.Formula.FormulaData FD, Service.Formula META = null)
			{
				string output = "null";
				if (FD.GetDataType == Service.Formula.FormulaData.datatype.str)
				{
					output = "\"" + FD.Text + "\"";
				}
				if (FD.GetDataType == Service.Formula.FormulaData.datatype.num)
				{
					output = FD.Value.ToString();
				}
				if (FD.GetDataType == Service.Formula.FormulaData.datatype.bol)
				{
					output = FD.Status.ToString().ToLower();
				}
				if (FD.GetDataType == Service.Formula.FormulaData.datatype.json)
				{
					output = FD.Json.ToString();
				}
				if (FD.GetDataType == Service.Formula.FormulaData.datatype.formula)
				{
					output = FD.SubFormula.PassToJson(META);
				}
				return output;
			}
			string json = GetStringValueFormulaToJsan(fd, meta);

			// jsonList "key" : [ ]
			if (fd.GetDataType == Service.Formula.FormulaData.datatype.list)
			{
				string argstring = "";
				foreach (var data in fd.ListFormulaDatas.GetFormulaDatas) {
					string row = GetStringValueFormulaToJsan(data);
					argstring = Service.String.AddCommarString(row, argstring);
				}
				json = "[" + argstring + "]";
			}
			return "\"" + ((meta != null) ? GetJsonTagKey(fd, meta) : fd.FormulaName) + "\" : " + json;

		}
		public static List<Service.Formula> JsonArrayToJFormulas(object raw)
		{
			return JsonArrayToJFormulas(ServiceJson.Json.SerializeObject(raw));
		}
		public static List<Service.Formula> JsonArrayToJFormulas(string raw)
		{
			List<Service.Formula> formulas = new List<Service.Formula>();
			if (string.IsNullOrEmpty(raw))
				return formulas;
			var list = ServiceJson.Json.DeserializeObject<List<object>>(raw);
			foreach (object obj in list)
			{
				var f = JsonToJFormula(obj.ToString());
				formulas.Add(f);
			}
			return formulas;
		}
		public static Service.Formula JsonToJFormula(object raw) {
			return JsonToJFormula(raw.SerializeToJson());
		}
		public static Service.Formula JsonToJFormula(string raw)
		{
			Service.Formula formula = new Service.Formula();
			if (string.IsNullOrEmpty(raw))
				return formula;

			var dict = ServiceJson.Json.DeserializeObject<Dictionary<string, object>>(raw);

			//meta
			if (dict != null) { 
				bool IsMeta = dict.ContainsKey(metakey);
				Service.Formula meta = (IsMeta)? JsonToJFormula(dict[metakey].ToString()):null;
				foreach (string key in dict.Keys)
				{
					object value = dict[key];
					if (value != null)
					{
						formula.AddFormula(JsonToJFormulaData(key, value, meta));
					}
				}
			}
			formula.DesFormula(metakey);
			return formula;
		}

		static Service.Formula.FormulaData JsonToJFormulaData( string key ,object value , Service.Formula meta  ) {
			bool IsMeta = meta != null;
			var FD = new Service.Formula.FormulaData(key);

			//Value == Json
			if (value.GetType() == typeof(ServiceJson.JsonObject))
				{
					var sub = JsonToJFormula(value.ToString());
					FD.SubFormula = sub;
					if (IsMeta)
						AdjustKey( FD , key, meta);
				}
			//Value == JsonArray
			else if (value.GetType() == typeof(ServiceJson.JsonArray))
			{

				FD.ListFormulaDatas = new Service.Formula();
				List<object> values = (List<object>)value;
				foreach (object v in values)
				{
					string argData = v.ToString();

					// v == Object   { } 
					if (Service.String.Json.isJson(argData))
					{
						Service.Formula sub = JsonToJFormula(argData);
						FD.AddList(sub);
					}
					// v == Normal "123"
					else
					{
						FD.AddList(v);
					}
				}
				if (IsMeta)
				{
					AdjustKey(FD, key, meta);
				}
			}
			//Value == Normal
			else
			{
				if (Service.Tools.ObjectDefine.IsNumeric(value))
				{
					// Number
					double valuesnumeric = System.Convert.ToDouble(value.ToString());
					FD.Value = valuesnumeric;
					if (IsMeta)
						AdjustKey( FD , key, meta);
				}
				else
				{
					// Text or Other
					FD = new Service.Formula.FormulaData(key, value);
					if (IsMeta)
						AdjustKey(FD , key, meta);
				}
			}

			return FD;
		}











        #region META
		// จัดการ Meta Formula
        static string metakey = "#meta";
		static string GetNormalKey( string key )
		{
			return Service.String.strCropRemove(key, "<m", ">");
		}
		static void AdjustKey(Service.Formula.FormulaData fd , string key , Service.Formula meta) {
			fd.Rename(GetNormalKey(fd.FormulaName));
			var metaData = meta.GetFormula(key);
			if (metaData.isHave) {
				fd.SetTag(metaData.SubFormula.GetFormula("tag").Text);
				fd.SetUniID(metaData.SubFormula.GetFormula("uniID").Text);
				if (metaData.SubFormula.GetFormula("args").isHave) {
					foreach (var arg in metaData.SubFormula.GetFormula("args").SubFormula.GetFormulaDatas)
					{
						fd.AddArg(arg.FormulaName, arg.Text);
					}
				}
			}
			return;
		}
		static string GetJsonTagKey( Service.Formula.FormulaData fd, Service.Formula meta )
		{
			int index = meta.meta;
			meta.meta++;
			string key = $"<m{index}>{fd.FormulaName}";

			Service.Formula metaData = new Service.Formula();
			if (!string.IsNullOrEmpty(fd.GetTag()))		
				metaData.AddFormula("tag", fd.GetTag());
			if (!string.IsNullOrEmpty(fd.uniID))		
				metaData.AddFormula("uniID", fd.uniID);
			if (fd.args.Count > 0){
				Service.Formula args = new Service.Formula();
				foreach (var a in fd.args)
				{
					args.AddFormula(a.argName,a.argData);
				}
				metaData.AddFormula("args", args);
			}
			if(metaData.GetFormulaDatas.Count>0)
				meta.AddFormula(key,metaData);
			return key;
		}








		//public static string CleaningMeta(string eventname , Service.Formula pushFormula) 
		//{

		//	var meta = new Service.Formula();
		//	var output = pushFormula.PassToJson(meta);
		//	//Debug.LogError(eventname +" : "+ output.Length);
		//	return output;
		//	/*
		//	var meta = new Service.Formula();
		//	string json = pushFormula.PassToJson(meta);
		//	ChangeKey(pushFormula);
		//	pushFormula.AddFormula(metakey, meta);
		//	return FormulaToJson(pushFormula, null );
		//	*/
		//}
		//static void ChangeKey(Service.Formula f)
		//{
		//	foreach (var fd in f.GetFormulaDatas)
		//	{
		//		fd.Rename(fd.LocalContent.String);
		//		if (fd.SubFormula != null)
		//			if (fd.SubFormula.GetFormulaDatas.Count > 0)
		//			{
		//				ChangeKey(fd.SubFormula);
		//			}
		//	}
		//}





		public static string CleaningJunkMeta(string json ) 
		{
			Service.Formula pushFormula = Service.String.Json.JsonToFormula(json);
			foreach (var fd in pushFormula.GetFormulaDatas)
			{
				/*
				if(fd!=null)
				if (fd.FormulaName != metakey)
					if (fd.SubFormula != null) 
							if(fd.SubFormula.GetFormulaDatas.Count > 0)
								{
									Debug.Log("CLEAN : " + fd.FormulaName);
									CleaningJunkMeta(fd.SubFormula);
								}
								*/

				if (fd != null) 
				{
					Debug.LogError("CLEAN : " + fd.FormulaName  + " ----  "+(fd.FormulaName != metakey));
				}
			}
			return pushFormula.PassToJson();
		}
		static void CleaningJunkMeta( Service.Formula f ) 
		{

			if (f.GetFormula(metakey).isHave) 
			{
				Debug.LogError("CleaningJunkMeta Find Found!!");
				f.DesFormula(metakey);
			}
			foreach (var fd in f.GetFormulaDatas)
			{
				if (fd.SubFormula != null )
					if(fd.SubFormula.GetFormulaDatas.Count > 0)
						{
							Debug.Log("CLEAN : " + fd.FormulaName);
							CleaningJunkMeta(fd.SubFormula);
						}
			}
			
		}
		
		#endregion
	}
    #endregion

}

