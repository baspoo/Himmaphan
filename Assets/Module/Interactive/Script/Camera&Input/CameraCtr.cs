using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeviceType 
{
    Web,Mobile
}

namespace Interactive.CameraControl
{
    public class CameraCtr : MonoBehaviour
    {


        [SerializeField] KeyCode rotateKey;
        [SerializeField] UserInput input;
        public bool IsEnable;
        public bool IsIgnoreRotate;
        public bool IsIgnoreZoom;
        public Transform t_spin;
        public Transform t_zoom;
        public DragPitch.P3dDragPitchYaw drag;
        [SerializeField] PinchZoomListener pinchZoomListener;
        public bool isCameraSpining => drag.IsChanging;
        public Vector2 deltaSpining => drag.delta;
        Vector3 _mouseReference;
        Vector3 _mouseOffset;
        Vector3 _rotation;
        Vector3 _rotate, _def_rotate;
        Data.PlistData.Interactive.Input.DeviceData inputDevice;
        public bool isRotating => _isRotating;
        bool _isRotating;
        public Vector2 m_speed;
        float zoomStart;
        float _pitchMax;
        public float zoom;
        float zoomspeed;
        float[] zoomlimit = new float[2];
        float _runzoom, _defzoom;
        public List<ZoomLimit> zoomlimits;
        [System.Serializable]
        public class ZoomLimit
        {
            public string mode;
            public Vector3 beginRotate;
            public float zoomStart;
            public float zoomspeed;
            public float[] zoomlimit = new float[2];
            public bool isIgnoreZoom;
            public bool isIgnoreRotate;
            public bool isIgnoreEffect;
        }
        Transform rootRotate
        {
            get
            {
                return drag.RootRotate;
            }
        }
        bool m_IsIgnoreRotate => IsIgnoreRotate || (_currentzoom != null) ? _currentzoom.isIgnoreRotate : false;
        bool m_IsIgnoreZoom => IsIgnoreZoom || (_currentzoom != null) ? _currentzoom.isIgnoreZoom : false;

        public SpinType spinType;
        public enum SpinType { None, X, Y }
        public void SetupLimitIndex(int index, bool reset)
        {
            var _zoom = zoomlimits[index];
            SetupLimitIndex(_zoom, reset);
        }
        ZoomLimit _currentzoom;
        public void SetupLimitIndex(ZoomLimit _zoom, bool reset)
        {
            _currentzoom = _zoom;
            zoomStart = _zoom.zoomStart;
            zoomspeed = _zoom.zoomspeed;
            zoomlimit = _zoom.zoomlimit;
            _def_rotate = _zoom.beginRotate;
            _defzoom = zoomStart;
            CameraEngine.instance.rootEffect.SetActive(!_zoom.isIgnoreEffect);
            if (reset) OnReset(true);
        }
        bool init = false;
        public void Init()
        {
            init = true;
            inputDevice = input.deviceType == DeviceType.Web ? Data.PlistData.plist.interactive.input.web : Data.PlistData.plist.interactive.input.mobile;
            zoom = zoomStart;
            _runzoom = zoom;
            _defzoom = zoom;
            _rotation = Vector3.zero;
            _def_rotate = rootRotate.localRotation.eulerAngles;
            _pitchMax = drag.PitchMax;

            if (input.deviceType == DeviceType.Mobile)
            {
                pinchZoomListener.OnZoom += onPinchZoom;
            }
            drag.PitchSensitivity = inputDevice.spinSensitivity;
            drag.YawSensitivity = inputDevice.spinSensitivity;
        }

        public void OnToNearCenter()
        {
            zoom = zoomlimit[0];
            _runzoom = zoom;
            _rotate = new Vector3(-20, drag.currentYaw, 0);
            drag.PitchMax = 0;
            onrotate(_rotate);
            dozoom();
        }
        public void OnReset(bool snapRotate = false)
        {
            drag.PitchMax = _pitchMax;
            zoom = _defzoom;
            _runzoom = _defzoom;
            _rotate = _def_rotate;

            if (snapRotate)
            {
                drag.currentPitch = _rotate.x;
                drag.currentYaw = _rotate.y;
            }

            onrotate(_rotate);
            dozoom();
        }





        void Update()
        {
            if (!init)
                return;

            if (CameraEngine.IsStopCameraEngine)
                return;

            if (isStartStep)
            {
                updatestartstep();
            }
            else
            {
                if (!input.IsCanInput)
                {
                    clearrotate();
                    return;
                }
                onInputzoom();
                onrotate();
            }
        }




        bool isStartStep;
        void updatestartstep()
        {
            if (!isZooming)
                isStartStep = false;
            onrotate(_rotate);
            drag.currentPitch = _rotate.x;
            drag.currentYaw = _rotate.y;
            dozoom();
        }





        void onInputzoom()
        {
            if (m_IsIgnoreZoom)
                return;

            float f = Input.mouseScrollDelta.y;
            if (Data.PlistData.plist != null)
            {
                f *= inputDevice.zoomSensitivity;
            }
            zoom += f;
            dozoom();
        }
        void onPinchZoom(float f)
        {
            if (m_IsIgnoreZoom)
                return;

            if (Data.PlistData.plist != null)
            {
                f *= inputDevice.zoomSensitivity;
            }
            zoom -= f;
            dozoom();
        }
        void dozoom()
        {
            if (zoom < zoomlimit[0])
                zoom = zoomlimit[0];
            if (zoom > zoomlimit[1])
                zoom = zoomlimit[1];
            _runzoom = Mathf.Lerp(_runzoom, zoom, Time.deltaTime * zoomspeed);
            t_zoom.transform.localPosition = new Vector3(0.0f, _runzoom, _runzoom * -1.0f);

            //UI
            var val = _runzoom * 0.05f;
            target_uizoomsize = 1.0f - val;
            if (target_uizoomsize > 1.0f) target_uizoomsize = 1.0f;
            if (target_uizoomsize < 0.5f) target_uizoomsize = 0.5f;
            uizoomsize = target_uizoomsize;// Mathf.Lerp(uizoomsize, target_uizoomsize, Time.deltaTime * zoomspeed);
        }

        float target_uizoomsize;
        public float uizoomsize;

        bool isZooming => Mathf.Abs(_runzoom - zoom) > 0.35f;



        [SerializeField] bool _ismodifyOffset;
        Vector2 bestScreen = new Vector2(1920.0f, 1080.0f);
        Vector2 ratioScreen => _ismodifyOffset ? new Vector2(bestScreen.x / Screen.width, bestScreen.y / Screen.height) : Vector2.one;
        [SerializeField] bool _ismodifymousePosition;
        Vector3 mousePosition => _ismodifymousePosition ? new Vector3(Input.mousePosition.x * ratioScreen.x, Input.mousePosition.y * ratioScreen.y, Input.mousePosition.z) : Input.mousePosition;


        void clearrotate()
        {
            spinType = SpinType.None;
            drag.CanRotate = false;
            _isRotating = false;
        }
        void onrotate(Vector3 rotate)
        {
            drag.Pitch = rotate.x;
            drag.Yaw = rotate.y;
        }
        void onrotate()
        {

            if (Input.touchCount > 1)
                return;


            if (!IsEnable)
                return;

            if (Input.GetKeyDown(rotateKey))
            {
                // rotating flag
                _isRotating = true;

                // store mouse
                spinType = SpinType.None;
            }
            if (Input.GetKeyUp(rotateKey))
            {
                // rotating flag
                _isRotating = false;
            }
            if (!Input.GetKey(rotateKey))
            {
                // rotating flag
                _isRotating = false;
            }
            drag.CanRotate = _isRotating && !m_IsIgnoreRotate;
            if (_isRotating)
            {
                bool x = Mathf.Abs(_mouseOffset.x) < Mathf.Abs(_mouseOffset.y);
                SpinType change = SpinType.None;
                if (x)
                {
                    change = SpinType.X;
                }
                else
                {
                    change = SpinType.Y;
                }
                if (drag.IsChanging)
                    spinType = change;
            }
        }




    }
}