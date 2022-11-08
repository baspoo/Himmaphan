using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayingData
{
    public static PlayingData Inst;
    public static PlayingData Create()
    {
        Inst = new PlayingData();
        Inst.Init();
        return Inst;
    }



    public void Init() 
    {
        Usage = new UsageData();
        EventData = new Data.EventData();
        //PlistData = new Data.PlistData();
        ProductDatas = new List<Data.ProductData>();
        CurrentFloor = null;
    }

    [System.Serializable]
    public class UsageData 
    {
        public string EventID;
    }
    public UsageData Usage;
    public Data.EventData EventData;
    public List<Data.ProductData> ProductDatas;
    //public Data.PlistData PlistData;


    public Data.FloorData CurrentFloor { get; private set; }
    public void ChangeCurrentFloor(Data.FloorData floor)
    {
        CurrentFloor = floor;
    }




}
