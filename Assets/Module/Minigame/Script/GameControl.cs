using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class GameControl : MonoBehaviour
    {

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

        public void Init()
        {
            player.Init();
            camera.Init();
            platform.Init();
            background.Init();
            input.Init();
        }
        public void StartGame()
        {
            StartCoroutine(DoStartGame());
        }
        IEnumerator DoStartGame() 
        {


            platform.PreParingScene();

            yield return new WaitForEndOfFrame();
            player.StartGame();
            camera.StartGame();
            platform.StartGame();
            background.StartGame();
            input.StartGame();
        }
        public void GameOver()
        {
            player.GameOver();
            camera.GameOver();
            platform.GameOver();
            background.GameOver();
            input.GameOver();
        }
    }
}
