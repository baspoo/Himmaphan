using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITaps : MonoBehaviour
{


    public TapObject tapObject;
    [System.Serializable]
    public class TapObject {
        public string tapID;
        public Transform t_root;
        public UITexture ui_imgIcon;
        public UITexture ui_imgBg;
        public UILabel ui_lbName;
        public UIButton ui_btn;
        public TapMain master => m_master;
        TapMain m_master;
        public void Init(TapMain master) 
        {
            this.m_master = master;
        }
    }
    public void OnTap() {
        if (tapObject.master == null) 
        {
            Debug.LogError("Use [UITap-OnTap By TapObject] Not Use By Master.");
            return;
        }
        tapObject.master.SelectTap(tapObject);
    }


   




    private void Start()
    {
        //if (tapMain.isSimpleTap)
            //tapMain.InitTap(null);
    }
    public TapMain tapMain;
    [System.Serializable]
    public class TapMain {
        [Header("ตั้งค่าเริ่มต้น - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ")]
        //public UITaps root;
        public List<UITaps> TapObjects = new List<UITaps>();
        public string tapIDDefault;
        public bool isSimpleTap;


        [Header("ตั้งค่า Style - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ")]
        public Texture[] BgTexture = new Texture[2];
        public Color[] IconColor = new Color[2];
        public Color[] BgColor = new Color[2];
        public Color[] NameColor = new Color[2];
        public TapObject selector => m_selector;
        TapObject m_selector;
        public bool isInit => m_isInit;
        bool m_isInit = false;
        Service.Callback.callback_data m_callback;
        public void InitTap(  Service.Callback.callback_data callback )
        {
            m_callback = callback;
            foreach (var tap in TapObjects)
                tap.tapObject.Init(this);


            if (string.IsNullOrEmpty(tapIDDefault))
            {
                if (TapObjects.Count > 0)
                    m_selector = TapObjects[0].tapObject;
            }
            else 
            {
                m_selector = Find(tapIDDefault);
            }
            if (m_selector != null)
                SelectTap(m_selector);
            m_isInit = true;
        }


        public TapObject Find(string tapID)
        {
            foreach (var tap in TapObjects)
                if (tap.tapObject.tapID == tapID)
                    return tap.tapObject;
            return null;
        }
        public void SelectTap(TapObject selector)
        {
            foreach (var tap in TapObjects)
                Style(tap.tapObject,false);
            m_selector = selector;
            Style( m_selector , true );
            if (m_callback != null)
                m_callback(selector.tapID);
        }


        void Style(TapObject selector , bool isActive) 
        {
            if (selector.t_root != null) selector.t_root.gameObject.SetActive(isActive);
            if (selector.ui_btn != null) selector.ui_btn.isEnabled = !isActive;
            if (selector.ui_imgIcon != null) selector.ui_imgIcon.color =  IconColor[(isActive) ? 1 : 0 ];
            if (selector.ui_imgBg != null) 
            {
                int bgindex = (isActive) ? 1 : 0;
                if (BgTexture[bgindex] != null)
                    selector.ui_imgBg.mainTexture = BgTexture[bgindex];
                selector.ui_imgBg.color = BgColor[bgindex];
            }
            if (selector.ui_lbName != null) selector.ui_lbName.color = NameColor[(isActive) ? 1 : 0];
        }

    }





}
