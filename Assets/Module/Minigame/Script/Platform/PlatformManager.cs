using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class PlatformManager : MonoBehaviour
    {
       
        public void Init()
        {

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
        public int maxPlatformRound;
        public float floorLenght = 30f;
        public float farToTrash = 50f;
        public Transform root;
        int countPlatformGenarate;
        bool isContinue = false;
        GameStore.ScenePlatform m_scenePlatform;
        void DoPreParingScene(GameStore.ScenePlatform scene)
        {
            isContinue = true;
            countPlatformGenarate = 0;

            m_platformObjs.ForEach(x=>x.Clear());
            m_platformObjs = new List<PlatformObj>();
            m_scenePlatform = scene;

            startFloor.Loop(()=> { Genarate(m_scenePlatform.platformObjs[0]); });
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
            }
        }
        public void Trash(PlatformObj platform)
        {
            platform.Clear();

            if (isContinue) 
            {
                if (countPlatformGenarate >= maxPlatformRound)
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
            yield return new WaitForEndOfFrame();
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