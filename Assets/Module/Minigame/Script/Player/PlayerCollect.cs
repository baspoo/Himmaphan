using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerCollect
    {
        PlayerData playerdata;
        public PlayerCollect(PlayerData playerdata) 
        {
            this.playerdata = playerdata;
        }


        public void Hit(GameObject hit) 
        {

            Debug.Log($"Player-Hit : {hit.name} - {hit.tag}");
            switch (hit.tag)
            {
                case "Obstacle":
                case "Coin":
                case "Booster":
                    var collect = hit.GetComponent<CollectObj>();
                    if (collect != null)
                        collect.OnHit();
                    break;
                default:
                    break;
            }
        }


    }
}