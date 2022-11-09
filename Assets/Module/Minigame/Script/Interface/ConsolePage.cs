using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class ConsolePage : UIBase
    {
        public static ConsolePage instance;
        public static ConsolePage Open( )
        {
            instance = CreatePage<ConsolePage>(GameStore.instance.page.prefab_consolePage);
            instance.Init();
            return instance;
        }







        public UILabel ui_lbScore;
        public UITexture ui_imgHp;
        public UIGrid ui_gridBooster;
        public GameObject prefabBooster;



        public void Init() 
        {
            UpdateHp();
            UpdateScore();
        }
        public void ClosePage()
        {
            OnClose();
        }


        public void UpdateHp()
        {
            var fill = (float)Player.PlayerData.player.stat.Hp / (float)Player.PlayerData.player.defaultStat.MaxHp;
            ui_imgHp.fillAmount = fill;
        }
        public void UpdateScore()
        {

            ui_lbScore.text = Player.PlayerData.player.stat.Score.ToString("#,##0");
        }
        public void AddBuff(BoosterRuntime runtime)
        {
            var buff = prefabBooster.Create(ui_gridBooster.transform).GetComponent<UIObj>();
            //buff.uiIcon.mainTexture = null;
            buff.uiTop.color = runtime.Data.Color;
            buff.OnUpdate = () => {
                if (runtime.Duration == 0.0f)
                {
                    Destroy(buff.gameObject);
                    RefreshBuff();
                }
                else
                {
                    buff.uiTop.fillAmount = runtime.Duration / runtime.Data.Duration;
                }
            };
            RefreshBuff();
        }
        void RefreshBuff() 
        {
            RefreshTime(()=> { ui_gridBooster.repositionNow = true; });
        }



        public void OnBtnJump()
        {
            GameControl.instance.input.OnBtnJump();
        }
        public void OnBtnSlide()
        {
            GameControl.instance.input.OnBtnSlide();
        }
        public void OnBtnStopSlide()
        {
            GameControl.instance.input.OnBtnStopSlide();
        }



        public void OnPause()
        {
            SettingPage.Open(true);
        }

    }
}