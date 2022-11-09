using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class GameOverPage : UIBase
    {
        public static GameOverPage instance;
        public static GameOverPage Open(   )
        {
            instance = CreatePage<GameOverPage>(GameStore.instance.page.prefab_gameoverPage);
            instance.Init( );
            return instance;
        }



        public UILabel ui_lbScore;
        public UILabel ui_lbHighScore;
        public Transform tNewHigh;

        public void Init( ) 
        {

        }
        public void ClosePage() 
        {
            OnClose();
        }

        public void OnHone()
        {
            GameControl.instance?.Home();
            OnClose();
        }
        public void OnPlayAgain()
        {
            GameControl.instance?.Restart();
            OnClose();
        }


    }
}