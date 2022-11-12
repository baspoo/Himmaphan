using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FirebaseSimple.Test
{

    public class TestScript : MonoBehaviour
    {
        public static TestScript testScript;
        public FirebaseService firebaseService;
        public string bucket;
        public string userId;
        public string username;
        public FirebaseService.User user;
        public List<FirebaseService.Runtime.Chat> chats;
        public List<FirebaseService.Runtime.Score> scores;
       


        void Start()
        {

       
            testScript = this;


            //** Init
            firebaseService.Init(bucket, userId , ()=> {

                Debug.Log("FirebaseService.DONE");


                //** New User
                if (firebaseService.user == null)
                {
                    Debug.Log("New User");
                    user = new FirebaseService.User();
                    user.name = username;
                    user.nickname = "baspoo";
                    user.userId = userId;
                    firebaseService.AddUser(user);
                }


                //** checkmessage > firstTime-login
                InComeMessage(firebaseService.runtime.chats);

            });

            //** checkmessage > onupdate
            firebaseService.onChatUpdate = InComeMessage;



        }




        //** Read a New Messaage
        void InComeMessage(Dictionary<string, FirebaseService.Runtime.Chat> chats)
        {
            foreach (var ch in chats)
            {
                if (firebaseService.ReadChat(ch.Key))
                {
                    Debug.Log($"message : {ch.Value.message}");
                }
            }
        }









        Service.Timmer.Update timeout = new Service.Timmer.Update();
        private void Update()
        {

            
            if (!firebaseService.IsDone)
                return;


            //** debug view data on inspector **
            timeout.OnUpdate(()=> {

                user = firebaseService.user;

                chats = new List<FirebaseService.Runtime.Chat>();
                foreach (var chat in firebaseService.runtime.chats)
                {
                    chat.Value.name = chat.Key;
                    chats.Add(chat.Value);
                }

                scores = new List<FirebaseService.Runtime.Score>();
                foreach (var score in firebaseService.runtime.scores)
                {
                    score.Value.name = score.Key;
                    scores.Add(score.Value);
                }
            });



        }



#if UNITY_EDITOR
        public RuntimeBtn GetUser = new RuntimeBtn((r)=> {
            testScript.firebaseService.GetUser((user)=> {
                Debug.Log($"GetUser Done : {user.SerializeToJson()}");
                testScript.user = user;
            });
        });
        public RuntimeBtn UpdateUser = new RuntimeBtn((r) => {
            testScript.firebaseService.UpdateUser();
        });
        public RuntimeBtn GetRuntime = new RuntimeBtn((r) => {
            testScript.firebaseService.GetRuntime((runtime)=> {
                Debug.Log($"GetRuntime Done : {runtime.SerializeToJson()}");
            });
        });
        public RuntimeBtn PushChat = new RuntimeBtn((r) => {
            testScript.firebaseService.PushChat(testScript.user.name, $"message - {Random.RandomRange(111111,999999)}" ,0);
        });
        public RuntimeBtn PushScore = new RuntimeBtn((r) => {
            testScript.firebaseService.PushScore(Random.RandomRange(0,999));
        });
#endif





    }

}
