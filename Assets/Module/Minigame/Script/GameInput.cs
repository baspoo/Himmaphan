using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame 
{
    public class GameInput : MonoBehaviour
    {

        public void Init()
        {
            isInput = false;
        }
        public void StartGame()
        {
            isInput = true;
        }
        public void GameOver()
        {
            isInput = false;
        }







        bool isInput = false;
        bool autoSlide = false;
        void Update()
        {
            if (!isInput || Player.PlayerData.player == null || ConsolePage.instance == null || !ConsolePage.instance.IsVisible)
                return;



#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.A))
            {
                Player.PlayerData.player.handle.AddLifePoint(20);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Player.PlayerData.player.handle.AddDamage(20);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Player.PlayerData.player.handle.AddBooster(GameStore.instance.objectData.boosters[0].HandleAction());
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                Player.PlayerData.player.handle.AddBooster(GameStore.instance.objectData.boosters[1].HandleAction());
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Player.PlayerData.player.handle.AddBooster(GameStore.instance.objectData.boosters[2].HandleAction());
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Player.PlayerData.player.handle.AddBooster(GameStore.instance.objectData.boosters[3].HandleAction());
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                Player.PlayerData.player.handle.AddBooster(GameStore.instance.objectData.boosters[4].HandleAction());
            }
#endif



            if ( Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) ) 
            {
                Player.PlayerData.player.handle.OnJump();
            }

            if (Input.GetKey(KeyCode.DownArrow) || autoSlide)
            {
                Player.PlayerData.player.handle.OnSlide();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!GameControl.instance.isPause) SettingPage.Open(true);
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GameControl.instance?.Restart();
            }
        }

        public void OnBtnJump() 
        {
            Player.PlayerData.player.handle.OnJump();
        }
        public void OnBtnSlide()
        {
            autoSlide = true;
        }
        public void OnBtnStopSlide()
        {
            autoSlide = false;
        }
    }
}

