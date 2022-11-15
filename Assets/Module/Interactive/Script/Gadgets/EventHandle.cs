using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class EventHandle : MonoBehaviour
{
    //public List<string> Tags;
    public bool IsEnable
    {
        get { return (collider != null)? collider.enabled : false ; }
        set {
            if (collider != null)
                collider.enabled = value;
        }
    }
    Collider n_collider;
    public Collider collider
    {
        get {
            if (n_collider == null)
                n_collider = GetComponent<Collider>();
            return n_collider;
        }
    }


    Service.Tools.DoubleClick dbc = new Service.Tools.DoubleClick();

    public List<System.Action<GameObject>> EventOnClick = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject>> EventOnDoubleClick = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject>> EventOnDown = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject>> EventOnUp = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject>> EventOnExit = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject>> EventOnOver = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject>> EventOnEnter = new List<System.Action<GameObject>>();
    public List<System.Action<GameObject,Vector3, Vector3>> EventOnDrag = new List<System.Action<GameObject, Vector3, Vector3>>();
    public List<System.Action<GameObject>> EventOnButton = new List<System.Action<GameObject>>();


    //bool validateTag(GameObject gameobject) {

    //    if (Tags.Count == 0)
    //        return true;
    //    if(gameobject == null)
    //        return false;

    //    foreach (var tag in Tags)
    //        if (gameobject.tag == tag)
    //            return true;
    //    return false;
    //}


    bool isReadyToClick = false;
    private void OnMouseExit()
    {
        isReadyToClick = false;
        if (IsEnable) EventOnExit?.ForEach(x => x.Invoke(gameObject));
    }
    private void OnMouseUp()
    {
        if (isReadyToClick)
        {
            if (IsEnable) EventOnClick?.ForEach(x => x.Invoke(gameObject));
            if (IsEnable) dbc.OnEnter(()=> { EventOnDoubleClick?.ForEach(x => x.Invoke(gameObject)); });
        }
        isReadyToClick = false;
        if(IsEnable) EventOnUp?.ForEach(x => x.Invoke(gameObject));
    }
    private void OnMouseDown()
    {
        lastDrag = Vector3.zero;
        isReadyToClick = true;
        if (IsEnable) EventOnDown?.ForEach(x => x.Invoke(gameObject));
    }
    private void OnMouseOver()
    {
        if (IsEnable) EventOnOver?.ForEach(x => x.Invoke(gameObject));
    }

    Vector3 lastDrag;
    private void OnMouseDrag()
    {
        var objpos = Camera.main.WorldToScreenPoint(transform.position);
        var position  =   Input.mousePosition - objpos; position.z = 0.0f;
        var delta = position - lastDrag;
        if (IsEnable) EventOnDrag?.ForEach(x => x.Invoke(gameObject, position , delta));
        lastDrag = position;
    }
    private void OnMouseEnter()
    {
        //Debug.Log("OnMouseEnter : " + IsEnable);
        if (IsEnable) EventOnEnter?.ForEach(x => x.Invoke(gameObject));
    }
    private void OnMouseUpAsButton()
    {
        if (IsEnable) EventOnButton?.ForEach(x => x.Invoke(gameObject));
        //if (IsEnable) dbc.OnEnter(() => { EventOnDoubleClick.ForEach(x => x.Invoke(gameObject)); });
    }
    
}
