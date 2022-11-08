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
    }
}