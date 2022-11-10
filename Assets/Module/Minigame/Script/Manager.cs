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
            yield return StartCoroutine(ManagerCenter.Init());
            yield return new WaitForEndOfFrame();
            yield return GameControl.instance.Init();

            Complete();
        }
        void Complete() 
        {
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