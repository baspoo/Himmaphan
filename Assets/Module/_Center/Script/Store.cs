using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utility
{
    public class Store : MonoBehaviour
    {
        static Store m_instance;
        public static Store instance {
            get {
                if (m_instance == null)
                    m_instance = ((GameObject)Resources.Load("System/Store")).GetComponent<Store>();
                return m_instance;
            }
        }
        public void Init()
        {
            instance.enabled = true;
        }



        public Pages page;
        [System.Serializable]
        public class Pages
        {
            public GameObject prefab_admin;
            public GameObject prefab_debug;
            public GameObject prefab_bubblePage;
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