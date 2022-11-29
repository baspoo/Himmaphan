using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Interactive.CameraControl
{
    public class UserInput : MonoBehaviour
    {




        public class MoveInputType
        {
            public bool Valid;
            public bool Held;
        }

        public DeviceType deviceType;
        [SerializeField] KeyCode moveKey;
        Camera playerCamera => CameraEngine.instance.maincamera;
        public bool isEnabled;
        public bool warp;
        //public bool stunned;
        //public float destinationTreshold = 0.25f;
        [SerializeField] LayerMask groundLayers;
        [SerializeField] LayerMask ignoreLayers;
        [SerializeField] LayerMask uiLayerMask;
        [SerializeField] float maxGroundRaycastDistance = 100;
        //public float minimumPathDistance = 0.5f;
        //public float samplePositionDistanceMax = 5f;
        [SerializeField] float deltaSpiningForSkip = 0.5f;
        bool allowHoldKey = true;
        float holdMoveCd = 0.1f, nextHoldMove;
        float dragThreshold = 0.15f;
        float holdTime;
        public System.Action<GameObject, Vector3, bool> onInput;
        int layerMask;
        public void Init()
        {

            #if !UNITY_EDITOR
            deviceType = Application.isMobilePlatform ? DeviceType.Mobile : DeviceType.Web;
            #endif

            isEnabled = true;
            layerMask = 1 << LayerMask.NameToLayer("UI");
            allowHoldKey = deviceType == DeviceType.Web;
            if (deviceType == DeviceType.Mobile)
            {
                moveKey = KeyCode.Mouse0;
                UIHover.Enable = false;
            }
        }
        public void Update()
        {
            if (!isEnabled || CameraEngine.IsStopCameraEngine)
                return;

            if (deviceType == DeviceType.Web) 
                UpdateInputWeb();
            else 
                UpdateInputMobile();
        }







        public bool IsCanInput
        {
            get
            {
                if (deviceType == DeviceType.Mobile)
                {
                    return !IsUiOverlapping();
                }
                else 
                {
                    return UIHover.Hover;
                }
            }
        }









        #region Web
        public void UpdateInputWeb() 
        {
            if (!IsCanInput)
                return;

            MoveInputType moveInputType = MovingInput();
            DoMoving(moveInputType);
        }
        private MoveInputType MovingInput()
        {
            MoveInputType moveInputType = new MoveInputType();
            if (Input.GetKey(moveKey))
            {
                holdTime += Time.deltaTime;
            }
            else if (Input.GetKeyUp(moveKey))
            {
                moveInputType.Valid = holdTime < dragThreshold;
                holdTime = 0f;
                return moveInputType;
            }
            if (!(Time.time >= nextHoldMove))
            {
                moveInputType.Valid = false;
                return moveInputType;
            }
            if (!allowHoldKey || !Input.GetKey(moveKey)) 
                return moveInputType;
            nextHoldMove = Time.time + holdMoveCd;
            moveInputType.Valid = true;
            moveInputType.Held = true;
            return moveInputType;
        }
        #endregion






        #region Mobile
        public void UpdateInputMobile()
        {
            if (Vector2.Distance(CameraEngine.instance.cameraCtr.deltaSpining, Vector2.zero) > deltaSpiningForSkip)
            {
                skipwalk = true;
            }
            if (Input.GetKeyDown(moveKey))
            {
                if (!IsUiOverlapping()) 
                {
                    StartCoroutine(DoClickToMove());
                }
            }
        }
        bool IsUiOverlapping() 
        {
            if (Input.touches.Length == 0) return false;

            Touch mainTouch = Input.touches[0];
            Ray ray = UICamera.currentCamera.ScreenPointToRay(mainTouch.position);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100, uiLayerMask);
            int count = hits.Length;
            Debug.Log($"UiOverlapping : {count}");
            return count > 0;
        }
        bool skipwalk;
        IEnumerator DoClickToMove()
        {
            skipwalk = false;
            yield return new WaitForSeconds(0.12f);
            if (!IsUiOverlapping() && !skipwalk)
                DoMoving(new MoveInputType()
                {
                    Valid = true,
                    Held = false
                });
        }
        #endregion
































        void DoMoving(MoveInputType moveInputType)
        {
            if (!moveInputType.Valid)
            {
                //Debug.LogError("!Valid");
                return;
            }
            if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var ignore, maxGroundRaycastDistance, ignoreLayers))
            {
                Debug.LogError($"Not Hit! {groundLayers} {maxGroundRaycastDistance}");
                return;
            }
            if (!Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var hit, maxGroundRaycastDistance, groundLayers))
            {
                Debug.LogError($"Not Hit! {groundLayers} {maxGroundRaycastDistance}");
                return;
            }
            var destination = hit.point;
            onInput?.Invoke(hit.collider.gameObject, destination, moveInputType.Held);
        }
    }
}