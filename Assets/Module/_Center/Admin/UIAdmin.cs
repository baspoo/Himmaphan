using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Collections.Generic;
using System.Text;
using System.Linq;
public class UIAdmin : UIBase
{




    void list()
    {
        cmds = new List<cmd>();
        //Log
        cmds.Add(new cmd()
        {
            key = "log",
            name = "open/close logs",
            icon = null,
            exe = (val) => {
                Reporter.Open(!Reporter.IsVisible);
                OnClose();
            }
        });
        //RuntimeHierarchy
        cmds.Add(new cmd()
        {
            key = "runtimeHierarchy",
            name = "Open RuntimeHierarchy",
            icon = null,
            exe = (val) => {
                var RuntimeHierarchy = GameObject.Find("RuntimeHierarchy");
                if (RuntimeHierarchy != null)
                {
                    RuntimeHierarchy.transform.GetChild(0).SetActive(true);
                }
                OnClose();
            }
        });
        //Debug
        cmds.Add(new cmd()
        {
            key = "debug",
            name = "debug",
            icon = null,
            exe = (val) => {
                UIDebuging.Open();
                OnClose();
            }
        });
        //Setting
        cmds.Add(new cmd()
        {
            key = "setting",
            name = "setting",
            icon = null,
            exe = (val) => {
                UIGameSettingPage.Open();
                OnClose();
            }
        });
        //Items
        cmds.Add(new cmd()
        {
            key = "addcostume",
            name = "Add Costume",
            icon = null,
            exe = (val) => {
                List<cmd> let = new List<cmd>();
                foreach (var item in ConfigData.Datas.Values)
                {
                    let.Add(new cmd()
                    {
                        key = item.Id,
                        icon = null,
                        name = item.Name,
                        exe = (val) => {
                            Debug.Log("ToDo Action");
                            //OnClose();
                        }
                    });
                }
                View(let);
            }
        });

        //Items
        cmds.Add(new cmd()
        {
            key = "api",
            name = "view history api.",
            icon = null,
            exe = (val) => {
                List<cmd> let = new List<cmd>();
                foreach (var item in LoadCenter.instance.ApiHistoryDatas.OrderByDescending(x=>x.date))
                {
                    let.Add(new cmd()
                    {
                        key = item.url,
                        icon = item.image,
                        name = $"{item.date}",
                        message = $"   {item.result}",
                        messageColor = item.complete? Color.white : Color.red,
                    });
                }
                View(let);
            }
        });

    }





























    static UIAdmin instance;
    public static UIAdmin Open()
    {
        if (instance) instance.OnClose();
        if (!instance)
        {
            instance = CreatePage<UIAdmin>(Pages.prefab_admin);
            instance.Init();
        }
        return instance;
    }
    public UILabel debug;
    public UIInput input;
    public UIScrollView sc;
    public UIGrid grid;
    public GameObject pObj;
    public GameObject pObjApi;
    public void Init()
    {
        OnClear();
        list();
    }

    public void OnEnter(string key) 
    {
        input.value = key;
        OnEnter();
    }
    public void OnEnter()
    {
        var keys = input.value;
        input.value = string.Empty;
        string key = string.Empty, value = string.Empty;
        if (keys.Contains("="))
        {
            var v = keys.Split('=');
            key = v[0];
            value = v[1];
        }
        else key = keys;
        var cmd = cmds.Find(x => x.key == key);
        if (cmd != null)
        {
            cmd.exe?.Invoke(value);
        }
    }
    public void OnHelp()
    {
        View(cmds);
    }
    public void OnClear()
    {
        View(new List<cmd>());
    }



    private void View(List<cmd> let)
    {
        Service.GameObj.DesAllParent(grid.transform);
        var height = 0;
        foreach (var c in let)
        {
            GameObject objPrefab = c.message.isnull() ? pObj : pObjApi;
            var obj = objPrefab.Create<UIObj>(grid.transform);
            height = (int)obj.Value;
            obj.gameObject.SetActive(true);
            obj.uiName.text = $"[{c.key}] : {c.name}";
            if (obj.uiDescription != null)
            {
                obj.uiDescription.text = c.message;
                obj.uiDescription.color = c.messageColor;
            }
            obj.uiIcon.enabled = c.icon != null;
            obj.uiIcon.mainTexture = c.icon;
            obj.onSumbit = (x) =>
            {
                if (c.guide.notnull())
                {
                    input.value = c.guide;
                }
                else c.exe.Invoke(string.Empty);
            };
        }
        grid.cellHeight = height;
        grid.repositionNow = true;
        sc.ResetPosition();
    }



    List<cmd> cmds = new List<cmd>();
    public class cmd
    {
        public string key;
        public string name;
        public string message;
        public Color messageColor = Color.white;
        public Texture icon;
        public string guide;
        public System.Action<string> exe;
    }













}
