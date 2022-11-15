using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metaverse.Data;

public class GameDataTableManager {



	static bool isInitFinish = false;
	public static void InitGameDataTable ( System.Action done ) {

        if (!isInitFinish) 
		{
			if (TSVLoaderTools.loaderData.isRealTime)
			{
				TSVLoaderTools.loader.Download(() =>
				{
					SetupDataTable();
					done?.Invoke();
				});
			}
			else
			{
				SetupDataTable();
				done?.Invoke();
			}
		}
	}

	static void SetupDataTable()
	{
		try
		{
			ConfigData.Init(GetDataTable("ConfigData"));
			TreeData.Init(GetDataTable("TreeData"));
			isInitFinish = true;
		}
		catch (System.Exception e)
		{
			Debug.LogError("[ " + lastDataName + " ]    " + e.Message + "\n" + e.StackTrace);
		}
	}



	static string lastDataName;
	private static GameDataTable GetDataTable(string dataTableKey)
	{
		lastDataName = dataTableKey;
		string Data = GameDataRaw.GetDataBaseSource(dataTableKey);
		return  GameDataTable.ReadData (Data);
	}

}

namespace Metaverse.Data
{
	public class GameDataRaw
	{
		public static string GetDataBaseSource(string database) 
		{
			return TSVLoaderTools.LoadContent(database);
		}
	}
}















