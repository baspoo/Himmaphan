using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Center
{
    public class LoadCenter : MonoBehaviour
    {

        public static LoadCenter instance { get; private set; }

        public void Init()
        {
            instance = this;
        }



















        #region Download-Error
        public List<ApiHistoryData> ApiHistoryDatas = new List<ApiHistoryData>();
        public class ApiHistoryData
        {
            public System.DateTime date;
            public string url;
            public string result;
            public bool complete;
            public Texture2D image;
        }
        void AddApiHistory(string url, string result, bool complete, Texture2D image = null)
        {
            ApiHistoryDatas.Add(new ApiHistoryData()
            {
                date = System.DateTime.Now,
                url = url,
                result = result,
                image = image,
                complete = complete
            });
        }
        void Error(string errormessage)
        {
            Debug.LogError($"ERRPR = {errormessage}");
        }
        #endregion










        #region Download-Plist
        [SerializeField] Data.PlistData plist;
        public void GetPlistData(System.Action<bool> callback) => StartCoroutine(DoPlistData(callback));
        IEnumerator DoPlistData(System.Action<bool> callback)
        {
            if (Data.PlistData.plist != null)
            {
                this.plist = Data.PlistData.plist;
                callback?.Invoke(true);
                yield break;
            }


            //** New Plist
            Data.PlistData plist = new Data.PlistData();



            //** Download Config
            var config = false;
            yield return StartCoroutine(DoDownloadPlistData("config", (json) => {
                if (json.notnull()) 
                {
                    plist.config = json.DeserializeObject<Data.PlistData.Config>();
                    config = true;
                }
            }));
            while (!config) yield return new WaitForEndOfFrame();


            //** Download Interactive
            var interactive = false;
            yield return StartCoroutine(DoDownloadPlistData("interactive", (json) => {
                if (json.notnull())
                {
                    plist.interactive = json.DeserializeObject<Data.PlistData.Interactive>();
                    interactive = true;
                }
            }));
            while (!interactive) yield return new WaitForEndOfFrame();


            //** Download Minigame
            var minigame = false;
            yield return StartCoroutine(DoDownloadPlistData("minigame", (json) => {
                if (json.notnull())
                {
                    plist.minigame = json.DeserializeObject<Data.PlistData.Minigame>();
                    minigame = true;
                }
            }));
            while (!minigame) yield return new WaitForEndOfFrame();



            //** VerifyVersion
            var verify = false;
            yield return StartCoroutine(DoVerifyVersion(plist.config.version, (complete) => { verify = complete; }));
            while (!verify) yield return new WaitForEndOfFrame();


            //** Assing
            Data.PlistData.plist = plist;
            this.plist = plist;
            callback?.Invoke(true);
        }
        IEnumerator DoDownloadPlistData(string plistName, System.Action<string> callback)
        {
            var url = $"{Application.streamingAssetsPath}/plist/{plistName}.json";
            WWW www = new WWW(url);
            yield return www;
            if (www.error.isnull())
            {
                //-> Complete
                var Json = www.text;
                //Debug.Log(Json);
                if (Json.notnull())
                {
                    callback?.Invoke(Json);
                }
                else
                {
                    Error($"Download-Data [Plist : {plistName}] = Json null");
                    callback?.Invoke(null);
                }
                AddApiHistory(url, Json, true);
            }
            else
            {
                //-> Error
                Error($"Download-Data [Plist : {plistName}] = {www.error}");
                AddApiHistory(url, www.error, false);
                callback?.Invoke(null);
            }
            www.Dispose();
        }
        public string version
        {
            get
            {
                return PlayerPrefs.GetString("appversion");
            }
            set
            {
                PlayerPrefs.SetString("appversion", value);
            }
        }
        IEnumerator DoVerifyVersion(string current_version, System.Action<bool> callback)
        {
            bool versionverify = false;
            if (version.isnull())
            {
                //** first time.
                Debug.Log("first time");
                version = current_version;
                versionverify = true;
            }
            else
            {
                if (current_version != version)
                {
                    //** change version.
                    Debug.Log($"change version {current_version} : {version}");
                    while (version != current_version)
                    {
                        version = current_version;
                        yield return new WaitForEndOfFrame();
                    }
                    versionverify = false;
                }
                else versionverify = true;
            }

            if (versionverify)
            {
                yield return new WaitForEndOfFrame();
                callback.Invoke(true);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                Debug.Log($"reload version save {version}");
                HtmlCallback.ClearCache();
                callback.Invoke(false);
            }
        }
        #endregion


































        #region Download-Image
        public void DownloadImage(string url, System.Action<Texture> onComplete)
        {
            StartCoroutine(DoDownloadImage(url, onComplete));
        }
        public class HandleDownload
        {
            public string url;
            public UITexture uiimage;
            public Renderer render;
            public SpriteRenderer spr;
            public bool isDispose;
            public int index;
            public Coroutine corotine;
            public void OnDispose() => isDispose = true;
        }
        List<HandleDownload> m_HandleDownloads = new List<HandleDownload>();
        public HandleDownload DownloadImage(string url, UITexture uiimage, bool manualDispose = false)
        {
            if (uiimage != null)
                uiimage.mainTexture = getDefaultImage(uiimage.gameObject, (Texture2D)uiimage.mainTexture);

            var handle = new HandleDownload()
            {
                url = url,
                uiimage = uiimage,
                isDispose = false
            };
            return DownloadImage(handle, manualDispose);
        }
        public HandleDownload DownloadImage(string url, Renderer render, int index, bool manualDispose = false)
        {
            if (render != null)
                render.materials[index].mainTexture = getDefaultImage(render.gameObject, (Texture2D)render.materials[index].mainTexture); ;


            var handle = new HandleDownload()
            {
                url = url,
                render = render,
                index = index,
                isDispose = false
            };
            return DownloadImage(handle, manualDispose);
        }
        public HandleDownload DownloadImage(string url, SpriteRenderer spr, bool manualDispose = false)
        {
            if (spr != null)
                spr.sprite = getSprite((Texture2D)defaultImage);

            var handle = new HandleDownload()
            {
                url = url,
                spr = spr,
                isDispose = false
            };
            return DownloadImage(handle, manualDispose);
        }

        List<string> downloading = new List<string>();
        List<HandleDownload> m_StockHandleDownloads = new List<HandleDownload>();
        TaskService.JobService job = new TaskService.JobService();
        HandleDownload DownloadImage(HandleDownload handle, bool manualDispose = false)
        {
            //if (handle.uiimage != null)
            //    handle.uiimage.mainTexture = getDefaultImage(handle.uiimage.gameObject, (Texture2D)handle.uiimage.mainTexture);
            //if (handle.render != null)
            //    handle.render.materials[handle.index].mainTexture = getDefaultImage(handle.render.gameObject, (Texture2D)handle.render.materials[handle.index].mainTexture); ;
            //if (handle.spr != null)
            //    handle.spr.sprite = getSprite((Texture2D)defaultImage);

            m_StockHandleDownloads.Add(handle);
            if (!manualDispose) m_HandleDownloads.Add(handle);


            if (!downloading.Contains(handle.url))
            {

                downloading.Add(handle.url);
                handle.corotine = StartCoroutine(DoDownloadImage(handle.url, (img) =>
                {
                    downloading.Remove(handle.url);
                    m_HandleDownloads.Remove(handle);
                    foreach (var h in m_StockHandleDownloads.FindAll(x => x.url == handle.url))
                        DownloadDone(h, img);
                }));

            }
            else
            {


            }
            return handle;
        }
        void DownloadDone(HandleDownload handle, Texture img)
        {
            m_StockHandleDownloads.Remove(handle);
            if (!handle.isDispose)
            {
                //Debug.Log($"{img != null} - {handle.url}" );
                if (handle.uiimage != null)
                    handle.uiimage.mainTexture = img != null ? img : getDefaultImage(handle.uiimage.gameObject);
                if (handle.render != null)
                    handle.render.materials[handle.index].mainTexture = img != null ? img : getDefaultImage(handle.render.gameObject);
                if (handle.spr != null)
                    handle.spr.sprite = getSprite((Texture2D)(img != null ? img : defaultImage));
            }
        }



        public void OnDisposeAllDownloadImage()
        {
            m_HandleDownloads.ForEach(x => x.OnDispose());
            m_HandleDownloads.Clear();
        }



        //** Default Image
        Texture defaultImage => Store.instance.image.defaultImage;
        Dictionary<GameObject, Texture2D> m_defaultimage = new Dictionary<GameObject, Texture2D>();
        Texture2D getDefaultImage(GameObject obj, Texture2D img = null)
        {
            if (m_defaultimage.ContainsKey(obj))
            {
                return m_defaultimage[obj];
            }
            else
            {
                m_defaultimage.Add(obj, img);
                return img;
            }
        }



        //** Stock Image
        Dictionary<string, Texture2D> m_stockimage = new Dictionary<string, Texture2D>();
        Dictionary<Texture2D, Sprite> m_stockSprite = new Dictionary<Texture2D, Sprite>();
        Sprite getSprite(Texture2D texture)
        {
            if (m_stockSprite.ContainsKey(texture))
            {
                return m_stockSprite[texture];
            }
            else
            {
                var dif = texture.height / 1024.0f;
                var sprite = Service.Image.TextureToSprite((Texture2D)texture, 100 * dif);
                m_stockSprite.Add(texture, sprite);
                return sprite;
            }
        }



        //int downloadImageCounter = 0;
        IEnumerator DoDownloadImage(string url, System.Action<Texture> onComplete)
        {



            if (url.isnull())
            {
                onComplete?.Invoke(null);
                yield break;
            }


            if (m_stockimage.ContainsKey(url))
            {
                onComplete?.Invoke(m_stockimage[url]);
                yield break;
            }



            yield return new WaitForSeconds(Random.RandomRange(0.0f, 0.2f));
            while (downloading.Count == 0) yield return new WaitForEndOfFrame();



            if (m_stockimage.ContainsKey(url))
            {
                onComplete?.Invoke(m_stockimage[url]);
            }
            else
            {




                //downloadImageCounter++;
                //Debug.Log($"downloadImageCounter : {downloadImageCounter}                 -{url}");

                WWW www = new WWW(url);
                //Debug.LogError($"download : {url}");

                yield return www;
                if (www.error.notnull())
                {
                    // set defualt image.
                    //Debug.LogError("myTexture is error");
                    //Debug.LogError(www.error);
                    AddApiHistory("download image", url, false);
                    onComplete?.Invoke(null);
                }
                else
                {
                    Texture2D myTexture = www.texture;
                    if (myTexture.updateCount == 0) myTexture = null;
                    AddApiHistory("download image", url, true, myTexture);
                    if (myTexture != null)
                    {
                        //Debug.Log(myTexture.dimension);
                        //Debug.Log(myTexture.format);
                        //Debug.Log(myTexture.updateCount);
                        //Debug.Log($"{myTexture.width}x{myTexture.height}");
                        //Debug.Log(myTexture.vtOnly);
                        //Debug.Log(myTexture.hideFlags);
                        //Debug.Log("-----------------------------------------------------------------");




                        if (!m_stockimage.ContainsKey(url))
                            m_stockimage.Add(url, myTexture);

                        //Debug.LogError("myTexture is complete");
                        onComplete?.Invoke(myTexture);
                    }
                    else
                    {
                        //Debug.LogError("myTexture is null");
                        onComplete?.Invoke(null);
                    }
                }
                //Debug.Log($"DownloadImage : { ((www.texture!=null)? www.texture.name : "null") } : {url} : {www.error}");
                www.Dispose();
            }
        }
        #endregion








        #region Download-Language
        public string Language(string key, string replace = null , string defaultText = null)
        {
            if (Data.PlistData.plist != null)
            {
                Dictionary<string, string> dict = null;
                switch (Center.ManagerCenter.SceneName)
                {
                    case RootManager.SceneName.None:
                        if(Data.PlistData.plist.config!=null) dict = Data.PlistData.plist.config.languages;
                        break;
                    case RootManager.SceneName.Minigame:
                        if (Data.PlistData.plist.minigame != null) dict = Data.PlistData.plist.minigame.languages;
                        break;
                    case RootManager.SceneName.Interactive:
                        if (Data.PlistData.plist.interactive != null) dict = Data.PlistData.plist.interactive.languages;
                        break;
                }

                if (dict!=null && dict.ContainsKey(key)) 
                {
                    if (replace == null) return dict[key];
                    else return dict[key].Replace("@", replace);
                }
            }
            return defaultText != null ? defaultText : key;
        }
        #endregion




























    }
}