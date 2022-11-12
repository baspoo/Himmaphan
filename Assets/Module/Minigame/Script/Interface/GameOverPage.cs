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
            ui_lbScore.text = Player.PlayerData.player.stat.Score.ToString("#,##0");
            ui_lbHighScore.text = Player.PlayerData.player.stat.TopScore.ToString("#,##0");
            tNewHigh.SetActive(Player.PlayerData.player.stat.Score > Player.PlayerData.player.stat.TopScore);
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