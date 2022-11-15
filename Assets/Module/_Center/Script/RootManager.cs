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
            None, Minigame, Interactive
        }
        private IEnumerator Start()
        {
            yield return StartCoroutine(Center.ManagerCenter.Init());
            yield return new WaitForEndOfFrame();



            //** Web URL
            bool requestParms = false;
            string modeString = string.Empty;
            URLParameters.Instance.Request((parms) => {
                requestParms = true;
                if (parms != null && parms.SearchParams.ContainsKey("mode"))
                    modeString = parms.SearchParams["mode"];
                else
                    modeString = sceneName.ToString().ToLower();
            });
            while (!requestParms) yield return new WaitForEndOfFrame();
            if (modeString.notnull()) 
            {
                if (modeString == "minigame") GotoMiniGame();
                if (modeString == "interactive") GotoInteractive();
            }

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
