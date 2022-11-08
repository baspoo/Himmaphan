using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerData : MonoBehaviour
    {

        [System.Serializable]
        public class Stat 
        {
            public int Hp;
            public int Score;
            public float Speed;
        }



        public Stat stat;
        public PlayerMove move;
        public PlayerHandle handle;
        public PlayerAnimation anim;
        public PlayerCollect collect;




        private void Awake()
        {
            Init();
        }
        public void Init()
        {
            move.Init(this);
            handle.Init(this);
            anim.Init(this);
            collect = new PlayerCollect(this);
        }




    }
}

