using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubTap : MonoBehaviour
{

    [Header("Familys")]
#if UNITY_EDITOR
    [SerializeField]
    public RuntimeBtn FindFamilys = new RuntimeBtn("FindFamily", (r) => {
        var subtap = r.Gameobject.GetComponent<UISubTap>();
        subtap.Familys.Clear();
        foreach (var tap in subtap.transform.parent.GetComponentsInChildren<UISubTap>()) {
            subtap.Familys.Add(tap);
        }
    });
#endif
    public List<UISubTap> Familys;
    public bool IsDefault;
    [Space(20)]


    [Header("UI -----------------------------------------------------------")]
    public UITexture uiTexture;
    public UITexture uiIcon;




    Style style => mEnable ? Enable : Disable;
    [Header("Status-Enable -------------------------------------------------")]
    [SerializeField] Style Enable;
    [Header("Status-Disable -------------------------------------------------")]
    [SerializeField] Style Disable;
    [System.Serializable]
    public class Style 
    {
        public Texture Icon;
        public Color IconColor;
        public Texture Texture;
        public Color TextureColor;
        public Vector2 Scale;
        public ReAwake TweenReawake;
        public Transform Transform;
        public EventDelegate OnChange;
    }
    private void Handle()
    {
        //style
        var s = style;
        if (uiIcon != null && s.Icon != null) { uiIcon.mainTexture = s.Icon; if (s.IconColor.a != 0) uiIcon.color = s.IconColor; }
        if (uiTexture != null && s.Texture != null) { uiTexture.mainTexture = s.Texture; if (s.TextureColor.a != 0) uiTexture.color = s.TextureColor; }  
        if (uiTexture != null && s.Scale != Vector2.zero)
        {
            uiTexture.width = (int)s.Scale.x;
            uiTexture.height = (int)s.Scale.y;
        }
        if (Enable.Transform != null) Enable.Transform.SetActive(mEnable);
        if (Disable.Transform != null) Disable.Transform.SetActive(!mEnable);
        if (s.TweenReawake != null) s.TweenReawake.OnReEnable();
    }

    public void OnSwitchTap( )
    {
        DoFamily();
        OnUpdateTapStatus(!IsEnable);
    }
    public void OnUpdateTapStatus (bool enable)
    {
        IsEnable = enable;
        style.OnChange?.Execute();
        Handle();
    }
    void DoFamily( )
    {
        if (Familys != null && Familys.Count > 0)
            Familys.ForEach(x =>
            {
                x.OnUpdateTapStatus(false);
            });
    }



    private void OnEnable()
    {
        mEnable = IsDefault;
        Handle( );
    }
    bool mEnable = false;
    public bool IsEnable
    {
        get 
        {
            return mEnable;
        }
        set 
        {
            mEnable = value;
            Handle( );
        }
    
    }





}
