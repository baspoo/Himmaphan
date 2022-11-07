using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDropDown : MonoBehaviour
{
    public enum Pivot { Down , Up }
    public Pivot pivot;
    public List<string> Contents;
   
    public List<Style> Styles;
    [System.Serializable]
    public class Style {
        public bool isEnable;
        public Color defaultColor;
        public Color hover;
        public Color pressed;
        public Color disabledColor;
    }
    public Transform root;
    public UILabel ui_lbname;
    public UIButton btn;
    public UITexture ui_imgbar;
    public UITexture ui_imgbg;
    GameObject popup;
    public EventDelegate EventChange;
    List<string> m_disble = new List<string>();

    public string value { get { return m_value; } }
    string m_value;
    public int Index { get { return Contents.FindIndex(x => x == m_value); } }
    public string GetContentIndex(int Index)
    {
        if (Contents != null && Index < Contents.Count)
            return Contents[Index];
        else
            return string.Empty;
    }

    int fixDefaultIndex = 0;
    public void FixDefaultIndex(int index)
    {
        fixDefaultIndex = index;

    }
    int defaultIndex { get { return fixDefaultIndex > 0 && fixDefaultIndex < Contents.Count ? fixDefaultIndex : 0; } }

    void Start()
    {
        OnChange(defaultIndex);
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) 
        {
           if(isActive)
                StartCoroutine(IEClose());
        }
    }






    public void OnAddDisble(int index)
    {
        var content = GetContentIndex(index);
        OnAddDisble(content);
    }
    public void OnAddDisble(string content )
    {
        if(!string.IsNullOrEmpty(content))
            m_disble.Add(content);
    }
    public void OnClearDisble( )
    {
        m_disble.Clear();
    }






    public void OnChange( int index  , bool isTriggerEvent = false) 
    {
        m_value = Contents[index];
        ui_lbname.text = m_value;//language(m_value);
        AdapToMainBtn(index);
        if (isTriggerEvent)
            if (EventChange != null)
                EventChange.Execute();


    }
    public void OnChange(string content , bool isTriggerEvent = false)
    {
        int index = Contents.FindIndex(x => x == content);
        if (index != -1)
            OnChange( index , isTriggerEvent );
    }
    public void Open( ) 
    {
        if(popup != null)
            Destroy(popup.gameObject);

        float fector = -1.0f;
        var uipivot = UIWidget.Pivot.Top;
        var bgpivot = UIWidget.Pivot.Top;
        if (pivot == Pivot.Up) 
        {
            fector = 1.0f;
            uipivot = UIWidget.Pivot.Bottom;
            bgpivot = UIWidget.Pivot.Bottom;
        }



        popup = new GameObject("popup");
        popup.transform.parent = root;
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localScale = Vector3.one;

        UIGrid grid = (new GameObject("grid")).AddComponent<UIGrid>();
        grid.transform.parent = popup.transform;
        grid.transform.localPosition = Vector3.zero;
        grid.transform.localScale = Vector3.one;
        grid.pivot = uipivot;
        grid.cellHeight = ui_imgbar.height + 5.0f;
        grid.arrangement = UIGrid.Arrangement.Vertical;
        int index = 0;
        foreach (var c in Contents) 
        {
            GenBar( c , index , grid.transform);
            index++;
        }
        grid.repositionNow = true;
        btn.isEnabled = false;



        UITexture BG = Instantiate( ui_imgbg, popup.transform);
        Service.GameObj.DesAllParent(BG.transform);
        BG.pivot = bgpivot;
        BG.depth += 1;
        BG.width += 20;
        BG.height = (int) ((grid.cellHeight * Contents.Count) + 20);
        BG.transform.localPosition = new Vector3(0.0f, ((grid.cellHeight + 20) / 2.0f ) * (-1.0f * fector), 0.0f);
        //BG.tr

        UIWidget widget = popup.gameObject.AddComponent<UIWidget>();

        TweenAlpha alpha = popup.gameObject.AddComponent<TweenAlpha>();
        alpha.from = 0.0f;
        alpha.to = 1.0f;
        alpha.duration = 0.2f;

        TweenScale tween = popup.gameObject.AddComponent<TweenScale>();
        tween.from = new Vector3(1.0f,0.0f,1.0f);
        tween.to = new Vector3(1.0f, 1.0f, 1.0f);
        tween.duration = 0.15f;

        TweenPosition position = popup.gameObject.AddComponent<TweenPosition>();
        position.from = new Vector3(0.0f, 0.0f, 0.0f);
        position.to = new Vector3(0.0f, ui_imgbar.height * fector , 0.0f);
        position.duration = 0.15f;
        StartCoroutine(IEOpen());
        

        if (m_opened != null)
            m_opened();
    }
    IEnumerator IEOpen()
    {
        //isActive = true;
        yield return new WaitForSeconds(0.15f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        isActive = true;
    }
    Service.Callback.callback m_opened;
    public void Opened(Service.Callback.callback opened) 
    {
        m_opened = opened;
    }

    public List<ContentObj> ContentObjs => contentObjs;
    List<ContentObj> contentObjs = new List<ContentObj>();
    public class ContentObj
    {
        public string ID;
        public int Index;
        public UITexture Bar;
        public UILabel Text;
        public UIButton Btn;
        public UIDropDown master;
        public void Init()
        {
            //Debug.Log(this.Index);
            var style = master.GetStyle(this.Index);
            if (style != null) 
            {
                ChangeBtnColor(style.defaultColor, style.hover, style.pressed, style.disabledColor);
            }
        }
        public void ChangeBtnColor(Color normal , Color hover , Color perssed , Color disable ) {
            Btn.defaultColor = normal;
            Btn.hover = hover;
            Btn.pressed = perssed;
            Btn.disabledColor = disable;
        }
    }



    Style GetStyle(int index) 
    {
        if (Styles != null)
        {
            if (index < Styles.Count)
            {
                var style = Styles[index];
                if (style.isEnable)
                    return style;
                else
                    return null;
            }
        }
        return null;
    }

    void AdapToMainBtn( int index )
    {
        var style = GetStyle(index);
        if (style != null)
        {
            btn.defaultColor = style.defaultColor;
            btn.hover = style.hover;
            btn.pressed = style.pressed;
            btn.disabledColor = style.disabledColor;
        }
    }
    void GenBar( string content , int index ,Transform parent)
    {
        UITexture Bar = Instantiate( ui_imgbar , parent ); 
        Service.GameObj.DesAllParent(Bar.transform);
        Bar.depth+=2;
        //Bar.gameObject.AddComponent<BoxCollider>();
        //Bar.autoResizeBoxCollider = true;


        UILabel Text = Instantiate( ui_lbname , Bar.transform); 
        Service.GameObj.DesAllParent(Text.transform);
        Text.depth+=2;
        Text.text = content; //language(content);
        Text.transform.parent = Bar.transform;
        Text.transform.localPosition = Vector3.zero;


        
        UIButton Btn = Bar.GetComponent<UIButton>();
        Destroy(Btn.GetComponent<UIDropDown>());
        Bar.color = Btn.defaultColor;
        Btn.ResetDefaultColor();
        Btn.onClick.Clear();
        Btn.onClick.Add(new EventDelegate(()=> {
            Debug.Log(content);
            OnChange(content,true);
            //Onclose();
            StartCoroutine(IEClose(0.05f));
        })) ;

        Btn.isEnabled = !m_disble.Contains(content);



        ContentObj contentObj = new ContentObj();
        contentObj.ID       = content;
        contentObj.Index    = index;
        contentObj.Bar      = Bar;
        contentObj.Text     = Text;
        contentObj.Btn      = Btn;
        contentObj.master   = this;
        contentObj.Init();
        contentObjs.Add(contentObj);
    }

    public bool IsActive =>isActive;
    bool isActive = false;
    public void Onclose() 
    {
        isActive = false;
        btn.isEnabled = true;
        popup.gameObject.SetActive(false);
        contentObjs.Clear();
    }
    IEnumerator IEClose(float delay = 0.15f) 
    {
        yield return new WaitForSeconds(delay);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Onclose();
    }




}
