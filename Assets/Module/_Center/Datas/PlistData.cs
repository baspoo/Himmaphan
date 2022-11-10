using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Data
{
    [System.Serializable]
    public class PlistData
    {
        public static PlistData plist;




        public string version;
        public Dictionary<string, string> languages = new Dictionary<string, string>();



        public Interactive interactive;
        [System.Serializable]
        public class Interactive
        {

        }


        public Minigame minigame;
        [System.Serializable]
        public class Minigame
        {
            public Network network;
            [System.Serializable]
            public class Network
            {
                public string firebase;
            }

            public Config config;
            [System.Serializable]
            public class Config
            {
                public int startFloor;
                public int mainFloor;
                public int freeverFloor;
                public int[] maxOfRound;
            }



            public List<Data> coins;
            public List<Data> boosters;
            [System.Serializable]
            public class Data 
            {
                public string objectId;
                public float duration;
                public double value;
            }

        }




    }

}
