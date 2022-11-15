using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Center 
{ 
    public class RootManager : MonoBehaviour
    {



        [SerializeField] SceneName sceneName;
        public enum SceneName
        {
            Minigame, Interactive
        }
        private IEnumerator Start()
        {
            yield return StartCoroutine(Center.ManagerCenter.Init());
            yield return new WaitForEndOfFrame();
        }
        public void GotoMiniGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.Minigame.ToString());
        }
        public void GotoInteractive()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.Interactive.ToString());
        }

    }
}
