using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public enum CollectType 
    { 
        Obstacle,Coin,Booster
    }
    public enum HitEventType
    {
        Collect, Solid, SolidEvent
    }
    public class CollectBase : BasePool
    {
        public string objectId;
        public CollectType type;
        public SpriteRenderer spriteRender;
        public Collider2D collider;
        public HitEventType hitEventType;
        bool IsHit;


        protected virtual void OnBegin()
        {

        }
        protected virtual void OnHited(Player.PlayerData player)
        {

        }

        PlatformObj platform;
        public void Init(PlatformObj platform ) 
        {
            this.platform = platform;
            IsHit = false;
            collider.enabled = true;
            gameObject.SetActive(true);
            OnBegin();
        }
        public void DisableCollider() 
        {
            collider.enabled = false;
        }


        public void OnHit( Player.PlayerData player )
        {
            if (!IsHit) 
            {  
                if (hitEventType == HitEventType.Collect) 
                {
                    IsHit = true;
                    collider.enabled = false;
                    gameObject.SetActive(false);
                    platform.RemoveCollect(this);
                }
                if (hitEventType == HitEventType.SolidEvent)
                {
                    IsHit = true;
                }
                if (hitEventType == HitEventType.Solid)
                {

                }
                OnHited(player);
            }
        }

    }
}
