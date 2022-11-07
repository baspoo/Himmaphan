using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBtnToggle : MonoBehaviour
{



    public Transform checker;
    public UIButton btn;
    public EventDelegate onChange;
    public bool IsValue {
        get { return checker.gameObject.activeSelf; }
        set { checker.gameObject.SetActive(value); }
    }
    public void OnToggle() 
    {
        IsValue = !IsValue;
        if (onChange != null)
            onChange.Execute();
    }




}
