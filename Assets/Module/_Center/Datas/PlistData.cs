using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Data
{
    [System.Serializable]
    public class PlistData
    {
        public static PlistData plist;




        public string version;
        public ApiData apiData;
        [System.Serializable]
        public class ApiData
        {
            public string api_getEventList = "http://smartobm-vsr-uat.thaitrade.com/api/public/events";
            public string api_getEventDetail = "http://smartobm-vsr-uat.thaitrade.com/api/public/events/@";
            public string api_getProductList = "http://smartobm-vsr-uat.thaitrade.com/api/public/sellers/@/products";
        }

        public ConfigData config;
        [System.Serializable]
        public class ConfigData
        {
            public float playerMoveSpeed;
            public float cameraFieldOfView;
            public float pitchSensitivity;
            public float yawSensitivity;
            public float guiScaleFactor;
            public int guiWidth;
            public int guiHeight;
            public bool protectImageFail;
        }
        public Dictionary<string, string> languages = new Dictionary<string, string>();
    }

}
