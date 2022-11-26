using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Data
{
    [System.Serializable]
    public class PlistData
    {
        public static PlistData plist;













        public Config config = new Config();
        [System.Serializable]
        public class Config
        {
            public Dictionary<string, string> languages = new Dictionary<string, string>();
            public string version;
        }



        public Interactive interactive = new Interactive();
        [System.Serializable]
        public class Interactive
        {
            public Dictionary<string, string> languages = new Dictionary<string, string>();

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
            public Dictionary<string, string> languages = new Dictionary<string, string>();

            public Network network;
            [System.Serializable]
            public class Network
            {
                public string firebase;
            }

            public Tuning tuning;
            [System.Serializable]
            public class Tuning
            {
                public Genarate genarate;
                public BoosterSpawn boosterSpawn;
                public Quiz quiz;
            }
            [System.Serializable]
            public class Genarate
            {
                public int startFloor;
                public int mainFloor;
                public int freeverFloor;
                public int[] maxOfRound;
            }
            [System.Serializable]
            public class BoosterSpawn
            {
                public int genaratePercent = 45;
                public int counter = 2;
            }
            [System.Serializable]
            public class Quiz
            {
                public int bonusPoint = 20;
            }















            public List<Data> coins;
            public List<Data> boosters;
            [System.Serializable]
            public class Data 
            {
                public string objectId;
                public float duration;
                public double value;
                public double percent;
            }



            public List<Question> questions;
            [System.Serializable]
            public class Question
            {
                public string id;
                public string quest;
                public string[] choices;
                public int answer;
            }

        }




    }

}
