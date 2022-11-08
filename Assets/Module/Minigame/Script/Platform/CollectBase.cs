using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public enum CollectType 
    { 
        Obstacle,Coin,Booster,Ground
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
        [SerializeField] float distanceComming = 5;
        bool IsHit;
        bool IsComming;

        protected virtual void OnBegin()
        {

        }
        protected virtual void OnHited(Player.PlayerData player)
        {

        }
        protected virtual void OnComming()
        {

        }
        public virtual void OnEnter()
        {

        }




        PlatformObj platform;
        public void Init(PlatformObj platform ) 
        {
            this.platform = platform;
            IsHit = false;
            IsComming = false;
            collider.enabled = true;
            gameObject.SetActive(true);

            OnUpdate = null;
            OnUpdate += DistanceComming;
            OnBegin();
        }
        void DistanceComming()
        {
            if (IsComming || hitEventType == HitEventType.Solid || distanceComming == 0.0f)
            {
                OnUpdate -= DistanceComming;
            }
            else
            {
                var dist = transform.position.x - Player.PlayerData.player.transform.position.x;
                if (dist > 0 && dist <= distanceComming)
                {
                    OnUpdate -= DistanceComming;
                    IsComming = true;
                    OnComming();
                }
            }
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
