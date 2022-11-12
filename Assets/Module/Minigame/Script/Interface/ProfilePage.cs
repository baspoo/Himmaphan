using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class ProfilePage : UIBase
    {
        public static ProfilePage instance;
        public static ProfilePage Open( string userId  )
        {
            if (instance != null)
                instance.OnClose();

            instance = CreatePage<ProfilePage>(GameStore.instance.page.prefab_profilePage);
            instance.Init(userId);
            return instance;
        }



        public UILabel ui_lbName;
        public UILabel ui_lbNickname;
        public UILabel ui_lbBegin;
        public UILabel ui_lbLast;
        public UILabel ui_lbLastScore;
        public UILabel ui_lbBestScore;
        public UILabel ui_lbPlayed;
        public Transform btnEdit;
        string userId;
        FirebaseSimple.FirebaseService.User user;

        public void Init(string userId) 
        {
            this.userId = userId;
            btnEdit.SetActive(GameControl.instance.network.userId == userId);
            GameControl.instance.network.GetUser(userId, (user) => {
                UpdateInterface(user);
            });
        }
        void UpdateInterface( FirebaseSimple.FirebaseService.User user  ) 
        {
            this.user = user;
            ui_lbName.text = $"Name : {user.name}";
            ui_lbNickname.text = $"Nickname : {user.nickname}";
            ui_lbBegin.text = $"Register : {Service.Time.UnixTimeStampToDateTime(user.unixBegin)}";
            ui_lbLast.text = $"Last Update : {Service.Time.UnixTimeStampToDateTime(user.unixLast)}";
            ui_lbLastScore.text = $"Last Score : {user.lastScore.ToString("#,##0")}";
            ui_lbBestScore.text = $"Best Score : {user.topScore.ToString("#,##0")}";
            ui_lbPlayed.text = $"Play Count : {user.played.ToString("#,##0")}";
        }


        public void ClosePage()
        {
            OnClose();
        }
        public void EditProfile()
        {
            GameControl.instance.network.EditUser(() => {
                OnVisible(true);
                UpdateInterface(user);
            });
            OnVisible(false);
        }


    }
}