


/* * * * *
 * URLParameters.cs
 * ----------------
 * 
 * This singleton script provides easy access to any URL components in a Web-build
 * Just use
 * 
 *     URLParameters.Instance.RegisterOnDone(OnDone);
 *     
 * To register a callback which will be invoked from javascript. The callback receives a
 * reference to the singleton instance. The instance has several properties to hold all the
 * different parts of the URL. In addition it will split and parse the search and hash /
 * fragment value into key/value pairs stored in a dictionary (SearchParameters, HashParameters)
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2012-2017 Markus Göbel (Bunny83)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class URLParameters : MonoBehaviour
{



    // exameple
    // url          .....http://www.index.html?parameter=123&also=baspoogamedev
    // parameter    .... "?parameter=123&also=baspoogamedev"


    // set testíng data here for in-editor-use
    // href | hash | host | hostname | pathname | port | protocol | search
    public static string TestData = "|||||||";
    private static URLParameters m_Instance = null;
    public static URLParameters Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (URLParameters)FindObjectOfType(typeof(URLParameters));
                if (m_Instance == null)
                    m_Instance = (new GameObject("URLParameters")).AddComponent<URLParameters>();
                //m_Instance.gameObject.name = "URLParameters";
                //DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }





    [System.Serializable]
    public class Parameters
    {
        public bool HaveInformation = false;
        public string RawData;
        public string Href;
        public string Hash;
        public string Host;
        public string Hostname;
        public string Pathname;
        public string Port;
        public string Protocol;
        public string Search;
        public Dictionary<string, string> SearchParams = new Dictionary<string, string>();
        public Dictionary<string, string> HashParams = new Dictionary<string, string>();
    }




    private System.Action<Parameters> m_OnDone = null;
    public void Request(System.Action<Parameters> aCallback)
    {
        StartCoroutine(_Request(aCallback));
    }



    public IEnumerator _Request(System.Action<Parameters> aCallback)
    {
        m_OnDone += aCallback;

#if UNITY_EDITOR
        m_OnDone(null);
        //SetAddressComponents(TestData);
#elif UNITY_WEBPLAYER
        const string WebplayerCode = "GetUnity ().SendMessage ('{0}', 'SetAddressComponents', location.href +'|'+ location.hash +'|'+ location.host +'|'+ location.hostname +'|'+ location.pathname +'|'+ location.port +'|'+ location.protocol +'|'+ location.search);";
        Application.ExternalEval(string.Format(WebplayerCode, gameObject.name));
#elif UNITY_WEBGL
        const string WebGLCode = "SendMessage ('{0}', 'SetAddressComponents', location.href +'|'+ location.hash +'|'+ location.host +'|'+ location.hostname +'|'+ location.pathname +'|'+ location.port +'|'+ location.protocol +'|'+ location.search);";
        Application.ExternalEval(string.Format(WebGLCode, gameObject.name));
#endif
        yield break;
    }

    //public IEnumerator Start()
    //{
    //    yield return null; // wait one frame to ensure all delegates are assigned.
    //Request();
    //}

    public void SetAddressComponents(string aData)
    {

        var param = new Parameters();

        string[] parts = aData.Split('|');
        param.RawData = aData;
        param.Href = parts[0];
        param.Hash = parts[1];
        param.Host = parts[2];
        param.Hostname = parts[3];
        param.Pathname = parts[4];
        param.Port = parts[5];
        param.Protocol = parts[6];
        param.Search = parts[7];
        var tmp = param.Search.TrimStart('?');
        var data = tmp.Split('&');
        for (int i = 0; i < data.Length; i++)
        {
            var val = data[i].Split('=');
            if (val.Length != 2)
                continue;
            param.SearchParams.Add(val[0], val[1]);
        }
        tmp = param.Hash.TrimStart('#');
        data = tmp.Split('&');
        for (int i = 0; i < data.Length; i++)
        {
            var val = data[i].Split('=');
            if (val.Length != 2)
                continue;
            param.HashParams.Add(val[0], val[1]);
        }

        param.HaveInformation = true;
        if (m_OnDone != null)
            m_OnDone(param);
    }
}


public static class IDictionaryExtension
{
    public static double GetDouble(this IDictionary<string, string> aDict, string aKey, double aDefault = 0d)
    {
        string tmp;
        if (aDict.TryGetValue(aKey, out tmp))
        {
            double val;
            if (double.TryParse(tmp, out val))
                return val;
        }
        return aDefault;
    }
    public static int GetInt(this IDictionary<string, string> aDict, string aKey, int aDefault = 0)
    {
        string tmp;
        if (aDict.TryGetValue(aKey, out tmp))
        {
            int val;
            if (int.TryParse(tmp, out val))
                return val;
        }
        return aDefault;
    }
}