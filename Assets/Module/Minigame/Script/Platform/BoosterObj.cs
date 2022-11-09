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
        Turbo
    }
    [System.Serializable]
    public class BoosterData
    {
        public BoosterType BoosterType;
        public float Duration;
        public double Value;
        public System.DateTime Time;
        public Color Color;
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
        protected override void OnComming()
        {

        }
        public override void OnEnter()
        {

        }



        public BoosterRuntime HandleAction() 
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
                        p.handle.AddLifePoint((int)Data.Value);
                    };
                    break;
                case BoosterType.Turbo:
                    boosterRuntime.EventStart = (p) => {
                        p.stat.Speed = (float)(p.defaultStat.BeginSpeed * Data.Value);
                        GameControl.instance.background.ActiveRoadOfRainBow(true);
                    };
                    boosterRuntime.EventDone = (p) => {
                        p.stat.Speed = p.defaultStat.BeginSpeed;
                        GameControl.instance.background.ActiveRoadOfRainBow(false);
                    };
                    break;
                default:
                    break;
            }
            return boosterRuntime;
        }



    }
}