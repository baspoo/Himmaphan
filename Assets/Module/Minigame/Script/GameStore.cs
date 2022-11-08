using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class GameStore : MonoBehaviour
    {
        static GameStore m_instance;
        public static GameStore instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = FindObjectOfType<GameStore>();
                return m_instance;
            }
        }

        public ObjectData objectData;
        [System.Serializable]
        public class ObjectData
        {
            public List<CoinObj> coins;
            public List<BoosterObj> boosters;
            public List<ObstacleObj> obstacles;
            public CollectBase Find(CollectType type , string objectId) 
            {
                //Debug.Log($"Find {type} {objectId}");

                CollectBase collectBase = null;
                switch (type)
                {
                    case CollectType.Obstacle:
                        collectBase = obstacles.Find(x => x.objectId == objectId);
                        break;
                    case CollectType.Coin:
                        collectBase = coins.Find(x=>x.objectId == objectId);
                        break;
                    case CollectType.Booster:
                        collectBase = boosters.Find(x => x.objectId == objectId);
                        break;
                    default:
                        break;
                }
                return collectBase;
            }
        }
        public List<ScenePlatform> scenePlatforms;
        [System.Serializable]
        public class ScenePlatform
        {
            public string platformName;
            public List<BackgroundObj> backgrounds;
            public List<PlatformObj> platformObjs;
        }
        public Page page;
        [System.Serializable]
        public class Page
        {


        }


    }
}
