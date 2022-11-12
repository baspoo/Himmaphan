using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FirebaseSimple;

namespace MiniGame
{
    public class GameNetworkControl : MonoBehaviour
    {



        public class AccountInfo 
        {
            public string userId;
            public string bucket;

            public static AccountInfo Create(string bucket) 
            {
                var account = new AccountInfo()
                {
                    userId = $"{Service.String.IUnikey("U")}-{Random.RandomRange(111111, 999999)}-{Service.Time.DateTimeToUnixTimeStamp(System.DateTime.Now)}",
                    bucket = bucket
                };
                PlayerPrefs.SetString("account", account.SerializeToJson());
                return account;
            }
            static AccountInfo m_account;
            public static AccountInfo account
            {
                get
                {
                    if (m_account == null)
                    {
                        if (isHasAccount)
                        {
                            m_account = PlayerPrefs.GetString("account").DeserializeObject<AccountInfo>();
                        }
                    }
                    return m_account;
                }
            }
            public static bool isHasAccount
            {
                get
                {
                    return PlayerPrefs.HasKey("account");
                }
            }
        }

















        FirebaseService firebase => FirebaseService.Instance;
        public bool isNetworkReady { get; private set; }
        public string userId => firebase.user != null ? firebase.user.userId : AccountInfo.account.userId;

        public void Init( )
        {
            StartCoroutine(DoInit());
        }


        IEnumerator DoInit() 
        {

            AccountInfo acc = null;
            if (AccountInfo.isHasAccount)
            {
                acc = AccountInfo.account;
            }
            else 
            {
                //Selete Bucket & Create Account
                AccountInfo.Create("swk");
            }
            while(acc == null)
                yield return new WaitForEndOfFrame();



            //** Init
            firebase.Init( acc.bucket , acc.userId, () => {

                Debug.Log("FirebaseService.DONE");

                //** New User
                if (firebase.user == null)
                {
                    Debug.Log("New User");
                    RegisterPage.Open((data) =>
                    {
                        var user = new FirebaseService.User();
                        user.userId = acc.userId;
                        user.name = $"{data.name} {data.lastname}";
                        user.nickname = data.nickname;
                        firebase.AddUser(user);
                        isNetworkReady = true;
                    });
                }
                else
                {
                    isNetworkReady = true;
                }
            });
        }


        public void StartGame()
        {
            Player.PlayerData.player.stat.TopScore = (int)firebase.user.topScore;
        }
        public void GameOver()
        {
            //UpdateScore
            var newScore = Player.PlayerData.player.stat.Score;
            firebase.PushScore(newScore);
        }




        public void GetUser( string userId ,System.Action<FirebaseService.User> callback)
        {
            firebase.GetUser(userId,(userData) => {
                callback?.Invoke(userData);
            });
        }
        public void EditUser( System.Action done)
        {
            RegisterPage.Open(new RegisterPage.RegisterData() { 
            
                name = firebase.user.name.Split(' ')[0],
                lastname = firebase.user.name.Split(' ')[1],
                nickname = firebase.user.nickname

            }, (data)=> {
                if (firebase.user != null) 
                {
                    firebase.user.name = $"{data.name} {data.lastname}";
                    firebase.user.nickname = data.nickname;
                    firebase.UpdateUser();
                    done?.Invoke();
                }
            });
        }


        public class ScoreData 
        {
            public string name;
            public string userId;
            public long score;
            public int index;
        }
        public void GetLeaderBoard( System.Action<List<ScoreData>> callback )
        {
            firebase.GetRuntime((runtime)=> {
                List<ScoreData> let = new List<ScoreData>();
                int index = 0;
                foreach (var score in new Dictionary<string, FirebaseService.Runtime.Score>(runtime.scores).OrderByDescending(x => x.Value.score)) 
                {
                    let.Add(new ScoreData()
                    {
                        index = index,
                        userId = score.Value.userId,
                        name = score.Value.name,
                        score = score.Value.score
                    });
                }
                callback?.Invoke(let);
            });
        }



    }
}