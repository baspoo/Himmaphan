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
            if (!isInput || Player.PlayerData.player == null)
                return;




            if ( Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) ) 
            {
                Player.PlayerData.player.handle.OnJump();
            }

            if (Input.GetKey(KeyCode.DownArrow) || autoSlide)
            {
                Player.PlayerData.player.handle.OnSlide();
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

