using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class GameControl : MonoBehaviour
    {
        public static bool isFirstTime = false;
        static GameControl m_instance;
        public static GameControl instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = FindObjectOfType<GameControl>();
                return m_instance;
            }
        }

        public Player.PlayerData player;
        public CameraEngine camera;
        public PlatformManager platform;
        public BackgroundManager background;
        public GameInput input;
        public GameNetworkControl network;

        public IEnumerator Init()
        {

            player.Init();
            camera.Init();
            platform.Init();
            background.Init();
            input.Init();
            
            network.Init();
            while(!network.isNetworkReady) 
                yield return new WaitForEndOfFrame();

        }





        public void FirstTime() 
        {
            //** Merge-Plist
            var minigame = Data.PlistData.plist.minigame;

            //** Merge-Coin
            foreach (var coin in minigame.coins)
            {
                var find = GameStore.instance.objectData.coins.Find( x=>x.objectId == coin.objectId );
                if (find != null) 
                {
                    find.Score = (int)coin.value;
                }
            }

            //** Merge-Booster
            foreach (var booster in minigame.boosters)
            {
                var find = GameStore.instance.objectData.boosters.Find(x => x.objectId == booster.objectId);
                if (find != null)
                {
                    find.Data.Duration = booster.duration;
                    find.Data.Value = (int)booster.value;
                }
            }
        }





        public void StartGame()
        {
            if (!isFirstTime)
            {
                WelcomePage.Open();
            }
            else 
            {
                LetGo();
            }
            isFirstTime = true;
        }

        public void LetGo() => StartCoroutine(DoLetGo());
        IEnumerator DoLetGo() 
        {

            ConsolePage.Open();
            platform.PreParingScene();

            yield return new WaitForEndOfFrame();
            player.StartGame();
            camera.StartGame();
            platform.StartGame();
            background.StartGame();
            input.StartGame();
            network.StartGame();

        }




        public void GameOver() => StartCoroutine(DoGameOver());
        IEnumerator DoGameOver()
        {
            player.GameOver();
            camera.GameOver();
            platform.GameOver();
            background.GameOver();
            input.GameOver();
            network.GameOver();

            ConsolePage.instance?.ClosePage();
            yield return new WaitForSeconds(1.5f);
            GameOverPage.Open();
        }






        public void Home()
        {
            isFirstTime = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Minigame", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene( "Minigame" , UnityEngine.SceneManagement.LoadSceneMode.Single );
        }







        public bool isPause => Time.timeScale == 0.0f;
        public void OnPause()
        {
            Time.timeScale = 0.0f;
        }
        public void OnResume()
        {
            Time.timeScale = 1.0f;
        }

    }
}





































