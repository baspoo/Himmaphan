using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Manager : MonoBehaviour
    {
        public static bool init = false;

        IEnumerator Start()
        {
            yield return StartCoroutine(Center.ManagerCenter.Init(Center.RootManager.SceneName.Minigame));
            yield return new WaitForEndOfFrame();
            yield return GameControl.instance.Init();

            Complete();
        }
        void Complete() 
        {


            List<int> test = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (var i in test.Count.Shuffle())
                Debug.Log($"index:{i}   //   value:{test[i]}");


            if (!init) 
            {
                //** first-time.
                init = true;
                GameControl.instance.FirstTime();
            }
          
            //** startgame.
            GameControl.instance.StartGame();
        }






    }
}