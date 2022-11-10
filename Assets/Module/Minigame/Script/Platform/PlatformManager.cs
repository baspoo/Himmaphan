using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class PlatformManager : MonoBehaviour
    {
       
        public void Init()
        {
            startFloor = Data.PlistData.plist.minigame.config.startFloor;
            mainFloor = Data.PlistData.plist.minigame.config.mainFloor;
            freeverFloor = Data.PlistData.plist.minigame.config.freeverFloor;
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
            maxPlatformRound = 
                Random.RandomRange(Data.PlistData.plist.minigame.config.maxOfRound[0], Data.PlistData.plist.minigame.config.maxOfRound[1]);

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
















        int boosterLocationCounter = 0;
        int boosterLocationCounterMax = 2;
        int boosterGenaratePercents = 45;
        public bool IsCanGenBooster() 
        {
            boosterLocationCounter++;
            if (boosterLocationCounter >= boosterLocationCounterMax) 
            {
                if (100.Random() < boosterGenaratePercents)
                {
                    boosterLocationCounter = 0;
                    return true;
                }
                else return false;
            }
            else return false;
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