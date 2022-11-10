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
        public UITexture ui_imgHpPilot;
        public UIGrid ui_gridBooster;
        public GameObject prefabBooster;
        public Color[] ColorsPiliot;
        public float speedHp = 1.0f;

        public void Init() 
        {
            UpdateHp();
            UpdateScore();
        }
        public void ClosePage()
        {
            OnClose();
        }
        private void Update()
        {
     
        }














        public void UpdateHp()
        {
            var fill = (float)Player.PlayerData.player.stat.Hp / (float)Player.PlayerData.player.defaultStat.MaxHp;
            if (fill == lastHp)
                return;

            if (coroUpdateHp != null) StopCoroutine(coroUpdateHp);
            coroUpdateHp = StartCoroutine(DoUpdateHp(fill));
            
        }

        Coroutine coroUpdateHp;
        float lastHp = 0;
        IEnumerator DoUpdateHp(float hp) 
        {
            var plus = hp > lastHp ? true : false;
            lastHp = hp;
            ui_imgHpPilot.color = ColorsPiliot[plus ? 0 : 1];

            Debug.Log($"DoUpdateHp : plus-{plus}");

            if (plus)
            {
                ui_imgHpPilot.fillAmount = hp;
                yield return new WaitForSeconds(0.5f);
                while (ui_imgHp.fillAmount != hp)
                {
                    yield return new WaitForEndOfFrame();
                    ui_imgHp.fillAmount += Time.deltaTime * speedHp;
                    if (ui_imgHp.fillAmount > hp)
                        ui_imgHp.fillAmount = hp;
                }
            }
            else 
            {
                ui_imgHp.fillAmount = hp;
                yield return new WaitForSeconds(0.5f);
                while (ui_imgHpPilot.fillAmount != hp)
                {
                    yield return new WaitForEndOfFrame();
                    ui_imgHpPilot.fillAmount -= Time.deltaTime * speedHp;
                    if (ui_imgHpPilot.fillAmount < hp)
                        ui_imgHpPilot.fillAmount = hp;
                }
            }


           




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