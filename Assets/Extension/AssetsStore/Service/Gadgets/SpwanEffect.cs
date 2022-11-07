using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanEffect : MonoBehaviour
{

    public bool UI;
    public GameObject pEnable;
    public GameObject pDisable;


    void OnEnable()
    {
        if (pEnable == null)
            return;

        if (UI) InterfaceRoot.instance.OnCreatedEffect(pEnable, transform);
        else pEnable?.PoolPosition(transform,2.5f);
    }
    void OnDisable()
    {
        if (pDisable == null)
            return;

        if (UI) InterfaceRoot.instance.OnCreatedEffect(pDisable, transform);
        else pDisable?.PoolPosition(transform, 2.5f);
    }


}
