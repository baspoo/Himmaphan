using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class ConfigData
{

    public bool Enable;
    public string Id;
    public string Name;
    public string Description;


    public ConfigData(GameData raw)
    {
        //Varable
        raw.GetValue("Enable", out Enable);
        raw.GetValue("Id", out Id);
        raw.GetValue("Name", out Name);
        raw.GetValue("Description", out Description);
    }








    public static ConfigData Find(string treeID)
    {
        if (Datas.ContainsKey(treeID))
            return Datas[treeID];
        return null;
    }
    public static Dictionary<string, ConfigData> Datas = new Dictionary<string, ConfigData>();
    public static void Init(GameDataTable table)
    {
        foreach (var t in table.GetTable())
        {
            var data = new ConfigData(t);
            Datas.Add(data.Id, data);
        }
    }


}
