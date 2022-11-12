using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObj : BasePool
{

    public bool IsActive;
    public int Index;
    public string Data;
    public object Raw;
    public float Value;
    public UILabel uiName;
    public UILabel uiDescription;
    public UILabel uiAmount;
    public UITexture uiIcon;
    public UITexture uiBg;
    public UITexture uiTop;
    public UIButton btn;
    public Color[] color;
    public System.Action<UIObj> onSumbit;
    public System.Action onActive;
    public System.Action onRefresh;
    public void Init()
    {
        
    }
    public void OnSubmit() {
        onSumbit?.Invoke(this);
    }
    public void OnCopyLabel(UILabel label)
    {
        if (label != null) 
        {
            HtmlCallback.Copy(label.text);
        }
    }
}
public static class UIObjFunction {
    public static UIObj BtnsDisableActive(this List<UIObj> objs , UIObj target ) 
    {
        objs.ForEach(x => x.btn.isEnabled = true);
        var btn = objs.Find(x => x == target);
        if (btn != null) btn.btn.isEnabled = false;
        return btn;
    }
}



