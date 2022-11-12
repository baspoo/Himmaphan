using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class LeaderBoardPage : UIBase
    {
        public static LeaderBoardPage instance;
        public static LeaderBoardPage Open(   )
        {
            instance = CreatePage<LeaderBoardPage>(GameStore.instance.page.prefab_leaderboardPage);
            instance.Init( );
            return instance;
        }


        public ReuseScrollView reuse;

        private void Awake()
        {
            Init();
        }
        public void Init( ) 
        {

            GameControl.instance.network.GetLeaderBoard((datas) => {

                reuse.OnGetNumberOfRow = (sc) => { return datas.Count; };
                reuse.OnItemForRowAtIndex = (sc, item, index) => {

                    item.gameObject.SetActive(true);
                    var data = datas[index];
                    var obj = item.GetComponent<UIObj>();

                    obj.uiName.text = data.name;
                    obj.uiAmount.text = data.score.ToString("#,##0");
                    obj.onSumbit = (x) => {
                        ProfilePage.Open(data.userId).EventOnClose.add("reopen-leaderboard",()=> { 
                            OnVisible(true); 
                        });
                        OnVisible(false);
                    };

                };
                reuse.ReloadData();
            });
        }
        public void ClosePage()
        {
            OnClose();
        }



    }
}