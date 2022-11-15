using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace Interactive.CameraControl
{
    public class PinchZoomListener : MonoBehaviour
    {
        #region Singleton Pattern
        public static PinchZoomListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PinchZoomListener>();
                }
                return _instance;
            }
        }
        private static PinchZoomListener _instance;
        #endregion

        public UnityAction<float> OnZoom;

        private void Update()
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevMag = (touch1PrevPos - touch2PrevPos).magnitude;
                float cMag = (touch1.position - touch2.position).magnitude;

                float diffMag = cMag - prevMag;

                OnZoom?.Invoke(diffMag * Time.deltaTime);
            }
        }
    }
}
