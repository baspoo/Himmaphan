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
            //--> Distance
            public PlayerDistance playerDistance = new PlayerDistance();
            [System.Serializable]
            public class PlayerDistance
            {
                public float distanceOfFar = 50;
                public float distanceActive = 100;
                public float distanceObject = 50;
                public float distanceInteractive = 5;
                public float distanceFollow = 20;
                public float distanceNear = 2.5f;
            }
            public Input input = new Input();
            [System.Serializable]
            public class Input
            {
                public DeviceData web = new DeviceData();
                public DeviceData mobile = new DeviceData();
                [System.Serializable]
                public class DeviceData
                {
                    public float spinSensitivity = 0.1f;
                    public float zoomSensitivity = 1.0f;
                }
            }
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
