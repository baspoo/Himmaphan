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
            public CollectBase FindRandom( CollectType type )
            {
                CollectBase collectBase = null;
                switch (type)
                {
                    case CollectType.Obstacle:
                        collectBase = obstacles[obstacles.Count.Random()];
                        break;
                    case CollectType.Coin:
                        collectBase = coins[coins.Count.Random()];
                        break;
                    case CollectType.Booster:
                        collectBase = boosters[boosters.Count.Random()];
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
            public List<PlatformObj> platformFreeverObjs;
        }
        public Page page;
        [System.Serializable]
        public class Page
        {
            public GameObject prefab_welcomePage;
            public GameObject prefab_registerPage;
            public GameObject prefab_consolePage;
            public GameObject prefab_settingPage;
            public GameObject prefab_gameoverPage;
            public GameObject prefab_leaderboardPage;
            public GameObject prefab_profilePage;
        }


    }
}
