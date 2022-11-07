using System.Collections;
using System.Collections.Generic;
using UnityEngine;





    public class TreeData 
    {
        public enum LayoutType
        {
            None, Normal, Exclusive
        }
        public bool Enable;
        public string Id;
        public string Name;
        public string Description;
        public LayoutType Layout;

        public TreeData(GameData raw)
        {
            //Varable
            raw.GetValue("Enable", out Enable);
            raw.GetValue("Id",out Id);
            raw.GetValue("Name", out Name);
            raw.GetValue("Description", out Description);

            //Enum
            Layout = (LayoutType)raw.GetEnum("Type", LayoutType.None);
        }








        public static TreeData Find(string treeID)
        {
            if (Datas.ContainsKey(treeID))
                return Datas[treeID];
            return null;
        }
        public static Dictionary<string, TreeData> Datas = new Dictionary<string, TreeData>();
        public static void Init(GameDataTable table)
        {
            foreach (var t in table.GetTable())
            {
                var data = new TreeData(t);
                Datas.Add(data.Id, data);
            }
        }


    }
