using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UserInput : MonoBehaviour
{


    public class MoveInputType
    {
        public bool Valid;
        public bool Held;
    }
    public KeyCode moveKey;
    public Camera playerCamera;
    public Movement movement;
    public Transform camerafollowPlayer;
    public Transform camerafollowOrigin;
    public bool movementEnabled = true;
    public bool warp;
    public bool stunned;
    public float destinationTreshold = 0.25f;
    public LayerMask groundLayers;
    public LayerMask ignoreLayers;
    public LayerMask uiLayerMask;
    public float maxGroundRaycastDistance = 100;
    public float minimumPathDistance = 0.5f;
    public float samplePositionDistanceMax = 5f;
    public bool allowHoldKey = true;
    public float holdMoveCd = 0.1f, nextHoldMove;
    public float dragThreshold = 0.15f;
    public float holdTime;
    public System.Action<GameObject,Vector3, bool> onInput ;
    public int layerMask;
    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("UI");
        //allowHoldKey = GameSetting.instance.deviceType == GameSetting.DeviceType.Web;
    }
    void Update()
    {
        camerafollowPlayer.position = camerafollowOrigin.position;
        MovementLogic();
    }
    private void MovementLogic()
    {


        if (InterfaceRoot.instance.isMask || UIHover.Hover)
            return;

        if (!movementEnabled || stunned || IsPointerOverUIObject())
        {
            return;
        }
        MoveInputType moveInputType = MovingInput();
        if (!moveInputType.Valid) return;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var ignore, maxGroundRaycastDistance, ignoreLayers))
        {
            return;
        }
        if (!Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var hit, maxGroundRaycastDistance, groundLayers))
        {
            return;
        }
        var destination = hit.point;
        //onInput?.Invoke( hit.collider.gameObject , destination , moveInputType.Held );
        movement.MoveTo( destination , moveInputType.Held );
    }


    private MoveInputType MovingInput()
    {
        MoveInputType moveInputType = new MoveInputType();

        if(Input.GetKey(moveKey))
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

        if (!allowHoldKey || !Input.GetKey(moveKey)) return moveInputType;
        nextHoldMove = Time.time + holdMoveCd;
        moveInputType.Valid = true;
        moveInputType.Held = true;
        return moveInputType;
    }
    public bool IsPointerOverUIObject()
    {
        //if (GameSetting.instance.deviceType == GameSetting.DeviceType.Mobile)
        //{
        //    if (Input.touchCount > 0)
        //    {
        //        Touch mainTouch = Input.touches[0];
        //        Ray ray = UICamera.currentCamera.ScreenPointToRay(mainTouch.position);
        //        RaycastHit[] hits = Physics.RaycastAll(ray, 100, uiLayerMask);
        //        int count = hits.Length;
        //        foreach (RaycastHit hit in hits)
        //        {
        //            //Debug.Log($"HIT {hit.transform.gameObject.name}");
        //            if (hit.transform.GetComponent<UIRoot>() != null)
        //            {
        //                count--;
        //                //Debug.Log($"HIT {hit.transform.gameObject.name} > REMOVED");
        //            }
        //        }
        //        return count > 0;
        //    }
        //}

        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var results = new List<RaycastResult>();
        if (EventSystem.current == null) return false;
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
