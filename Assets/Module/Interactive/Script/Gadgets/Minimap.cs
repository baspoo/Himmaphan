using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactive
{
    public class Minimap : MonoBehaviour
    {




        public void Init()
        {
            //hight = transform.position.y;
            map.gameObject.SetActive(true);
        }

        public float hight;
        public Transform arrow;
        public Transform map;
        public Transform m_target;
        public void OnStart(Transform target)
        {
            m_target = target;
        }
        public void OnStop()
        {
            m_target = null;
        }
        void Update()
        {
            if (m_target != null)
            {
                var pos = m_target.position;
                pos.y = hight;
                transform.position = pos;


                var r = transform.localRotation.eulerAngles;
                var rotate =   CameraControl.CameraEngine.instance.cameraCtr.drag.RootRotate.localRotation.eulerAngles;
                rotate.x = r.x;
                rotate.z = r.z;
                transform.localRotation = Quaternion.Euler(rotate);
                arrow.rotation =   Player.PlayerClient.client.transform.rotation;

            }
        }
    }
}