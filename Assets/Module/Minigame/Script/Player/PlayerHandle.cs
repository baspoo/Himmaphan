using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerHandle : MonoBehaviour
    {



        public Rigidbody2D rigi;
        public BoxCollider2D collider;
        public AudioSource andio;

        PlayerData playerdata;
        public void Init(PlayerData playerdata)
        {
            this.playerdata = playerdata;
        }



        void OnCollisionEnter2D(Collision2D coll) {
            playerdata.collect.Hit(coll.gameObject);
        }
        void OnTriggerEnter2D(Collider2D hit) {
            playerdata.collect.Hit(hit.gameObject);
        }
        void OnCollisionExit2D(Collision2D coll) {
          
        }
        void OnTriggerExit2D(Collider2D coll) { 
           
        }
        void OnCollisionStay2D(Collision2D coll) {
            //playerdata.collect.Hit(coll.gameObject);
        }
        void OnTriggerStay2D(Collider2D coll) {
            playerdata.collect.Hit(coll.gameObject);
        }
    }
}

