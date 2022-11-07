using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupPage : UIBase
{

    public static UIPopupPage instance;
    public static UIPopupPage Open(string header, string message, System.Action yes = null, System.Action no = null)
    {
        instance = CreatePage<UIPopupPage>(Pages.prefab_popupPage);
        instance.Init(header, message, yes, no);
        return instance;
    }





    public UILabel lbHeader, lbMessage;
    public UIButton btnOk, btnAccept, btnCancel;
    System.Action yes , no;
    public void Init(string header , string message , System.Action yes = null, System.Action no = null)
    {
        SceneHandle.instance.lighting.Blur(true);

        lbHeader.text = header;
        lbMessage.text = message;

        this.yes = yes;
        this.no = yes;
        if (yes == null && no == null)
        {
            //Message style
            btnOk.gameObject.SetActive(false);
            btnAccept.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
        }
        else 
        {
            if (yes != null && no == null)
            {
                //Ok style
                btnOk.gameObject.SetActive(true);
                btnAccept.gameObject.SetActive(false);
                btnCancel.gameObject.SetActive(false);
            }
            else 
            {
                //Yes-no style
                btnOk.gameObject.SetActive(false);
                btnAccept.gameObject.SetActive(true);
                btnCancel.gameObject.SetActive(true);
            }
        }

    }
    public void Accept()
    {
        DesPage();
        yes?.Invoke();
    }
    public void Close()
    {
        DesPage();
        no?.Invoke();
    }

    void DesPage()
    {
        SceneHandle.instance.lighting.Blur(false);
        Playlist.let.sfx_click.Play();
        OnClose();
    }

}
