using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class CoinObj : CollectBase
    {

        public int Score;
        protected override void OnBegin()
        {
 
        }
        protected override void OnHited(Player.PlayerData player)
        {
            player.handle.AddScore(Score);
        }
        protected override void OnComming()
        {
            deltra = 0.0f;
            if(Player.PlayerData.player.buffbooster.active.IsMagnet)
                OnUpdate += MoveToPlayer;
        }
        public override void OnEnter()
        {

        }










        float deltra;
        void MoveToPlayer() 
        {
            deltra += Time.deltaTime*0.5f;
            transform.position = Vector3.MoveTowards(transform.position, Player.PlayerData.player.transform.position, deltra);
        }



    }
}