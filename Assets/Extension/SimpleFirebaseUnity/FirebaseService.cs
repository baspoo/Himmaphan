using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using SimpleFirebaseUnity;
using Newtonsoft.Json;
using UnityEngine.Networking;



public class FirebaseService : MonoBehaviour
{
    static FirebaseService m_Instance;
    public static FirebaseService Instance{
        get {
            if(m_Instance == null) 
            {
                var fireBase = new GameObject("FirebaseService");
                m_Instance = fireBase.AddComponent<FirebaseService>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }







    bool m_init = false;
    public void Init(string bucket , string userId, System.Action complete) 
    {
        if (m_init)
        {
            complete?.Invoke();
        }
        else 
        {
            StartCoroutine(DoInit(bucket, userId, complete));
        }
    }
    IEnumerator DoInit(string bucket, string userId, System.Action complete) 
    {
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Setup(bucket, userId, ()=> {
            m_init = true;
        }));
        complete?.Invoke();
    }


































    public User user { get; private set; }
    [System.Serializable]
    public class User
    {
        public string name;
        public string userId;
        public long unixBegin;
        public long unixLast;
        public int played;
        public long topScore;
        public long lastScore;
    }

    public Runtime runtime { get; private set; }
    [System.Serializable]
    public class Runtime
    {
        public Dictionary<string, Chat> chats = new Dictionary<string, Chat>();
        [System.Serializable]
        public class Chat
        {
            public string name;
            public string message;
            public int code;
            public long date;
        }


        public Dictionary<string, Score> scores = new Dictionary<string, Score>();
        [System.Serializable]
        public class Score
        {
            public string name;
            public string userId;
            public long score;
            public long unix;
        }
    }













    [SerializeField] bool isLog;
    [SerializeField] string bucket;
    [SerializeField] string userId;
    public bool IsDone { get; private set; }
    public System.Action<Dictionary<string, Runtime.Chat>> onChatUpdate;

    Firebase firebaseUser;
    Firebase firebase;
    FirebaseQueue firebaseQueue;
    System.Action done;
    IEnumerator Setup(string bucket, string userId, System.Action done)
    {
        this.bucket = bucket;
        this.userId = userId;

        string url = "https://minigame-3e6e8-default-rtdb.asia-southeast1.firebasedatabase.app/"+ bucket ;


        // Create a FirebaseQueue
        firebaseQueue = new FirebaseQueue(true, 3, 1f);
        firebase = Firebase.CreateNew($"{url}");
        firebaseUser = Firebase.CreateNew($"{url}/users/{userId}");


        firebase.OnGetFailed = (sender, error) => {
            if(isLog) Debug.LogError("[ERR-GetFailed ] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };
        //Set
        firebase.OnSetSuccess = (sender, snapshot) => {
            if (isLog) Debug.Log($"[OK-SetSuccess] {sender.FullKey} Raw Json: " + snapshot.RawJson);
        };
        firebase.OnSetFailed = (sender, error) => {
            if (isLog) Debug.LogError("[ERR-SetFailed] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };
        //Update
        firebase.OnUpdateSuccess = (sender, snapshot) => {
            if (isLog) Debug.Log($"[OK-UpdateSuccess] {sender.FullKey} Raw Json: " + snapshot.RawJson);
        };
        firebase.OnUpdateFailed = (sender, error) => {
            if (isLog) Debug.LogError("[ERR-UpdateFailed] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };


        FirebaseObserver observer = new FirebaseObserver(firebase.Child("runtime/chats", true), 1f);
        observer.OnChange = (Firebase sender, DataSnapshot snapshot) =>
        {
            runtime.chats = JsonConvert.DeserializeObject<Dictionary<string, Runtime.Chat>>(snapshot.RawJson);
            ChatVerify(runtime);
            onChatUpdate?.Invoke(runtime.chats);
        };
        observer.Start();




        bool timeDone = false;
        GetTime(() => { timeDone = true; });
        while (!timeDone) yield return new WaitForEndOfFrame();



        bool userDone = false;
        GetUser((user) => { this.user = user; userDone = true; });
        while (!userDone) yield return new WaitForEndOfFrame();



        bool runtimeDone = false;
        GetRuntime((runtime) => { this.runtime = runtime; runtimeDone = true; });
        while (!runtimeDone) yield return new WaitForEndOfFrame();




        IsDone = true;
        done?.Invoke();
    }


    #region GetTime
    public long unixServer { get; private set; }
    public Service.Time.TimeServer timeServer { get; private set; }
    public void GetTime(System.Action done)
    {
        //** GetTime **
        Firebase lastUpdate = firebase.Child("time", true);
        lastUpdate.OnSetSuccess = (sender, snap) =>
        {

            long timeStamp = snap.Value<long>();
            var dateTime = Firebase.TimeStampToDateTime(timeStamp);
            unixServer = timeStamp;
            timeServer = new Service.Time.TimeServer();
            timeServer.Init(dateTime);

            if (isLog) Debug.Log($"[OK-GetTime] {dateTime}      ||||||  Raw Json: {snap.RawJson}");
            done?.Invoke();
        };
        lastUpdate.SetValue(Firebase.SERVER_VALUE_TIMESTAMP, true);
    }
    #endregion












    #region User
    public void GetUser(System.Action<User> done)
    {
        //** GetUser **
        firebaseUser.OnGetSuccess = (sender, snapshot) => {
            var json = snapshot.RawJson;
            if (isLog) Debug.Log($"[OK-GetUser] Raw Json: " + json);

            if (json.isnull())
            {
                done?.Invoke(null);
            }
            else 
            {
                user = JsonConvert.DeserializeObject<User>(json);
                done?.Invoke(user);
            }
        };
        firebaseUser.GetValue();
    }
    public void AddUser(User newUser)
    {
        if (newUser == null) { Debug.LogError("user == null !!"); return; }

        newUser.unixBegin = timeServer.UnixTime;
        newUser.unixLast = timeServer.UnixTime;
        var json = newUser.SerializeToJson();
        if (isLog) Debug.Log($"AddUser : {json}");
        user = newUser;
        firebaseUser.UpdateValue(json);
    }
    public void UpdateUser()
    {
        if (user == null) { Debug.LogError("user == null !!"); return; }

        user.unixLast = timeServer.UnixTime;
        var json = user.SerializeToJson();
        if(isLog) Debug.Log($"UpdateUserData : {json}");
        firebaseUser.UpdateValue(json);
    }
    #endregion















    #region Runtime
    public void GetRuntime(System.Action<Runtime> done)
    {
        //** GetPet **
        firebase.OnGetSuccess = (sender, snapshot) => {
            var json = snapshot.RawJson;
            if (isLog) Debug.Log($"[OK-GetRuntime] Raw Json: " + json);
            runtime = json.isnull()? new Runtime()  :  JsonConvert.DeserializeObject<Runtime>(json);
            ChatVerify(runtime);
            ScoreVerify(runtime);
            done?.Invoke(runtime);
        };
        firebase.Child("runtime",true).GetValue();
    }








    public void PushChat(string name, string message, int code)
    {
        Runtime.Chat chat = new Runtime.Chat();
        chat.date = timeServer.UnixTime;
        chat.code = code;
        chat.name = name;
        chat.message = message;

        //** Push Chat **
        Firebase put = firebase.Child("runtime/chats", true);
        put.OnPushSuccess = (sender, snap) =>
        {
            if (isLog) Debug.Log($"[OK-PushChat] Raw Json: " + snap.RawJson);
        };
        put.Push(chat.SerializeToJson(), true);
    }



    public void PushScore(long newscore)
    {
        //** Load - UpdateRuntime
        GetRuntime((runtime) => {

            
            //** Filter
            bool isTopscore = false;
            if (runtime.scores.Count < maxScore)
            {
                isTopscore = true;
            }
            else
            {
                foreach (var s in runtime.scores) 
                {
                    if (newscore > s.Value.score) 
                    {
                        isTopscore = true;
                        continue;
                    }
                }
            }

            Debug.Log($"[{newscore}]  topscore amount: {runtime.scores.Count} < {maxScore} = {isTopscore}");

            //** Upload
            if (isLog) Debug.Log($"TopScoreVerify topscore: {isTopscore} - {newscore}");
            if (isTopscore)
            {
                Runtime.Score score = new Runtime.Score();
                score.unix = timeServer.UnixTime;
                score.score = newscore;
                score.name = user.name;
                score.userId = user.userId;

                //** Push Score **
                Firebase put = firebase.Child("runtime/scores", true);
                put.OnPushSuccess = (sender, snap) =>
                {
                    if (isLog) Debug.Log($"[OK-PushScore] Raw Json: " + snap.RawJson);
                    GetRuntime((r)=> { });
                };
                put.Push(score.SerializeToJson(), true);
            }

        });
    }








    int maxChat = 50;
    void ChatVerify(Runtime runtime)
    {
        int max = maxChat;
        foreach (var chat in new Dictionary<string, Runtime.Chat>(runtime.chats).OrderByDescending(x => x.Value.date))
        {
            //check day
            //var dateTime = Firebase.TimeStampToDateTime(chat.Value.date);

            if (max > 0 )
            {
                max--;
            }
            else
            {
                Debug.LogError($"Remove-Chat : {chat.Key} - {chat.Value.date}");
                Firebase updatechat = firebase.Child($"runtime/chats/{chat.Key}", true);
                updatechat.Delete();
                runtime.chats.Remove(chat.Key);
            }
        }
    }

    int maxScore = 100;
    void ScoreVerify(Runtime runtime)
    {
        int max = maxScore;
        foreach (var score in new Dictionary<string, Runtime.Score>(runtime.scores).OrderByDescending(x => x.Value.score))
        {
            if (max > 0)
            {
                max--;
            }
            else
            {
                Debug.LogError($"Remove-Score : {score.Key} - {score.Value.score}");
                Firebase updatechat = firebase.Child($"runtime/scores/{score.Key}", true);
                updatechat.Delete();
                runtime.scores.Remove(score.Key);
            }
        }
    }

    #endregion




















































    #region Utility
    string ToJson(object myObject)
    {
        return JsonConvert.SerializeObject(myObject, Newtonsoft.Json.Formatting.None,
         new JsonSerializerSettings
         {
             NullValueHandling = NullValueHandling.Ignore
         });
    }

    List<string> letReadchats = new List<string>();
    public bool ReadChat(string key)
    {
        if (!IsReadChat(key)) 
        {
            letReadchats.Add(key);
            if (letReadchats.Count > maxChat * 2)
            {
                maxChat.Loop(() => {
                    letReadchats.RemoveAt(0);
                });
            }
            return true;
        }
        return false;
    }
    public bool IsReadChat(string key) => letReadchats.Contains(key);
    #endregion





}
