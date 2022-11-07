using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArrowBar : MonoBehaviour
{

    public List<string> Contents = new List<string>();
    public enum BarStyle { Once, Loop }
    public BarStyle barStyle;
    public UILabel ui_lbname;
    public UIButton btn_left;
    public UIButton btn_right;
    public EventDelegate EventChange;
    List<string> m_disble = new List<string>();
    bool isInit = false;
    int m_index=0;
    public string Value { get { return GetContent(Index); } set { OnChange(GetContent(value)); } }
    public int Index { get { return m_index; } set { OnChange(value); } }

    public string GetContent(int Index)
    {
        if (Contents != null && Index < Contents.Count)
            return Contents[Index];
        else
            return string.Empty;
    }
    public int GetContent(string contecnt)
    {
        return Contents.FindIndex( x => x == contecnt);
    }
    public void OnLeft( )
    {
        OnChange(Index-1);
    }
    public void OnRight()
    {
        OnChange(Index+1);
    }
    void Start()
    {
        if(!isInit)
            OnChange(0,false);
    }
    public void OnChange(int index , bool isTriggerEvent = true) 
    {
        Debug.Log(index);
        if (barStyle == BarStyle.Once) 
        {
            if (index >= Contents.Count)
                return;
            if (index < 0)
                return;
            m_index = index;
        }
        if (barStyle == BarStyle.Loop)
        {
            m_index = index;
            if (index >= Contents.Count)
                m_index = 0;
            if (index < 0)
                m_index = Contents.Count-1;
        }
        ui_lbname.text = Contents[m_index];
        if (isTriggerEvent && EventChange != null)
            EventChange.Execute();
        isInit = true;
    }



}
