using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBubble : UIBase
{
    public static List<UIBubble> UIBubbles => instances;
    static List<UIBubble> instances = new List<UIBubble>();
    public static UIBubble Open( Texture Icon , Transform target3D , Transform page , System.Action action , string group = null )
    {
        var instance =   InterfaceRoot.instance.UIPool<UIBubble>(Pages.prefab_bubblePage, page);
        instance.Init(Icon,target3D, action);
        instances.Add(instance);
        return instance;
    }
    public static List<UIBubble> Find(string groupName)
    {
        return UIBubbles.FindAll(x=>x.group == groupName);
    }
    public static void OnClose(string groupName)
    {
        Find(groupName).ForEach(x => x.OnCloseBubble());
    }
    public static void OnClose(List<UIBubble> UIBubbles)
    {
        UIBubbles.ForEach(x=>x.OnCloseBubble());
    }


    static Texture mImgDefault;
    static Texture imgDefault 
    {
        get {
            if (mImgDefault == null)
                mImgDefault = Pages.prefab_bubblePage.GetComponent<UIBubble>().imgIcon.mainTexture;
            return mImgDefault;
        }
    }

    public bool isActive => pool != null ? pool.isActive : false;

    public UITexture imgIcon;
    public UITexture imgBg;
    public Transform popup;
    public string group;
    public Color color_stateCompleted_top;
    public Color color_stateCompleted_bottom;

    public Material mat_blink;

    PoolObj pool;
    [SerializeField]Transform target3D;
    System.Action action;
    ScreenPoint.Looking look;
    private void Init(Texture Icon ,Transform target3D, System.Action action, string group = null)
    {
        pool = GetComponent<PoolObj>();
        this.group = group;
        this.target3D = target3D;
        this.action = action;
        BubbleState_Normal();
        imgIcon.mainTexture = Icon!=null? Icon : imgDefault;
        look = root.screenPoint.AddLooking(transform, target3D);
    }
    [SerializeField] float distance;
    [SerializeField] float size;
    private void Update()
    {
        imgBg.depth = ((int)transform.localPosition.y) * -1;
        imgIcon.depth = imgBg.depth + 1;
        popup.localScale = Vector3.one * calculateDistance;
    }


    float calculateDistance
    {
        get
        {

            
            if (Player.instance != null)
            {
                var distance = Vector3.Distance(target3D.position, Player.instance.transform.position);
                //ToDo : calculate Distance for adjust size......
            }


            return 1.0f;
        }
    
    }


    public void BubbleState_Normal()
    {
        this.imgBg.gradientTop = Color.white;
        this.imgBg.gradientBottom = Color.white;
        this.imgBg.applyGradient = false;
        this.imgBg.material = null;
        this.imgBg.Update();

    }

    public void BubbleState_Completed() {
        this.imgBg.gradientTop = color_stateCompleted_top;
        this.imgBg.gradientBottom = color_stateCompleted_bottom;
        this.imgBg.applyGradient = true;
        this.imgBg.material = mat_blink;
        this.imgBg.Update();
    }

    public void OnSubmit() 
    {
        //Playlist.let.sfx_click.Play();
        action?.Invoke();
    }
    public void OnCloseBubble()
    {
        if (look != null)
            root.screenPoint.RemoveLooking(look);
        pool.Deactive();
        instances.Remove(this);
    }


}
