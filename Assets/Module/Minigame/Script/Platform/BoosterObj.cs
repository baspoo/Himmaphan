using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public enum BoosterType{
        Magnet,
        Immortal,
        X2Score,
        LifePoint,
        Boom
    }
    [System.Serializable]
    public class BoosterData
    {
        public BoosterType BoosterType;
        public float Duration;
        public int Value;
        public System.DateTime Time;
    }
    public class BoosterRuntime
    {
        public BoosterData Data;
        public float Duration;
        public System.DateTime Time;
        public System.Action<Player.PlayerData> EventStart;
        public System.Action<Player.PlayerData> EventUpdate;
        public System.Action<Player.PlayerData> EventDone;
        public Coroutine Coroutine;
    }






    public class BoosterObj : CollectBase
    {

        public BoosterData Data;

        protected override void OnBegin()
        {

        }
        protected override void OnHited(Player.PlayerData player)
        {
            var boosterRuntime = HandleAction();
            player.handle.AddBooster(boosterRuntime);
        }





        BoosterRuntime HandleAction() 
        {
            var boosterRuntime = new BoosterRuntime() {
                Data = Data,
                Duration = Data.Duration,
                Time = System.DateTime.Now,
            };
            switch (Data.BoosterType)
            {
                case BoosterType.Magnet:
                    boosterRuntime.EventStart = (p) => {

                    };
                    boosterRuntime.EventUpdate = (p) => {

                    };
                    boosterRuntime.EventDone = (p) => {

                    };
                    break;
                case BoosterType.Immortal:
                    boosterRuntime.EventStart = (p) => {

                    };
                    boosterRuntime.EventDone = (p) => {

                    };
                    break;
                case BoosterType.X2Score:
                    boosterRuntime.EventStart = (p) => {

                    };
                    boosterRuntime.EventDone = (p) => {

                    };
                    break;
                case BoosterType.LifePoint:
                    boosterRuntime.EventStart = (p) => {
                        p.handle.AddLifePoint(Data.Value);
                    };
                    break;
                case BoosterType.Boom:
                    boosterRuntime.EventUpdate = (p) => {

                    };
                    break;
                default:
                    break;
            }
            return boosterRuntime;
        }



    }
}