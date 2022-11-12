using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class SettingPage : UIBase
    {
        public static SettingPage instance;
        public static SettingPage Open( bool pause = false )
        {
            instance = CreatePage<SettingPage>(GameStore.instance.page.prefab_settingPage);
            instance.Init(pause);
            return instance;
        }




        [SerializeField] SettingZone settingZone;
        [System.Serializable]
        public class SettingZone
        {
            public Transform root;
            public UIBtnToggle toggleSFX;
            public UIBtnToggle toggleBGM;
        }



        [SerializeField] ResumeZone resumeZone;
        [System.Serializable]
        public class ResumeZone
        {
            public Transform root;
            public UILabel ui_uiCountDown;
        }










        bool pause;
        List<Transform> pages => new List<Transform>() { settingZone.root, resumeZone.root };
        public void Init(bool pause = false)
        {
            this.pause = pause;
            if (pause)
                GameControl.instance?.OnPause();

            settingZone.toggleSFX.IsValue = Sound.IsSfx;
            settingZone.toggleBGM.IsValue = Sound.IsBgm;
            pages.Open(settingZone.root);
        }
        public void OnSfx()
        {
            Sound.IsSfx = settingZone.toggleSFX.IsValue;
        }
        public void OnBgm()
        {
            Sound.IsBgm = settingZone.toggleBGM.IsValue;
        }
        IEnumerator DoResume()
        {
            if (!pause)
            {
                OnClose();
            }
            else
            {
                pages.Open(resumeZone.root);
                int countdown = 3;
                while (countdown != 0)
                {
                    resumeZone.ui_uiCountDown.text = countdown.ToString();
                    yield return new WaitForSecondsRealtime(1.0f);
                    countdown--;
                }
                GameControl.instance?.OnResume();
                OnClose();
            }
        }
        public void ClosePage()
        {
            StartCoroutine(DoResume());
        }



    }
}