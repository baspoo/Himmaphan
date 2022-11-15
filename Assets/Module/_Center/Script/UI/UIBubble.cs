using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Center
{
    public class UIBubble : MonoBehaviour
    {


        public bool isActive => pool != null ? pool.isActive : false;

        public UITexture imgIcon;
        public UITexture imgBg;
        public Transform popup;
        public string group;
        public Color color_stateCompleted_top;
        public Color color_stateCompleted_bottom;

        public Material mat_blink;

        PoolObj pool;
        [SerializeField] Transform target3D;
        System.Action action;
        ScreenPoint.Looking look;
        UIBubblePage page;
        public void Init(UIBubblePage page, Texture Icon, Transform target3D, System.Action action)
        {
            pool = GetComponent<PoolObj>();
            this.page = page;
            this.target3D = target3D;
            this.action = action;
            imgIcon.mainTexture = Icon;
            look = page.root.screenPoint.AddLooking(transform, target3D);
        }
        [SerializeField] float distance;
        [SerializeField] float size;
        private void Update()
        {
            imgBg.depth = ((int)transform.localPosition.y) * -1;
            imgIcon.depth = imgBg.depth + 1;
            popup.localScale = Vector3.one * calculateDistance;
        }


        float calculateDistance
        {
            get
            {


                //if (Player.instance != null)
                //{
                //    var distance = Vector3.Distance(target3D.position, Player.instance.transform.position);
                //    //ToDo : calculate Distance for adjust size......
                //}


                return 1.0f;
            }

        }
        public void OnSubmit()
        {
            //Playlist.let.sfx_click.Play();
            action?.Invoke();
        }
        public void OnCloseBubble()
        {
            if (look != null)
                page.root.screenPoint.RemoveLooking(look);
            pool.Deactive();
            page.Remove(this);
        }


    }
}