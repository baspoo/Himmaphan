using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Center {
    public class ManagerCenter
    {
        public static RootManager.SceneName SceneName;
        public static IEnumerator Init(RootManager.SceneName sceneName)
        {
            SceneName = sceneName;
            Store.Create();
            InterfaceRoot.instance?.Init();
            yield return new WaitForEndOfFrame();

            var done = false;
            LoadCenter.instance.GetPlistData(x => { done = x; });
            while (!done) yield return new WaitForEndOfFrame();
        }




    }
}
