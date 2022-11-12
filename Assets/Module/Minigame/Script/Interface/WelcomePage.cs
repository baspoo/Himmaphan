using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class WelcomePage : UIBase
    {
        public static WelcomePage instance;
        public static WelcomePage Open(   )
        {
            instance = CreatePage<WelcomePage>(GameStore.instance.page.prefab_welcomePage);
            instance.Init( );
            return instance;
        }





       


        public void Init( ) 
        {

        }
        public void OnLeaderBoard()
        {
            LeaderBoardPage.Open();
        }
        public void OnStartGame()
        {
            GameControl.instance?.LetGo();
            OnClose();
        }



    }
}