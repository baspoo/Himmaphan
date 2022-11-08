using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace MiniGame {
    public class BackgroundObj : BasePool
    {

        public enum BackgroundType
        {
            Small,Big,View,Ground,Air,Cloud
        }

        public BackgroundType Type;
        [SerializeField] Vector2 size;
        [SerializeField] Vector2 hightLocation;
        [SerializeField] bool flip;
        [SerializeField] SpriteRenderer spriteRenderer;

        public BackgroundManager.Layer layer { get; private set; }
        public bool isActive => pool.isActive;
        public void Init(PlatformObj platform, BackgroundManager.Layer layer )
        {
            this.layer = layer;

            var location = Random.RandomRange(layer.range[0], layer.range[1]) + platform.transform.position.x;
            transform.position = new Vector3(location, Random.RandomRange(hightLocation.x,hightLocation.y), 0.0f) ;
            transform.parent = layer.root;
            if (flip) spriteRenderer.flipX = 100.Random() > 50 ? true : false;
            spriteRenderer.sortingOrder = layer.depth;
            transform.localScale = Vector3.one * Random.RandomRange(size.x, size.y);
        }


    }
}
