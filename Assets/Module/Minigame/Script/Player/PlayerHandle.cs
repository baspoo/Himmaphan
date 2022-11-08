using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame.Player
{
    public class PlayerHandle : PlayerBase
    {



        public Rigidbody2D rigi;
        public BoxCollider2D collider;
        public AudioSource andio;

        //PlayerData playerdata;
        public void Init(PlayerData playerdata)
        {
            this.playerdata = playerdata;
        }






        public void OnJump() 
        {
            playerdata.move.OnJump();
        }
        public void OnSlide()
        {
            playerdata.move.OnSlide();
        }
        public void OnStopSlide()
        {
            playerdata.move.OnStopSlide();
        }




        void OnCollisionEnter2D(Collision2D coll) {
            Hit(coll.gameObject);
        }
        void OnTriggerEnter2D(Collider2D hit) {
            Hit(hit.gameObject);
        }
        void OnCollisionExit2D(Collision2D coll) {
          
        }
        void OnTriggerExit2D(Collider2D coll) { 
           
        }
        void OnCollisionStay2D(Collision2D coll) {
            //playerdata.collect.Hit(coll.gameObject);
        }
        void OnTriggerStay2D(Collider2D coll) {
            Hit(coll.gameObject);
        }
        public void Hit(GameObject hit)
        {

            Debug.Log($"Player-Hit : {hit.name} - {hit.tag}");
            switch (hit.tag)
            {
                case "Obstacle":
                case "Coin":
                case "Booster":
                    var collect = hit.GetComponent<CollectBase>();
                    if (collect != null)
                        collect.OnHit(playerdata);
                    break;

                case "Ground":
                    break;

                case "DeadZone":
                    AddDamage(-1);
                    break;

                default:
                    break;
            }
        }








        #region AddScore
        public void AddScore(int score) 
        {
            if (!playerdata.isReady || playerdata.stat.isDead) return;

            var buff = playerdata.buffbooster.GetBooster(BoosterType.X2Score);
            if (buff != null) 
            {
                score = score * buff.Data.Value;
            }

            playerdata.stat.Score += score;
        }
        #endregion








        #region AddBooster
        public void AddBooster(BoosterRuntime boosterRuntime)
        {
            if (!playerdata.isReady || playerdata.stat.isDead) return;

            var boosterType = boosterRuntime.Data.BoosterType;
            var oldrunTime = playerdata.buffbooster.GetBooster(boosterType);

            //** remove
            if (oldrunTime != null) 
            {
                if (oldrunTime.Coroutine != null)
                    StopCoroutine(oldrunTime.Coroutine);
                oldrunTime.EventDone?.Invoke(playerdata);
                playerdata.buffbooster.Remove(oldrunTime);
            }

            //** newbuff
            boosterRuntime.Coroutine = StartCoroutine(BuffRuntime(boosterRuntime));
        }
        IEnumerator BuffRuntime(BoosterRuntime boosterRuntime)
        {
            playerdata.buffbooster.Add(boosterRuntime);
            boosterRuntime.EventStart?.Invoke(playerdata);
            while (boosterRuntime.Duration <= 0.0f)
            {
                yield return new WaitForEndOfFrame();
                if (!playerdata.isReady || playerdata.stat.isDead) yield break;

                boosterRuntime.EventUpdate?.Invoke(playerdata);
                boosterRuntime.Duration -= Time.deltaTime;
            }
            boosterRuntime.EventDone?.Invoke(playerdata);
            playerdata.buffbooster.Remove(boosterRuntime);
        }
        #endregion










        #region AddDamage
        public void AddDamage(int damage)
        {
            if (!playerdata.isReady || playerdata.stat.isDead) return;

            if (damage == -1)
            {
                //** Dead
                OnDead();
            }
            else 
            {
                if (playerdata.buffbooster.IsActiveBooster(BoosterType.Immortal))
                    return;

                playerdata.stat.Hp -= damage;
                if (playerdata.stat.Hp <= 0)
                {
                    //** Dead
                    OnDead();
                }
            }
        }
        public void AddLifePoint(int hp)
        {
            if (!playerdata.isReady || playerdata.stat.isDead) return;

            playerdata.stat.Hp += hp;
            if (playerdata.stat.Hp > playerdata.defaultStat.MaxHp)
                playerdata.stat.Hp = playerdata.defaultStat.MaxHp;
        }
        #endregion












        #region Dead
        public void OnDead()
        {
            if (!playerdata.stat.isDead)
            {
                playerdata.stat.isDead = true;
                StartCoroutine(DoDead());
            }
        }
        IEnumerator DoDead()
        {
            GameControl.instance.GameOver();
            if (playerdata.move.IsFlying)
            {
                yield return new WaitForSeconds(5.0f);
                playerdata.gameObject.SetActive(false);
            }
            else
            {
                playerdata.anim.OnDead();
            }
        }
        #endregion




    }
}

