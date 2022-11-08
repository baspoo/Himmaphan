using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class ObstacleObj : CollectBase
    {

        public int Damage;
        protected override void OnBegin()
        {

        }
        protected override void OnHited(Player.PlayerData player)
        {
            player.handle.AddDamage(Damage);
        }
        protected override void OnComming()
        {

        }
        public override void OnEnter()
        {

        }
    }
}