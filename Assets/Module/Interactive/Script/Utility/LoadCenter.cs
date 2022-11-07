using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metaverse.Data;

public class LoadCenter : MonoBehaviour
{
    public static LoadCenter instance { get; private set; }
  
    public static void Init( )
    {
        instance = FindObjectOfType<LoadCenter>();
    }
     




    public void Download(System.Action<bool> done)
    {
        DoDownload(done);
    }

    [Header("Dummy PlagingData")]
    [SerializeField] bool IsDummy;
    [SerializeField] PlayingData playing;
    public RuntimeBtn EventToJson = new RuntimeBtn((x)=> {
        var eventData = x.Gameobject.GetComponent<LoadCenter>().playing.EventData;
        var json = eventData.SerializeToJson();
        json.Copy();
        Debug.Log(json);

        //http://junk.onemoby.com/baspoo/nftdemo/metaverse/obm/image/p1_1.jpg
        string url = "http://junk.onemoby.com/baspoo/nftdemo/metaverse/obm/image";



        //eventData.landingImage = $"{url}/landingimage.jpeg";
        //foreach (var floor in eventData.floors)
        //{

        //    foreach (var booth in floor.booths)
        //    {

        //        booth.seller.logo = $"{url}/logo.png";

        //        int billboard = 0;
        //        foreach (var banner in booth.seller.banners)
        //        {
        //            if (banner.type == "main")
        //            {
        //                banner.url = $"{url}/banner_main.jpg";
        //            }
        //            if (banner.type == "stand")
        //            {
        //                banner.url = $"{url}/banner_stand.jpg";
        //            }
        //            if (banner.type == "cover")
        //            {
        //                banner.url = $"{url}/banner_cover.png";
        //            }
        //            if (banner.type == "billboard")
        //            {
        //                billboard++;
        //                banner.url = $"{url}/banner_billboard{billboard}.jpg";
        //            }
        //        }

        //    }
        //}


        //int productIndex = 0;
        //foreach (var product in x.Gameobject.GetComponent<LoadCenter>().playing.ProductDatas)
        //{
        //    productIndex++;
        //    int imageIndex = 0;
        //    foreach (var image in product.images)
        //    {
        //        imageIndex++;
        //        image.url = $"{url}/p{productIndex}_{imageIndex}.jpg";
        //    }
        //}



    });
    public RuntimeBtn ProductToJson = new RuntimeBtn((x) => {
        var json = x.Gameobject.GetComponent<LoadCenter>().playing.ProductDatas.SerializeToJson();
        json.Copy();
        Debug.Log(json);
    });


    [Header("Dummy Event Json")]
    [SerializeField] bool IsDummyEvent;
    [SerializeField] string DummyEventJson;

    void DoDownload(System.Action<bool> done) 
	{
		IEnumerator Do() 
        {
            InterfaceRoot.instance.OpenLoading(true);
            //--> P-List
            yield return StartCoroutine(DoPlistData());

            //--> EventData
            bool complete = true;
            yield return StartCoroutine(URLParameters.Instance._Request((data)=> {
                if (data != null)
                    PlayingData.Inst.Usage.EventID = data.SearchParams.Find("eventId");
                else
                    PlayingData.Inst.Usage.EventID = playing.Usage.EventID;
            }));
            yield return StartCoroutine(DoGetEventData(PlayingData.Inst.Usage.EventID , (finish)=> { 
                complete = finish; 
                if(!complete)
                    InterfaceRoot.instance.OpenLoading(false);
            }));

            //--> Done
            yield return new WaitForEndOfFrame();
            playing = PlayingData.Inst;
            done?.Invoke(complete);
            InterfaceRoot.instance.OpenLoading(false);
        }
		StartCoroutine(Do());
	}







    #region Download-Error
    public List<ApiHistoryData> ApiHistoryDatas = new List<ApiHistoryData>();
    public class ApiHistoryData {
        public System.DateTime date;
        public string url;
        public string result;
        public bool complete;
        public Texture2D image;
    }
    public void AddApiHistory(string url, string result, bool complete , Texture2D image = null) {
        ApiHistoryDatas.Add(new ApiHistoryData() {
            date = System.DateTime.Now,
            url = url,
            result = result,
            image = image,
            complete = complete
        });
    }
    void Error( string errormessage ) 
    {
        Debug.LogError($"ERRPR = {errormessage}");
    }
    #endregion






    #region Download-Plist
    IEnumerator DoPlistData()
    {
        var url = $"{Application.streamingAssetsPath}/plist.json";
        WWW www = new WWW(url);
        yield return www;
        if (www.error.isnull())
        {
            //-> Complete
            var Json = www.text;
            //Debug.Log(Json);
            if (Json.notnull())
            {
                PlayingData.Inst.PlistData = Json.DeserializeObject<Data.PlistData>();

                var verify = false;
                yield return StartCoroutine(DoVerifyVersion(PlayingData.Inst.PlistData.version, (complete) => { verify = complete; }));
                while (!verify) yield return new WaitForEndOfFrame();
            }
            else Error($"Download-Data [Plist] : Json == null");
            AddApiHistory(url, Json, true);
        }
        else
        {
            //-> Error
            Error($"Download-Data [Plist] : {www.error}");
            AddApiHistory(url, www.error, false);
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
    IEnumerator DoVerifyVersion( string current_version , System.Action<bool> callback )
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








    #region Download-EventData
    [SerializeField] bool ForceIgnoreEditor;
    IEnumerator DoGetEventData(string eventID , System.Action<bool> callback )
    {
        if (Service.IsEditor && !ForceIgnoreEditor)
        {
            if (IsDummy)
            {
                PlayingData.Inst.Usage = playing.Usage;
                PlayingData.Inst.EventData = playing.EventData;
                PlayingData.Inst.ProductDatas = playing.ProductDatas;
                callback?.Invoke(true);
                yield break;
            }
            if (IsDummyEvent)
            {
                PlayingData.Inst.EventData = DummyEventJson.DeserializeObject<Data.EventData>();
                callback?.Invoke(true);
                yield break;
            }
        }
        else 
        {
            if (eventID == "dummy") 
            {
                Debug.LogError("--- USING DUMMY EVENT DATA! ---");

                var eventPath = $"{Application.streamingAssetsPath}/debug/event.json";
                WWW eventDownload = new WWW(eventPath);
                yield return eventDownload;
                if (eventDownload.error.isnull())
                {
                    PlayingData.Inst.EventData = eventDownload.text.DeserializeObject<Data.EventData>();
                }
                else 
                {
                    callback?.Invoke(false);
                    yield break;
                }

                var productPath = $"{Application.streamingAssetsPath}/debug/products.json";
                WWW producctsDownload = new WWW(productPath);
                yield return producctsDownload;
                if (producctsDownload.error.isnull())
                {
                    PlayingData.Inst.ProductDatas = producctsDownload.text.DeserializeObject<List<Data.ProductData>>();
                }
                else
                {
                    callback?.Invoke(false);
                    yield break;
                }

                IsDummy = true;
                callback?.Invoke(true);
                yield break;
            }
        }



        void EventNotFound(bool internet ,string errormessage) 
        {
            UIPopupPage.Open(
                LoadCenter.instance.Language("notif_header"), 
                LoadCenter.instance.Language( internet? "error_message_notfoundevent" : "error_message_cantconnectapi" ) 
                );

            Error(errormessage);
            callback?.Invoke(false);
        }

        var url = PlayingData.Inst.PlistData.apiData.api_getEventDetail.Replace("@", eventID);
        WWW www = new WWW(url);
        yield return www;
        if (www.error.isnull())
        {
            //-> Complete
            var Json = www.text;
            if (Json.notnull())
            {
                try
                {
                    PlayingData.Inst.EventData = Json.DeserializeObject<Data.EventData>();
                    callback?.Invoke(true);
                }
                catch (System.Exception e)
                {
                    EventNotFound(true,$"Download-Data [EventData] : Catch Json DeserializeObject. {e.Message} {e.StackTrace}");
                }
            }
            else EventNotFound(true, $"Download-Data [EventData] : Json == null");
            AddApiHistory(url, Json, true);
        }
        else
        {
            //-> Error
            AddApiHistory( url , www.error , false);
            EventNotFound(false, $"Download-Data [EventData] : {www.error}");
        }
        www.Dispose();
    }
    #endregion





    #region Download-ProductData
    public void GetProductData(string productID, System.Action<List<Data.ProductData>> callback) => StartCoroutine(DoGetProductData(productID, callback));
    IEnumerator DoGetProductData(string productID, System.Action<List<Data.ProductData>> callback)
    {
        if (IsDummy)
        {
            callback?.Invoke(PlayingData.Inst.ProductDatas);
            yield break;
        }

        var url = PlayingData.Inst.PlistData.apiData.api_getProductList.Replace("@", productID);

        InterfaceRoot.instance.OpenLoading(true);
        WWW www = new WWW(url);
        yield return www;
        InterfaceRoot.instance.OpenLoading(false);

        if (www.error.isnull())
        {
            //-> Complete
            var Json = www.text;
            if (Json.notnull())
            {
                var productDatas = Json.DeserializeObject<List<Data.ProductData>>();
                PlayingData.Inst.ProductDatas = productDatas;
                callback?.Invoke(productDatas);
            }
            else
            {
                Error($"Download-Data [ProductData] : Json == null");
                callback?.Invoke(new List<Data.ProductData>());
            }
            AddApiHistory(url, Json, true);
        }
        else
        {
            //-> Error
            Error($"Download-Data [ProductData] : {www.error}");
            callback?.Invoke(new List<Data.ProductData>());
            AddApiHistory(url, www.error, false);
        }
        www.Dispose();
    }
    #endregion










  













    #region Download-Image
    public void DownloadImage(string url , System.Action<Texture> onComplete)
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
    public HandleDownload DownloadImage(string url, UITexture uiimage , bool manualDispose = false)
	{
        if (uiimage != null)
            uiimage.mainTexture = getDefaultImage(uiimage.gameObject, (Texture2D)uiimage.mainTexture);

        var handle = new HandleDownload()
        {
            url = url,
            uiimage = uiimage,
            isDispose = false
        };
        return DownloadImage(handle, manualDispose) ;
    }
    public HandleDownload DownloadImage(string url, Renderer render , int index, bool manualDispose = false)
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
    void DownloadDone(HandleDownload handle , Texture img)
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



    public void OnDisposeAllDownloadImage( )
    {
        m_HandleDownloads.ForEach(x => x.OnDispose());
        m_HandleDownloads.Clear();
    }



    //** Default Image
    Texture defaultImage => Utility.Store.instance.image.defaultImage;
    Dictionary<GameObject, Texture2D> m_defaultimage = new Dictionary<GameObject, Texture2D>();
    Texture2D getDefaultImage(GameObject obj , Texture2D img = null)
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
    Sprite getSprite(Texture2D texture) {
        if (m_stockSprite.ContainsKey(texture))
        {
            return m_stockSprite[texture];
        }
        else 
        {
            var dif = texture.height / 1024.0f;
            var sprite = Service.Image.TextureToSprite((Texture2D)texture, 100* dif);
            m_stockSprite.Add(texture,sprite);
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



        yield return new WaitForSeconds(Random.RandomRange(0.0f,0.2f));
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
                AddApiHistory("download image", url , false);
                onComplete?.Invoke(null);
			}
			else
			{
				Texture2D myTexture = www.texture;
                if (myTexture.updateCount == 0) myTexture = null;
                AddApiHistory("download image", url , true , myTexture);
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
    public string Language( string key , string replace = null)
    {
        if (PlayingData.Inst != null && PlayingData.Inst.PlistData.languages.ContainsKey(key))
        {
            if (replace == null) return PlayingData.Inst.PlistData.languages[key];
            else return PlayingData.Inst.PlistData.languages[key].Replace("@", replace);
        }
        return key;
    }
    public void Language( string key, UILabel lable )
    {
        string message = lable.text;
        if (PlayingData.Inst != null && PlayingData.Inst.PlistData.languages.ContainsKey(key)) 
        {
            message = PlayingData.Inst.PlistData.languages[key];
        }
        lable.text = message;
    }
    #endregion

    











    #region Download-BoothModel
    Dictionary<string, GameObject> m_stockmodel = new Dictionary<string, GameObject>();
    public GameObject LoadBooth(string boothmodel)
    {
        if (m_stockmodel.ContainsKey(boothmodel))
            return m_stockmodel[boothmodel];
        else 
        {
            var booth = (GameObject)Resources.Load($"Prefab/Booth/{boothmodel}", typeof(GameObject));
            if (booth != null) 
            {
                m_stockmodel.Update(boothmodel, booth);
            }
            return booth;
        }
    }
    #endregion





















    //public class Bundle {

    //	public const string island = "island";
    //	public const string tree = "tree";
    //	public const string world = "world";
    //	public const string costume = "costume";
    //	public const string furniture = "furniture";
    //	public const string texture = "texture";
    //}




    ////Texture
    //public enum TextureBundle
    //{
    //	rank ,  item , furniture , island , tree , costume , mission , achievement , avatarprofile, activity,minigame
    //}
    //Dictionary<TextureBundle, Dictionary<string, Texture>> dictImage = new Dictionary<TextureBundle, Dictionary<string, Texture>>();
    //public Texture LoadTexture(TextureBundle texture , string filename )
    //{
    //	Texture image = null;
    //	if (dictImage.ContainsKey(texture))
    //	{
    //		image = dictImage[texture].Find(filename);
    //	}
    //	else dictImage.Add(texture,new Dictionary<string, Texture>());

    //	if (image == null)
    //	{
    //		image = (Texture)ResourcesHandle.Load(Bundle.texture, $"{texture}/{filename}", ResourcesHandle.FileType.png);
    //		dictImage[texture].Update(filename, image);
    //	}
    //	return image;
    //}
    //public Object[] LoadTextures(TextureBundle texture)
    //{
    //	return ResourcesHandle.LoadAll(Bundle.texture, $"{texture}");
    //}









    ////World
    //public GameObject LoadWorld() 
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.world, "world", ResourcesHandle.FileType.prefab);
    //}
    //public GameObject LoadMiniEvent(string miniEventID)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.world, $"minievent/{miniEventID}", ResourcesHandle.FileType.prefab);
    //}
    //public GameObject LoadMiniGame(string miniGameID)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.world, $"minigame/{miniGameID}", ResourcesHandle.FileType.prefab);
    //}
    //public GameObject LoadNPC(string npcID)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.world, $"npc/{npcID}", ResourcesHandle.FileType.prefab);
    //}




    ////Avatar
    //public GameObject LoadAvatar(Metaverse.Player.Avatar.AvatarUtility.AvatarCustomize.Gender Gender)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.costume, $"avatar/{(Gender == Metaverse.Player.Avatar.AvatarUtility.AvatarCustomize.Gender.Man? "man":"woman")}", ResourcesHandle.FileType.prefab);
    //}
    //public GameObject LoadSkinBase(string baseName) => DoLoadSkinBase(baseName);
    //public static GameObject DoLoadSkinBase(string baseName)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.costume, $"avatar/skin/{baseName}", ResourcesHandle.FileType.prefab);
    //}
    //public GameObject LoadCostume(CostumeData costumeData)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.costume, $"{costumeData.Type.ToString().ToLower()}/{costumeData.Id}", ResourcesHandle.FileType.prefab);
    //}


    ////Island
    //public Texture LoadIsland(IslandData islandData)
    //{
    //	return (Texture)ResourcesHandle.Load(Bundle.island, islandData.Id , ResourcesHandle.FileType.png);
    //}
    //public GameObject LoadIslandSurface(IslandData islandData)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.island, $"surface/{islandData.Id}", ResourcesHandle.FileType.prefab);
    //}
    ////Furniture
    //public GameObject LoadFurniture(string furnitureID)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.furniture, furnitureID , ResourcesHandle.FileType.prefab);
    //}
    ////Tree
    //public GameObject LoadTree(TreeData treeData, TreeData.TreeState state)
    //{
    //	return (GameObject)ResourcesHandle.Load(Bundle.tree, $"{treeData.Id}/{state}", ResourcesHandle.FileType.prefab);
    //}

}
