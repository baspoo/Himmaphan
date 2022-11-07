//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class Rarity{
//    public const string UltraRare = "Ultra Rare";
//    public const string Rare = "Rare";
//    public const string Uncommond = "Uncommond";
//    public const string Commond = "Commond";
//}
//[System.Serializable]
//public class Feeling
//{
//    public enum FeelingType
//    {
//        Super, Happy, Normal, Bad
//    }
//    public string Name;
//    public FeelingType Type;
//    public Sprite Icon;
//}
//public class PetData
//{

    
//    public bool Enable;
//    public string ID;
//    public string Name;
//    public string Description;
//    public string Air;
//    public string Kind;
//    public string Rarity;
//    public int vBundle;
//    public int[] LvUnlocks;
//    public Dictionary<string, object> Meta = new Dictionary<string, object>();
//    public string ContractAddress;
//    public string TokenId;

//    public PetData(GameData raw)
//    {
//        Enable = System.Convert.ToBoolean(raw.GetValue("Enable"));
//        ID = raw.GetValue("ID");
//        Name = raw.GetValue("Name");
//        Description = raw.GetValue("Description");
//        Kind = raw.GetValue("Kind");
//        Air = raw.GetValue("Air");
//        Rarity = raw.GetValue("Rarity");
//        vBundle = raw.GetValue("vBundle").ToInt();
//        ContractAddress = raw.GetValue("ContractAddress");
//        TokenId = raw.GetValue("TokenId");
//        Meta = raw.GetValue("Meta").DeserializeObject<Dictionary<string, object>>();
//        LvUnlocks = raw.GetValue("LvUnlocks").DeserializeObject<int[]>();
//    }


//    public static PetData Find(string petID) => PetDatas.Find(x => x.ID == petID);

//    public static List<PetData> PetDatas = new List<PetData>();
//    public static void Init()
//    {
//        var text = "";
//        var table = GameDataTable.ReadData(text);
//        foreach (var d in table.GetTable())
//        {
//            PetDatas.Add(new PetData(d));
//        }
//    }


//}
