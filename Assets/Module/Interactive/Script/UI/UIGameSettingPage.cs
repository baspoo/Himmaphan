
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIGameSettingPage : UIBase
{
    public static Dictionary<string, bool> settingData = new Dictionary<string, bool>();

    public static UIGameSettingPage instance;
    public static UIGameSettingPage Open(bool awaking = false)
    {
        instance = CreatePage<UIGameSettingPage>(Pages.prefab_settingPage);
        instance.panel.alpha = awaking ? 0.0f : 1.0f;
        instance.Init(awaking);
        return instance;
    }

    public UIGrid graphicGrid;
    List<UIObj> UIObjs = new List<UIObj>();
    Dictionary<string, List<UIObj>> dict = new Dictionary<string, List<UIObj>>();



    UIObj Find(string type, string name)
    {
        if (!dict.ContainsKey(type))
        {
            return null;
        }
        foreach (var uiObj in dict[type])
        {
            if (uiObj.gameObject.name.Contains(name))
                return uiObj;
        }
        return null;
    }
    bool ismute = true;
    public void Init(bool awaking)
    {
        if (awaking)
        {
            ismute = true;
            InitSetting();
            HandleAwake();
            OnClose();
        }
        else 
        {
            ismute = true;
            SceneHandle.instance.lighting.Blur(true);
            InitSetting();
            ismute = false;
        }
    }




    public void Close()
    {
        SceneHandle.instance.lighting.Blur(false);
        //if(!ismute)
            //Playlist.let.sfx_back.Play();

        SaveSetting();
        OnClose();
    }






    public void SaveSetting()
    {

    }


    //bool IsLow => SystemInfo.systemMemorySize < PlayingData.Inst.PlistData.config.systemMemorySize || SystemInfo.graphicsMemorySize < PlayingData.Inst.PlistData.config.graphicsMemorySize;
    //public void LowLevel()
    //{
    //    Debug.Log("--- LOW MEMORY!! ---");
    //}


    public void InitSetting()
    {

        //Debug.Log($"deviceModel : {SystemInfo.deviceModel}");
        //Debug.Log($"systemMemorySize : {SystemInfo.systemMemorySize}");
        //Debug.Log($"graphicsMemorySize : {SystemInfo.graphicsMemorySize}");
        //Debug.Log($"graphicsMultiThreaded : {SystemInfo.graphicsMultiThreaded}");
        //Debug.Log($"supportsAsyncGPUReadback : {SystemInfo.supportsAsyncGPUReadback}");
        //Debug.Log($"renderingThreadingMode : {SystemInfo.renderingThreadingMode}");
        //Debug.Log($"targetFrameRate : {Application.targetFrameRate}");
        //Application.lowMemory += LowLevel;

        


        if (UIObjs.Count == 0)
        {
            foreach (var g in graphicGrid.transform.GetAllNode())
            {
                var ui = g.GetComponent<UIObj>();
                if (ui != null)
                {
                    UIObjs.Add(ui);

                    if (!dict.ContainsKey(ui.Data))
                        dict.Add(ui.Data, new List<UIObj>());
                    dict[ui.Data].Add(ui);
                    ui.onSumbit = (r) => { DoActive(r, g.name); };
                    ui.onActive = () =>
                    {
                        if (ui.uiBg != null)
                            ui.uiBg.gameObject.SetActive(true);
                    };
                    ui.onRefresh = () =>
                    {
                        if (ui.uiBg != null)
                            ui.uiBg.gameObject.SetActive(false);
                    };
                }
            }
        }
        DoDetect();
    }

  




    static Dictionary<string, string> m_dictSetting = null;
    Dictionary<string, string> getDictSetting => PlayerPrefs.GetString("gamesetting","{}").DeserializeObject<Dictionary<string, string>>();
    void SetSettingValue(string key , string value)
    {
        var dict = getDictSetting;
        if (dict == null) dict = new Dictionary<string, string>();
        dict.Update(key, value);
        m_dictSetting = dict;
        PlayerPrefs.SetString("gamesetting", dict.SerializeToJson());
    }
    void HandleAwake() 
    {
        if (!PlayerPrefs.HasKey("gamesetting"))
        {
            //DoGraphicSet(!IsLow);
            return;
        }

        var dict = getDictSetting;
        if (dict != null) 
        {
            foreach (var p in dict) 
            {
                DoActive(p.Key, p.Value);
            }
        }
    }








    void DoActive(UIObj obj, string Name) 
    {
        DoActive(obj.Data , Name);
    }
    SceneLighting lighting => SceneHandle.instance.lighting;
    void DoActive(string Data, string Name)
    {
        SetSettingValue(Data, Name);
        bool IsOpen = Name.Contains("Open");


       




        if (Data == "graphic-set") 
        {
            DoGraphicSet(IsOpen);
        }
        if (Data == "fog")
        {
            RenderSettings.fog = IsOpen;
        }
        if (Data == "light")
        {
            lighting.light.enabled = IsOpen;
        }
        if (Data == "shadow")
        {
            lighting.light.shadows = IsOpen ? LightShadows.Soft : LightShadows.None;
        }
        if (Data == "sfx")
        {
            Sound.IsSfx = IsOpen;
        }
        if (Data == "bgm")
        {
            Sound.IsBgm = IsOpen;
        }
        if (Data == "graphic")
        {
            QualitySettings.SetQualityLevel(Name.ToInt(), true);
           
        }
        if (Data == "particle")
        {
            SceneHandle.instance.effectRoot.SetActive(IsOpen);
        }
        if (Data == "aa")
        {
            //SetSettingValue(Data, IsOpen ? 1 : 0);
            //QualitySettings.antiAliasing = IsOpen?1:0;
            //SceneHandle.instance.lighting.universalAdditionalCameraData.antialiasing =
            //IsOpen? UnityEngine.Rendering.Universal.AntialiasingMode.FastApproximateAntialiasing : UnityEngine.Rendering.Universal.AntialiasingMode.None;
        }

        //** profile
        var vol = SceneHandle.instance.lighting.volume.profile.components.Find(x => x.name.Replace("(Clone)", "") == Data);
        if (vol != null)
        {
            vol.active = IsOpen;
            //Debug.Log("volName : " + Data);
        }


        DoDetect();
    }

    void DoClear()
    {
        foreach (var uiObj in UIObjs)
        {
            uiObj.onRefresh.Invoke();
        }
    }

    void DoDetect()
    {
        //if(!ismute)
            //Playlist.let.sfx_click.Play();

        DoClear();
        DoSet();
        DoFog();
        DoLight();
        DoShadow();
        DoAA();
        DoSfx();
        DoBgm();
        DoPartile();
        DoCG();
        DoProfile();
    }







    void DoGraphicSet(bool high)
    {
        low = !high;
        var name = high ? "Open" : "Close";
        //DoActive(Find("fog", name), null, name);
        DoActive(Find("light", name), name);
        //DoActive(Find("shadow", name), null, name);
        DoActive(Find("aa", name), name);
        DoActive(Find("particle", name), name);
        DoActive(Find("graphic", (high ? 3 : 1).ToString()), (high ? 3 : 1).ToString());

        foreach (var c in lighting.volume.profile.components)
        {
            var btn = Find(c.name.Replace("(Clone)",""), name);
            if (btn != null) 
                DoActive(btn , name);
        }

    }

    static bool low = false;
    void DoSet()
    {
        if (!low) Find("graphic-set", "Open").onActive.Invoke();
        else Find("graphic-set", "Close").onActive.Invoke();
    }
    void DoFog()
    {
        if (RenderSettings.fog) Find("fog", "Open").onActive.Invoke();
        else Find("fog", "Close").onActive.Invoke();
    }
    void DoLight()
    {
        if (lighting.light.enabled) Find("light", "Open").onActive.Invoke();
        else Find("light", "Close").onActive.Invoke();
    }
    void DoShadow()
    {
        if (lighting.light.shadows != LightShadows.None) Find("shadow", "Open").onActive.Invoke();
        else Find("shadow", "Close").onActive.Invoke();
    }
    void DoAA()
    {
        if (SceneHandle.instance.lighting.universalAdditionalCameraData.antialiasing != UnityEngine.Rendering.Universal.AntialiasingMode.None ) Find("aa", "Open").onActive.Invoke();
        else Find("aa", "Close").onActive.Invoke();
    }
    void DoSfx()
    {
        if (!Sound.is_sfx_mute) Find("sfx", "Open").onActive.Invoke();
        else Find("sfx", "Close").onActive.Invoke();
    }
    void DoBgm()
    {

        if (!Sound.is_bgm_mute) Find("bgm", "Open").onActive.Invoke();
        else Find("bgm", "Close").onActive.Invoke();
    }
    void DoPartile()
    {
        if (SceneHandle.instance.effectRoot.gameObject.activeSelf) Find("particle", "Open").onActive.Invoke();
        else Find("particle", "Close").onActive.Invoke();
    }
    void DoCG()
    {
        var i = QualitySettings.GetQualityLevel();
        if (Find("graphic", i.ToString()) == null) return;
        Find("graphic", i.ToString()).onActive.Invoke();
    }
    void DoProfile()
    {
        foreach (var c in lighting.volume.profile.components)
        {
            var btn = Find(c.name.Replace("(Clone)",""), $"{(c.active ? "Open" : "Close")}");
            if (btn != null) btn.onActive.Invoke();
        }
    }
}
