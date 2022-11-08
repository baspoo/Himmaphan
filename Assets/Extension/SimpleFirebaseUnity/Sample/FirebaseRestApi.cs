using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Proyecto26;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using Newtonsoft.Json;

public class FirebaseRestApi : MonoBehaviour
{

    [System.Serializable]
    public class Account
    {
        public User userData = null;
        public PublicData publicData = null;
    }
    [System.Serializable]
    public class PublicData
    {
        public string publicToken;


        public Content content = new Content();
        [System.Serializable]
        public class Content
        {
            public string currentDevice;
            public string rank;
            public int exp;
        }
    }

    [System.Serializable]
    public class User {


        public string uid;
        public string token;


        public Profile profile = null;
        [System.Serializable]
        public class Profile
        {
            public string username;
            public int age;
        }

        public Data data = null;
        [System.Serializable]
        public class Data
        {
            public int exp;
            public string chapters;
        }


    }




    public UnityEngine.UI.Text text;


    Firebase firebase;
    FirebaseQueue firebaseQueue;
    void Start()
    {
        // Create a FirebaseQueue
        firebaseQueue = new FirebaseQueue(true, 3, 1f);
        firebase = Firebase.CreateNew($"{"https://"}gamedata-2fc04-default-rtdb.firebaseio.com/users/{userID}", "AIzaSyCAwJHY4mRdQ4IiRPN-vkj0kP_5BS_52hI");
        




        //Get
        firebase.OnGetSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-OnGetSuccess] Raw Json: " + snapshot.RawJson);
        };
        firebase.OnGetFailed = (sender, error) => {
            Debug.LogError("[ERR-GetFailed ] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };
        //Set
        firebase.OnSetSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-SetSuccess] {sender.FullKey} Raw Json: " + snapshot.RawJson);
        };
        firebase.OnSetFailed = (sender, error) => {
            Debug.LogError("[ERR-SetFailed] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };
        //Update
        firebase.OnUpdateSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-UpdateSuccess] {sender.FullKey} Raw Json: " + snapshot.RawJson);
        };
        firebase.OnUpdateFailed = (sender, error) => {
            Debug.LogError("[ERR-UpdateFailed] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };





        FirebaseObserver observer = new FirebaseObserver(firebase.Child("publicData/content", true), 1f);
        observer.OnChange += (Firebase sender, DataSnapshot snapshot) =>
        {

            Debug.Log($"[OBSERVER] Raw Json: " + snapshot.RawJson);
            text.text = snapshot.RawJson;

            Debug.Log($"[OBSERVER] Raw Json: " + JsonConvert.SerializeObject(snapshot.RawValue));


        };
        observer.Start();

        StartCoroutine(getTime());

    }



    IEnumerator getTime() {

        yield return new WaitForEndOfFrame();
       // //var url = "https://showcase.api.linx.twenty57.net/UnixTime/tounix?date=now";
       // var url = "https://script.google.com/macros/s/AKfycbyODzFRYWLVh_7EpsrTXXEYi2GEJ4UBOZMV_OsXeHY-RH1MRVh-xRpH7Y_LRxyMvEci/exec";

       // WWW www = new WWW(url);
       // yield return www;
        
        
       //if(www.error==null) Debug.Log(www.text);
       // else Debug.Log(www.error);
    }




    public string userID;
    public string token;

    public string authtoken
    {
        get 
        {
            var data = new {
                provider = "anonymous",
                uid = userID
            };
            var json = JsonUtility.ToJson(data);
            return json;
        }
    }




    string ToJson(object myObject)
    {
        return JsonConvert.SerializeObject(myObject, Newtonsoft.Json.Formatting.None,
         new JsonSerializerSettings
         {
             NullValueHandling = NullValueHandling.Ignore
         });
    }




    void Update()
    {

#if UNITY_EDITOR


        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("GetTime");
            // ".info/serverTimeOffset"
            Firebase lastUpdate = firebase.Child("publicData/content/time", true);
            lastUpdate.OnSetSuccess = (sender, snap) =>
            {
                long timeStamp = snap.Value<long>();
                var dateTime = Firebase.TimeStampToDateTime(timeStamp);
                Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson}");
                Debug.Log($"[OK-GetTime] Datetime: {dateTime}");
            };
            lastUpdate.SetValue(Firebase.SERVER_VALUE_TIMESTAMP,true);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Put");
            // ".info/serverTimeOffset"
            Firebase put = firebase.Child("publicData/content/put", true);
            put.OnPushSuccess = (sender, snap) =>
            {
                Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson}");
            };
            put.Push(Random.RandomRange(1111,9999));
        }




        if (Input.GetKeyDown(KeyCode.A))
        {
            FirebaseQueue firebaseQueue = new FirebaseQueue(true, 3, 1f);
            Firebase add = firebase.Child("publicData/content/add", true);
            add.OnGetSuccess = (sender, snap) =>
            {
                var val = snap.Value<long>();
                Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson} : {val}");
                val = val + 1;
                firebaseQueue.AddQueueSet(add, val);
            };
            firebaseQueue.AddQueueGet(add);
        }




        if (Input.GetKeyDown(KeyCode.Z))
        {
            var index = Random.RandomRange(1, 9);
            Firebase add = firebase.Child($"publicData/content/add/{index}", true);
            add.OnGetSuccess = (sender, snap) =>
            {
                var val = snap.Value<long>();
                Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson} : {val}");
                val = val + 1;
                add.SetValue(val);
            };
            add.GetValue();
        }







        if (Input.GetKeyDown(KeyCode.G))
        {
            PublicData publicData = new PublicData();
            publicData.publicToken = token;
            var json = ToJson(publicData);
            Debug.Log(json);
            firebase.Child("publicData", true).UpdateValue(json , FirebaseParam.Empty.Auth(authtoken));
        }

        if (Input.GetKeyDown(KeyCode.F)) 
        {

            var userData = new User() { };
            userData.profile = new User.Profile();
            userData.data = new User.Data();

            userData.uid = userID;
            userData.token = token;
            userData.profile.username = "baspoo";
            userData.profile.age = 20;
            userData.data.exp = 758;
            userData.data.chapters = "g1,g2,g3";

            var json = ToJson(userData);
            Debug.Log(json);
            firebase.Child("userData", true).UpdateValue(json,FirebaseParam.Empty.Auth(authtoken));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            User userData = new User() { };
            userData.profile = new User.Profile();
            userData.uid = userID;
            userData.token = token;
            userData.profile.age = 123;
            var json = ToJson(userData);
            Debug.Log(json);
            firebase.Child("userData", true).UpdateValue(json  , FirebaseParam.Empty.Auth(authtoken));
        }






        if (Input.GetKeyDown(KeyCode.S))
        {
            firebase.Child("userData",true).GetValue( );
        }

#endif


    }








}
