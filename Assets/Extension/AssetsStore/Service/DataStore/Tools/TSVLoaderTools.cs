#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FileData{
	[SerializeField]
	public string Name;
	public string GID;
	public bool finish;
	public string GetContent {
		get {
			return TSVLoaderTools.LoadContent(Name);
		}
	}
}

public class TSVLoaderTools : MonoBehaviour {
	static TSVLoaderTools m_loader;
	public  static bool isHave {
		get
		{
			return (m_loader != null);
		}
	}
	public  static TSVLoaderTools loader
	{
		get{
			finish = false;
			if(m_loader!=null)
				DestroyImmediate (m_loader.gameObject);
			m_loader = null;
			m_loader = Instantiate (loaderData.gameObject).GetComponent<TSVLoaderTools>();
			if(Application.isPlaying)
				DontDestroyOnLoad(m_loader);
			return m_loader;
		}
	}
	public static TSVLoaderTools loaderData
	{
		get
		{
			//return MonsterWarBuildSetting.Instance.BuildData.Tsvloader;
			return ((GameObject)Resources.Load("Database/_tsvloader")).GetComponent<TSVLoaderTools>();
		}
	}
	static string FullPath {
		// ที่อยู่ของ File .Tsv == คือที่เดียวกับที่วางไฟล์  _loader.prefab
		get
		{
			//Assets/Core/Service/Tools/DataStore/Resources/Database/_loader.prefab
			#if UNITY_EDITOR
			string path = AssetDatabase.GetAssetPath(loaderData);
			path = path.Replace($"{loaderData.name}.prefab", "");
			return path;
			#else
						return "";
			#endif
		}
	}
	public static string LoadFile(string FileName) {
		string path = "Database/" + FileName;
		//Debug.Log("LoadFile Path:" + path);
		TextAsset mytxtData = (TextAsset)Resources.Load(path);
		if (mytxtData == null)
			Debug.LogError("FileName Is Null!!!! : " + FileName);
		string Data = mytxtData.text;
		return Data;
	}
	public static string LoadContent(string FileName)
	{

		if (loaderData.isRealTime) 
		{
			return LoadLocal(FileName);
		}

		if (loaderData.loadAssetType == LoadAssetType.File)
		{
			return TSVLoaderTools.LoadFile(FileName);
		}
		else
		{
			if (IsHaveLocal(FileName))
				return LoadLocal(FileName);
			else
				return LoadFile(FileName);
		}
	}











	public enum LoadAssetType { File , PlayerPref }
	public LoadAssetType loadAssetType;
	public bool isRealTime;
	
	[Header("GoogleSheet Id")]
	public string GoogleSheetID;

	public List<FileData> FileDatas = new List<FileData>();
	int count ;
	int maxcount ;

	public static bool finish;
	public static bool isLoading;
	public void Download( Service.Callback.callback onfinish = null )
	{
		count = 0;
		maxcount = FileDatas.Count;
		foreach (FileData f in FileDatas) {
			StartCoroutine (load( f , onfinish ));
		}
	}
	public void Download( FileData fileData , Service.Callback.callback onfinish = null ){
		count = 0;
		maxcount = 1;
		StartCoroutine (load( fileData , onfinish ));
	}
	public void Stop(   ){
		if (m_loader != null) 
		{
			if (Application.isPlaying)
				Destroy(m_loader.gameObject);
			else
				DestroyImmediate(m_loader.gameObject);
		}
		m_loader = null;
	}
	IEnumerator load (FileData file , Service.Callback.callback onfinish = null ){

		Debug.Log("file name : " + file.Name);
		isLoading = true;
		file.finish = false;
		string filePath = FullPath + file.Name + ".txt";

		Debug.Log("Save Path : " + filePath);
		Debug.Log( "Loading Data from Google Sheet id : "+ GoogleSheetID);
		string URL = "https://docs.google.com/spreadsheets/d/" +  GoogleSheetID  +  "/export?gid="+  file.GID  +"&exportFormat=tsv";
		WWW www = new WWW (URL);
		yield return www;
		count++;
		if (www.isDone) 
		{
			if (www.error == null) 
			{
				if (this.loadAssetType == LoadAssetType.File)
				{
					#if UNITY_EDITOR
					FileUtil.DeleteFileOrDirectory(filePath);
					File.WriteAllText(filePath, www.text);
					AssetDatabase.ImportAsset(filePath);
					#endif
				}
				else 
				{
					SaveLocal(file.Name, www.text);
				}
				Debug.Log (file.Name + " : <color=green> Success </color> [" + count+"/"+ maxcount+"]");
				file.finish = true;
			} 
			else 
			{
				Debug.Log (  file.Name + " : <color=red> Fail => </color>" + www.error);
			}
		}
		else 
		{
			Debug.Log (  file.Name + " : <color=red> Fail </color>");
		}

		if (count == maxcount) 
		{
			isLoading = false;
			finish = true;
			Debug.Log ("<color=yellow> ----------- FINISH -------------</color>" + Application.isPlaying);
			Stop();
			if (onfinish!=null)
				onfinish();
		}
	}

	static string localkey = "TSVLoaderTools";

	static string fileName(string name) {
		return $"[GameData:{name}]";
	}


	public static void SaveLocal( string name , string text ) {
		PlayerPrefs.SetString(fileName(name), text );
	}
	public static string LoadLocal(string name)
	{
		return PlayerPrefs.GetString(fileName(name));
	}
	public static bool IsHaveLocal(string name)
	{
		return PlayerPrefs.HasKey(fileName(name));
	}
}
