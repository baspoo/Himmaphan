using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCenter 
{
    public static IEnumerator Init(  )
    {
        LoadCenter.instance.Init();
        Sound.sound.Init();
        InterfaceRoot.instance.Init();
        yield return new WaitForEndOfFrame();

        var done = false;
        LoadCenter.instance.GetPlistData(x => { done = x; });
        while (!done) yield return new WaitForEndOfFrame();
    }

}
