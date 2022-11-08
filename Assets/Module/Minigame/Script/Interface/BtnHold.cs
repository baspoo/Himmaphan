using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnHold : MonoBehaviour
{
    public EventDelegate active, deactive;
    public void Active()
    {
        Debug.Log("Active");
        active?.Execute();
    }

    public void Deactive()
    {
        Debug.Log("Deactive");
        deactive?.Execute();
    }
}
