using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Manager : MonoBehaviour
    {
       
        IEnumerator Start()
        {
           
            yield return StartCoroutine(ManagerCenter.Init());
            yield return new WaitForEndOfFrame();

            GameControl.instance.Init();
            Complete();
        }
        void Complete() 
        {

            //** test startgame.
            GameControl.instance.StartGame();


        }
        
    }
}