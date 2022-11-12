using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerBase : MonoBehaviour 
    {
        protected PlayerData playerdata;
        protected bool isReay => playerdata == null ? false : playerdata.isReady;
    }


    public class PlayerData : MonoBehaviour
    {


        public static PlayerData player;



        [System.Serializable]
        public class Stat 
        {
            public int Hp;
            public int Score;
            public int TopScore;
            public float Speed;
            public bool isDead;
            public void Init(PlayerData player)
            {
                Hp = player.defaultStat.MaxHp;
                Speed = player.defaultStat.BeginSpeed;
                Score = 0;
                isDead = false;
            }
        }
        [System.Serializable]
        public class DefaultStat
        {
            public int MaxHp;
            public float BeginSpeed;
        }






        [System.Serializable]
        public class BuffBooster
        {
            public QuickShort active = new QuickShort();
            public class QuickShort 
            {
                public QuickShort() {
                    IsMagnet = false;
                    IsImmortal = false;
                    X2ScoreValue = 1;
                }
                public bool IsMagnet;
                public bool IsImmortal;
                public bool IsTurbo;
                public double X2ScoreValue;
                public bool IsProtectHp 
                {
                    get 
                    {
                        return IsImmortal || IsTurbo;
                    }
                }

                public void Set(BoosterRuntime data , bool active ) 
                {
                    switch (data.Data.BoosterType)
                    {
                        case BoosterType.Magnet:
                            IsMagnet = active;
                            break;
                        case BoosterType.Immortal:
                            IsImmortal = active;
                            break;
                        case BoosterType.X2Score:
                            X2ScoreValue = active?data.Data.Value : 1;
                            break;
                        case BoosterType.LifePoint:
                            break;
                        case BoosterType.Turbo:
                            IsTurbo = active;
                            break;
                        default:
                            break;
                    }
                }
            }


            List<BoosterRuntime> Boosters = new List<BoosterRuntime>();
            public void Init() {
                Boosters = new List<BoosterRuntime>();
            }
            public void Add(BoosterRuntime data)
            {
                Remove(data);
                Boosters.Add(data);
                active.Set(data,true);
            }
            public void Remove(BoosterRuntime data)
            {
                active.Set(data, false);
                Boosters.RemoveAll(x=>x.Data.BoosterType == data.Data.BoosterType);
            }
            public bool IsActiveBooster(BoosterType type)
            {
                if (Boosters.Count == 0) return false;
                return GetBooster(type) != null;
            }
            public BoosterRuntime GetBooster(BoosterType type)
            {
                if (Boosters.Count == 0) return null;
                return Boosters.Find(x => x.Data.BoosterType == type);
            }
        }








       
        public Stat stat;
        public DefaultStat defaultStat;
        public BuffBooster buffbooster;
        public PlayerMove move;
        public PlayerHandle handle;
        public PlayerAnimation anim;
        public bool isReady { get; private set; }



        public void Init()
        {
            player = this;
            isReady = false;
            stat.Init(this);
            buffbooster.Init();
            move.Init(this);
            handle.Init(this);
            anim.Init(this);
            gameObject.SetActive(false);
        }
        public void StartGame()
        {
            gameObject.SetActive(true);
            isReady = true;
            move.OnRun();
        }
        public void GameOver()
        {
            handle.collider.enabled = false;
            handle.rigi.simulated = false;
            move.OnStopRun();
            anim.OnDead();
            isReady = false;
        }












    }
}

