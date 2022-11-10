using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame 
{
    public class BackgroundManager : MonoBehaviour
    {
        public void Init()
        {
            isReady = false;
            ActiveRoadOfRainBow(false);
            gameObject.SetActive(false);
        }
        public void StartGame()
        {
            isReady = true;
            gameObject.SetActive(true);
        }
        public void GameOver()
        {
            isReady = false;
        }








        bool isReady = false;
        public Transform origin;
        public Transform rainbow;
        public Layer[] layers;
        [System.Serializable]
        public class Layer 
        {
            public Transform root;
            public double percent;
            public int depth;
            public float distanceToDeactve;
            public int[] amountPerRound;
            public float[] range = new float[2] { 0 , 30 };
            public List<BackgroundObj.BackgroundType> types;


            GameStore.ScenePlatform m_scenePlatform;
            List<BackgroundObj> m_let;
            public List<BackgroundObj> GetBackgroundList(GameStore.ScenePlatform scenePlatform) 
            {
                if (m_scenePlatform == null)
                {
                    m_let = null;
                }
                else if(m_scenePlatform != scenePlatform)
                {
                    m_let = null;
                }

                if (m_let == null) 
                {
                    m_scenePlatform = scenePlatform;
                    m_let = new List<BackgroundObj>();
                    foreach (var perfab in m_scenePlatform.backgrounds)
                    {
                        if (types.Contains(perfab.Type))
                        {
                            m_let.Add(perfab);
                        }
                    }
                }
                return m_let;
            }
        }
        List<BackgroundObj> m_backgrounds = new List<BackgroundObj>();
        public void Push(PlatformObj platform , GameStore.ScenePlatform scenePlatform ) 
        {
            foreach (var layer in layers) 
            {
                Random.RandomRange(layer.amountPerRound[0], layer.amountPerRound[1]).Loop(() => {

                    if (100.Random() < layer.percent) 
                    {
                        var let = layer.GetBackgroundList(scenePlatform);
                        var rand = let[let.Count.Random()];
                        var bg = rand.gameObject.Pool(transform).GetComponent<BackgroundObj>();
                        bg.Init(platform, layer);
                        m_backgrounds.Add(bg);
                    }
                });
            }
        }


        public void ActiveRoadOfRainBow(bool active) 
        {
            rainbow.SetActive(active);
        }



        Service.Timmer.Update timeOut = new Service.Timmer.Update(1,1);
        private void Update()
        {
            if (isReady) 
            {
                timeOut.OnUpdate(()=> {
                    foreach (var background in m_backgrounds) 
                    {
                        var dist = Player.PlayerData.player.transform.position.x - background.transform.position.x;
                        if (dist > background.layer.distanceToDeactve) 
                        {
                            background.Destroy();
                        }
                    }
                    m_backgrounds.RemoveAll(x=>!x.isActive);
                });
            }
        }



    }

}

