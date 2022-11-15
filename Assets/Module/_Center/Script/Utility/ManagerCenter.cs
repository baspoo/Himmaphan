using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Center {
    public class ManagerCenter
    {
        public static IEnumerator Init()
        {
            Store.Create();
            InterfaceRoot.instance?.Init();
            yield return new WaitForEndOfFrame();

            var done = false;
            LoadCenter.instance.GetPlistData(x => { done = x; });
            while (!done) yield return new WaitForEndOfFrame();
        }




    }
}
