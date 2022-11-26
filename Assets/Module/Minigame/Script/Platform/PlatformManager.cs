using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace MiniGame
{
    public class PlatformManager : MonoBehaviour
    {
       
        public void Init()
        {
            //** floor
            startFloor = Data.PlistData.plist.minigame.tuning.genarate.startFloor;
            mainFloor = Data.PlistData.plist.minigame.tuning.genarate.mainFloor;
            freeverFloor = Data.PlistData.plist.minigame.tuning.genarate.freeverFloor;
            maxOfRound = Data.PlistData.plist.minigame.tuning.genarate.maxOfRound;

            //** genarate
            boosterSpawn = Data.PlistData.plist.minigame.tuning.boosterSpawn;

            //** booster
            InitBooster();
        }
        public void PreParingScene()
        {
            DoPreParingScene(GameStore.instance.scenePlatforms[0]);
        }
        public void StartGame()
        {
            DoStart();
        }
        public void GameOver()
        {
            DoStop();
        }




























        public int startFloor;
        public int mainFloor;
        public int freeverFloor;
        public int maxPlatformRound;
        public float floorLenght = 30f;
        public float farToTrash = 50f;
        public Transform root;
        int[] maxOfRound = new int[2] { 1 , 3 };

        int countPlatformGenarate;
        int countPlatformComplete;
        bool isContinue = false;
        bool isFirst = true;
        GameStore.ScenePlatform m_scenePlatform;
        void DoPreParingScene(GameStore.ScenePlatform scene)
        {
            isContinue = true;
            countPlatformGenarate = 0;
            countPlatformComplete = 0;

            //** Random Max Of Round
            maxPlatformRound = maxOfRound.Random();

            //** Snap Player to Origin
            Player.PlayerData.player.transform.position = GameControl.instance.background.origin.transform.position;

            m_platformObjs.ForEach(x=>x.Clear());
            m_platformObjs = new List<PlatformObj>();
            m_scenePlatform = scene;


            if (isFirst)
            {
                isFirst = false;
                startFloor.Loop(() => { Genarate(m_scenePlatform.platformObjs[0]); });
            }
            else 
            {
                Genarate(m_scenePlatform.platformObjs[0]);
                freeverFloor.Loop(() => { Genarate(m_scenePlatform.platformFreeverObjs[m_scenePlatform.platformFreeverObjs.Count.Random()]); });
            }
            mainFloor.Loop(() => { Genarate( ); });
        }


        List<PlatformObj> m_platformObjs = new List<PlatformObj>();
        void Genarate(PlatformObj platform = null) 
        {
            if (platform == null)
                platform = m_scenePlatform.platformObjs[Random.Range(1, m_scenePlatform.platformObjs.Count)];

            if (platform != null) 
            {
                var p = platform.gameObject.Pool(root).GetComponent<PlatformObj>();
                p.Init(this);
                p.transform.localPosition = new Vector3( floorLenght * countPlatformGenarate , 0.0f,0.0f);
                m_platformObjs.Add(p);
                countPlatformGenarate++;
                GameControl.instance.background.Push(p,m_scenePlatform);
            }
           
        }
        public void Trash(PlatformObj platform)
        {
            platform.Clear();
            countPlatformComplete++;


            if (isContinue) 
            {
                if (countPlatformComplete >= maxPlatformRound)
                {
                    ReScenePlatform();
                }
                else
                {
                    Genarate();
                }
            }
        }

        void ReScenePlatform( ) 
        {
            isContinue = false;
            Debug.Log("ReScenePlatform !!!");
            StartCoroutine(DoReScenePlatform());
        }
        IEnumerator DoReScenePlatform() 
        {

           
            var turbo = GameStore.instance.objectData.boosters.Find(x=>x.Data.BoosterType== BoosterType.Turbo);
            Player.PlayerData.player.handle.AddBooster(turbo.HandleAction());


            InterfaceRoot.instance.OnDisplayTopic("FREEVER!!");
            ConsolePage.instance?.OnVisible(false);
            yield return new WaitForSeconds(2.5f);
            yield return new WaitForEndOfFrame();

            InterfaceRoot.instance.OpenFade();
            yield return new WaitForEndOfFrame();
            DoPreParingScene(GameStore.instance.scenePlatforms[0]);
            yield return new WaitForEndOfFrame();


            ConsolePage.instance?.OnVisible(true);
        }















        //** BOOSTER
        [SerializeField] List<BoosterObj> boosters;
        List<double> randomBooster;
        int boosterLocationCounter = 0;
        Data.PlistData.Minigame.BoosterSpawn boosterSpawn = new Data.PlistData.Minigame.BoosterSpawn() { 
            genaratePercent = 45,
            counter = 2
        };
        void InitBooster()
        {
            boosters = new List<BoosterObj>();
            boosters.AddRange(GameStore.instance.objectData.boosters);
            randomBooster = boosters.Select(x => x.Data.Percent).ToList();
        }
        public bool IsCanGenBooster() 
        {
            boosterLocationCounter++;
            if (boosterLocationCounter >= boosterSpawn.counter) 
            {
                if (100.Random() < boosterSpawn.genaratePercent)
                {
                    boosterLocationCounter = 0;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public BoosterObj RandomBooster() {

            return boosters[CakeRandom.ChooseRandomIndex(randomBooster)];
        }
        public void RemoveBooster( BoosterType boosterType)
        {
            boosters.RemoveAll(x=>x.Data.BoosterType == boosterType);
            randomBooster = boosters.Select(x => x.Data.Percent).ToList();
        }














        bool isStartting = false;
        void DoStart() 
        {
            isStartting = true;
        }
        void DoStop()
        {
            isStartting = false;
        }
        private void Update()
        {
            if (isStartting) 
            {
            
            
            }
        }






    }
}