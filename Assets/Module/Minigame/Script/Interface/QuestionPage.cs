using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class QuestionPage : UIBase
    {
        public static QuestionPage instance;

        public static QuestionPage Open( int questCount ,System.Action done )
        {
            instance = CreatePage<QuestionPage>(GameStore.instance.page.prefab_questionPage);
            instance.Init(questCount, done);
            return instance;
        }



        [SerializeField] QuestPopup questPopup;
        [System.Serializable]
        public class QuestPopup
        {
            public Transform root;
            public UILabel ui_lbHeader;
            public UILabel ui_lbMessage;
            public UILabel ui_lbAmount;
            public UIGrid ui_gridAnswer;
            public GameObject prefabAnswer;
            public ReAwake reawake;
        }

        [SerializeField] ResultPopup resultPopup;
        [System.Serializable]
        public class ResultPopup
        {
            public Transform root;
            public UILabel ui_lbScore;
            public UILabel ui_lbBonus;
        }

        List<Transform> popup => new List<Transform>() { questPopup.root , resultPopup.root };
        int questCount;
        int questIndex;
        int answerCount;
        bool isReadyToAnswer;
        System.Action done;
        List<UIObj> uiObjs = new List<UIObj>();
        List<Data.PlistData.Minigame.Question> questions = new List<Data.PlistData.Minigame.Question>();






        public void Init(int questCount, System.Action done)
        {
            this.questCount = questCount;
            this.done = done;
           
            StartCoroutine(DoInit());
        }


       
        IEnumerator DoInit() 
        {
            //** Init Order List-Quest
            isReadyToAnswer = false;
            popup.Close();
            questIndex = 0;
            questions = new List<Data.PlistData.Minigame.Question>();
            foreach (var index in Data.PlistData.plist.minigame.questions.Count.Shuffle()) 
            {
                if(questions.Count < questCount) 
                    questions.Add(Data.PlistData.plist.minigame.questions[index]);
            }

            //** Start Quest
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(Next());
        }




        IEnumerator Next()
        {
            //** Init Next-Quest
            isReadyToAnswer = false;
            popup.Close( );
            var quest = questions[questIndex];
            questPopup.ui_lbAmount.text = $"{questIndex+1}/{questions.Count}";
            questPopup.ui_lbMessage.text = quest.quest;
            uiObjs.ForEach(x=>x.Destroy());
            foreach (var index in quest.choices.Length.Shuffle()) 
            {
                var uiobj =  root.UIPool( questPopup.prefabAnswer , questPopup.ui_gridAnswer.transform ).GetComponent<UIObj>();
                uiobj.uiName.text = quest.choices[index];
                uiobj.uiName.color = Color.white;
                uiobj.IsActive = (index+1) == quest.answer;
                uiobj.onSumbit = OnAnswer;
                uiObjs.Add(uiobj);
            }
            uiObjs.ForEach(b=>b.btn.isEnabled = false);
            questPopup.ui_gridAnswer.repositionNow = true;
            yield return new WaitForSeconds(0.5f);


            //** Ready Quest
            popup.Open(questPopup.root);
            isReadyToAnswer = true;
            uiObjs.ForEach(b => b.btn.isEnabled = true);
        }
        void OnAnswer( UIObj uiobj) 
        {
            if(isReadyToAnswer) 
                StartCoroutine(DoAnswer(uiobj));
        }
        IEnumerator DoAnswer(UIObj uiobj)
        {

            if (uiobj.IsActive)
            {
                //** Answer : Ture
                answerCount++;
                uiobj.uiName.color = Color.green;
            }
            else
            {
                //** Answer : False
                uiobj.uiName.color = Color.red;
            }




            //** Next or Done
            yield return new WaitForSeconds(0.5f);
            questIndex++;
            if (questIndex < questions.Count)
            {
                StartCoroutine(Next());
            }
            else
            {
                StartCoroutine(Done());
            }
        }

        IEnumerator Done()
        {
            //** Done
            popup.Close();
            yield return new WaitForSeconds(0.5f);
            var bonusPoint = answerCount * Data.PlistData.plist.minigame.tuning.quiz.bonusPoint;
            resultPopup.ui_lbScore.text = $"Score {Player.PlayerData.player.stat.Score.ToString("#,##0")}";
            resultPopup.ui_lbBonus.text = $"Bonus +{bonusPoint.ToString("#,##0")}";
            Player.PlayerData.player.handle.AddBonusScore(bonusPoint);
            popup.Open(resultPopup.root);
        }
        public void OnClosePage() 
        {
            OnClose();
            done?.Invoke();
        }


    }
}
