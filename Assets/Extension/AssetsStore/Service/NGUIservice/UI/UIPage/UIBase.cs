using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class UIBase : MonoBehaviour
{
#if UNITY_EDITOR
    public RuntimeBtn FindPanel = new RuntimeBtn("Find", (r) => {

        UIBase m_base = ((GameObject)Selection.activeObject).GetComponent<UIBase>();
        if (m_base)
        {
            m_base.settingpage.UIPanels.Clear();

            m_base.panel = m_base.GetComponent<UIPanel>();
            if(m_base.panel!=null)
                m_base.settingpage.UIPanels.Add(m_base.panel);
            foreach (var g in m_base.gameObject.transform.GetAllNode())
            {
                var panel = g.GetComponent<UIPanel>();
                if (panel != null)
                    m_base.settingpage.UIPanels.Add(panel);
            }

            //RootUIBubble
            //if (m_base.settingpage.RootUIBubble == null)
                //m_base.settingpage.RootUIBubble = m_base.transform;

            m_base.FindPanel.BtnName = $"Find [{m_base.settingpage.UIPanels.Count}]";
        }
        else Debug.LogError("Not Found UIBase!");

    });
#endif
    [System.Serializable]
    public class UISettingPage 
    {
        //public Transform RootUIBubble;
        public PageLayer pageLayer;
        public List<UIPanel> UIPanels = new List<UIPanel>();
        public enum PageLayer
        {
            None , Background , Console , Mask , Page , Popup , Notif , System
        }
        public enum Sfx
        {
            none, page , movepage
        }

        public bool DontDestoryOnClose;


        [Header("On Open")]
        public ActionPage Open;
        [Header("On Close")]
        public ActionPage Close;

        [System.Serializable]
        public class ActionPage
        {
            public bool IsMask;
            public bool IsAnimation;
            public AnimationCurve Curve;
            public float duration = 0.15f;
        }


        public TaskService.Function EventOnClose = new TaskService.Function();
    }



    //protected static Store.Pages Pages => Store.instance.page;
    bool IsActive = false;
    protected bool IsMute { get; private set; }

    static Dictionary<GameObject, GameObject> m_stock = new Dictionary<GameObject, GameObject>();
    public static T CreatePage<T>(GameObject page) 
    {
        GameObject Create() 
        {
            var np = page.Create(InterfaceRoot.instance.rootpage);
            return np;
        } 
        GameObject newpage = null;
        bool resue = false;
        if (!m_stock.ContainsKey(page))
            newpage = Create();
        else
        {
            newpage = m_stock[page];
            if (newpage != null) 
            {
                resue = true;
            }
            else
            {
                m_stock.Remove(page);
                newpage = Create();
            }
        }

        //UIBase
        var uibase = newpage.GetComponent<UIBase>();
        if(resue) uibase.ReuseHandle();
        else uibase.HandleOpen();

        uibase.name = $"{page.name} [depth - { ((uibase.panel!=null)? uibase.panel.depth : 0) }]";
        if (uibase.settingpage.Open.IsMask)
            uibase.root.OpenMask(true, uibase.transform);

        if (!m_stock.ContainsKey(page) && uibase.settingpage.DontDestoryOnClose)
            m_stock.Add(page, newpage);

        uibase.DoPanel();
        uibase.IsActive = true;

 

        return newpage.GetComponent<T>();
    }





    Dictionary<UISettingPage.PageLayer, int> pagePanelDepth = new Dictionary<UISettingPage.PageLayer, int> {
        { UISettingPage.PageLayer.None, 0 },
         { UISettingPage.PageLayer.Background, 50 },
          { UISettingPage.PageLayer.Console, 100 },
           { UISettingPage.PageLayer.Mask, 150 },
            { UISettingPage.PageLayer.Page, 200 },
             { UISettingPage.PageLayer.Popup, 250 },
              { UISettingPage.PageLayer.Notif, 300 },
              { UISettingPage.PageLayer.System, 350 }
    };

    //[HideInInspector]
    bool AdjustPanels = false;
    void DoPanel() {
        if (AdjustPanels)
            return;

        //Debug.LogError($"DoPanel [{gameObject.name}] : " + settingpage.pageLayer);
        AdjustPanels = true;
        foreach (var panel in settingpage.UIPanels) 
        {
            //Debug.LogError($"Depth [{panel.depth}] + [{ pagePanelDepth[settingpage.pageLayer]}]");
            panel.depth += pagePanelDepth[settingpage.pageLayer];
        }
    }

     

   
    public UISettingPage settingpage;
    public UIPanel panel;
    public void OnVisible(bool visible)
    {
        if(panel!=null) panel.alpha = visible ? 1.0f : 0.0f;
    }
    public bool IsVisible => panel.alpha != 0.0f;
    public InterfaceRoot root => InterfaceRoot.instance;
    public TaskService.Function EventOnClose => settingpage.EventOnClose;



    Coroutine sfxhandle;
    void HandleOpen()
    {
        IsMute = true;
        if (sfxhandle != null) StopCoroutine(sfxhandle);
        sfxhandle = RefreshTime(30, (i) => { IsMute = false; });

        if (settingpage.Open.IsAnimation)
        {
            if (tweenScale == null)
                tweenScale = gameObject.AddComponent<TweenScale>();
            tweenScale.from = Vector3.zero;
            tweenScale.to = Vector3.one;
            tweenScale.duration = settingpage.Open.duration;
            tweenScale.animationCurve = settingpage.Open.Curve;
            ReAwake.ReTween(tweenScale);
        }
    }



    Coroutine coroscale;
    TweenScale tweenScale;
    void ReuseHandle() 
    {
        if (coroscale != null) coroscale.StopCorotine();

        IEnumerator Do()
        {
            gameObject.SetActive(false);
            yield return new WaitForEndOfFrame( );
            gameObject.transform.ResetTransform();
            gameObject.SetActive(true);
            HandleOpen();
        }
        Do().StartCorotine();
    }
    protected void OnClose()
    {
        HandleClose();
        IEnumerator Do()
        {
            if (settingpage.Close.IsAnimation)
            {
                if (tweenScale == null)
                    tweenScale = gameObject.AddComponent<TweenScale>();
                tweenScale.from = gameObject.transform.localScale;
                tweenScale.to = Vector3.zero;
                tweenScale.duration = settingpage.Close.duration;
                tweenScale.animationCurve = settingpage.Close.Curve;
                ReAwake.ReTween(tweenScale);
                yield return new WaitForSeconds(settingpage.Close.duration);
            }

            if (settingpage.DontDestoryOnClose)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
        coroscale = StartCoroutine(Do());
        IsActive = false;
    }


    void HandleClose() {

        if (!IsActive) return;

        //UIBubble.OnClose(gameObject.name);

        if (settingpage.Close.IsMask)
            root.OpenMask(false, transform);
        //HandleSound(settingpage.CloseSfx);
        EventOnClose?.callall();
    }

    public bool IsPageHide => !gameObject.activeSelf;

    public void OnHide(bool Hide)
    {
        if (!IsActive) return;

        gameObject.SetActive(!Hide);

        if (!Hide && settingpage.Open.IsMask)
            root.OpenMask(true, transform);
        if (Hide && settingpage.Close.IsMask)
            root.OpenMask(false, transform);
    }

    public T UIPool<T>(GameObject obj, Transform trans , float wait = 0.0f )
    {
        return UIPool(obj, trans, wait).GetComponent<T>();
    }
    public GameObject UIPool(GameObject obj, Transform trans, float wait = 0.0f)
    {
        PoolManager.CreateNewPoolGroup(obj, root.pool);
        var g = PoolManager.SpawParent(obj, trans, wait).gameObject;
        g.SetActive(false);
        RefreshTime(() => { g.SetActive(true); });
        return g;
    }
    public GameObject UIPoolPosition(GameObject obj, Transform trans, float wait = 0.0f)
    {
        PoolManager.CreateNewPoolGroup(obj, root.pool);
        var g = PoolManager.SpawParent(obj, trans, wait).gameObject;
        g.transform.parent = root.pool;
        g.SetActive(false);
        RefreshTime(() => { g.SetActive(true); });
        return g;
    }


    public Coroutine RefreshTime(System.Action actionToRefresh) =>  IERefresh(1, (i) => { actionToRefresh?.Invoke(); }).StartCorotine();
    public Coroutine RefreshTime(int time, System.Action<int> actionToRefresh) => StartCoroutine(IERefresh(time, actionToRefresh));
    IEnumerator IERefresh(int time, System.Action<int> actionToRefresh) {
        for (int i = 0; i < time; i++) 
        {
            yield return new WaitForEndOfFrame();
            actionToRefresh?.Invoke(i);
        }
    }

    public void SnapAtPanel(Transform target) {
        RefreshTime(3, (i) => {
            var mBounds = NGUIMath.CalculateRelativeWidgetBounds(panel.cachedTransform, target);
            panel.ConstrainTargetToBounds(target, ref mBounds, true);
        });
    }


    

   





}














