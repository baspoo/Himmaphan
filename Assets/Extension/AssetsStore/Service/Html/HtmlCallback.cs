using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class HtmlCallback : MonoBehaviour
{

    // Unity Send To Html
    [DllImport("__Internal")]
    private static extern void OnHtmlPing(int code);

    public static void onHtmlPing(int code)
    {
        OnHtmlPing(code);

        // Add On Html Script
        // function HtmlPing( code )  {  }
    }




    [DllImport("__Internal")]
    private static extern void OnHtmlMessage(int code, string str);

    public static void onHtmlMessage(int code, string str)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
               OnHtmlMessage(code, str);
        #endif
        // Add On Html Script
        // function HtmlMessage( code , str )  {  }
    }








    

    public static void OnFullscreen( )
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
                OnHtmlMessage(1, "fullscreen");
        #else
                UIPopupPage.Open(
                    LoadCenter.instance.Language("notif_header"),
                    LoadCenter.instance.Language("error_message_cantfullscreen") ,
                    () => { });
        #endif
    }
    public static void GoToURL(string url)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
                OnHtmlMessage(2, url);
        #else
                Application.OpenURL(url);
        #endif
    }
    public static void Copy(string str)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            OnHtmlMessage(3,str);
        #else
            str.Copy();
        #endif
    }
    public static void ClearCache()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        OnHtmlMessage(4, string.Empty); // clear cache
        OnHtmlMessage(5, string.Empty); // refresh - reload page
        #endif
    }





    public static TaskService.Function onCallback = new TaskService.Function();

    // Html Send To Unity
    // unityInstance.SendMessage(  <gameobject.name> , <function>, <parms> );
    // unityInstance.SendMessage('HtmlCallback', 'Callback', 'string message');
    public void Callback( string str)
    {
        onCallback?.callall(str);
    }
















}
