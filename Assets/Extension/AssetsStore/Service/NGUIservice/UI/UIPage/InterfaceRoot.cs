using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceRoot : UIBase
{
    static InterfaceRoot m_instance;
    public static InterfaceRoot instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<InterfaceRoot>();
            return m_instance;
        }
    }


    public Camera mainCamera;
    public UIRoot uiRoot;
    public UIPanel uiPanel;
    public UIPanel uiPanelTop;
    public ScreenPoint screenPoint;
    public Camera uiCamera;
    public Transform rootpage;
    public Transform uipoint;
    public Transform pool;
    public Transform mask;
    public Transform loading;
    public Transform fade;
    public UILabel toppic;
    bool isReady;



    public BackgroundClossing backgroundClossing;
    [System.Serializable]
    public class BackgroundClossing 
    {
        public UIPanel panel;
        public Transform trans;
    }



    public void Init()
    {
        m_instance = this;
        screenPoint.cam = mainCamera;
        OpenLoading(false);
        OpenMask(false);
        OpenBackground(false);
        OnClosescreen(false);
    }

    public void OnReady()
    {

        isReady = true;
    }




    void Update()
    {
        var pos = Input.mousePosition;
        pos.z = 0;
        pos = uiCamera.ScreenToWorldPoint(pos);
        uipoint.transform.position = pos;
    }
    public Vector3 mousePosition => uipoint.position;





    public bool isMask => mask.gameObject.activeSelf;
    public  List<Transform> notClickRequire = new List<Transform>();
    public void OpenMask(bool visible , Transform root = null)
    {
        if (root != null) 
        {
            if (visible)
            {
               if(!notClickRequire.Contains(root)) 
                    notClickRequire.Add(root);
            }
            else
                notClickRequire.Remove(root);
        }
        if (!visible)
        {
            if (notClickRequire.Count != 0)
                return;
        }
        mask.gameObject.SetActive(visible);
    }
    public void OpenLoading(bool visible)
    {
        loading.SetActive(visible);
    }
    public void OpenFade( )
    {
        fade.SetActive(true);
    }






    public void OnDisplayTopic(string message) 
    {
        toppic.text = message;
        toppic.gameObject.SetActive(true);
    }


    public bool IsbackgroundClossing => backgroundClossing.trans.gameObject.activeSelf;
    public void OnClosescreen(bool visible, bool showLoadingBar = false)
    {
        backgroundClossing.panel.depth = 1000;
        //backgroundClossing.particleSystems.ForEach(x => { x.sortingOrder = 1000; });
        backgroundClossing.trans.SetActive(visible);
        ToggleLoadingProgress(showLoadingBar);
    }
    public void OpenBackground(bool visible)
    {

        //if (visible)
        //{
        //    backgroundClossing.panel.depth = 0;
            //backgroundClossing.particleSystems.ForEach(x => { x.sortingOrder = 0; });
        //    ToggleLoadingProgress(false);
        //}
        backgroundClossing.trans.SetActive(visible);
    }

    public void ToggleLoadingProgress(bool isShow)
    {
        //backgroundClossing.uiLoadingProgress.value = 0f;
        //backgroundClossing.uiLoadingBar.SetActive(isShow);
        //backgroundClossing.uiLoadingProgress.gameObject.SetActive(false);
    }

    public void SetLoadingProgress(float progress)
    {
        //backgroundClossing.uiLoadingProgress.gameObject.SetActive(true);
        //backgroundClossing.uiLoadingProgress.value = progress;
    }














    public void OnCreatedEffect(GameObject effect , Transform position)
    {
        PoolManager.CreateNewPoolGroup(effect, root.pool);
        var g = PoolManager.SpawParent(effect, transform , 3.0f).gameObject;
        g.SetActive(false);
        g.transform.position = position.position;
        RefreshTime(() => { g.SetActive(true); });
    }





}
