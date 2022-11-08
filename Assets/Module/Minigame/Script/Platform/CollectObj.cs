using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class CollectObj : MonoBehaviour
    {
        public SpriteRenderer spriterender;
        public Collider2D collider;
        public bool IsHit;
        public void OnHit( )
        {
            if (!IsHit) 
            {
                Debug.Log($"Collect-Hit : {name}");
                IsHit = true;
                collider.enabled = false;
                spriterender.enabled = false;
            }
        }

    }
}
