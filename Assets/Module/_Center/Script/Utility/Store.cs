using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Center
{
    public class Store : MonoBehaviour
    {
        static Store m_instance;
        public static Store instance {
            get 
            {
                return m_instance;
            }
        }
        public static Store load
        {
            get
            {
                return ((GameObject)Resources.Load("CenterStore")).GetComponent<Store>(); ;
            }
        }
        public static Store Create()
        {
            if (m_instance == null)
            {
                m_instance = ((GameObject)Instantiate(load.gameObject)).GetComponent<Store>();
                m_instance.Init();
                m_instance.name = "CenterStore";
                DontDestroyOnLoad(m_instance.gameObject);
            }
            return m_instance;
        }
        public void Init()
        {
            instance.enabled = true;
            link.loadcenter.Init();
            link.sound.Init();
        }


        public Link link;
        [System.Serializable]
        public class Link
        {
            public LoadCenter loadcenter;
            public Sound sound;
        }




        public Pages page;
        [System.Serializable]
        public class Pages
        {
            public GameObject prefab_admin;
            public GameObject prefab_debug;
            public GameObject prefab_popupPage;
        }


        public Image image;
        [System.Serializable]
        public class Image
        {
            public Texture defaultImage;
        }














    }
}
