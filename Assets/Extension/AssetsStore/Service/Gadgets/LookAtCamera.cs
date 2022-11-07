using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LookAtCamera : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 Offset;
    public bool IslockY;
    void Update()
    {

        if (lookAt == null) 
        {
            lookAt = SceneHandle.instance.cameraManager.mainCamera.transform;
        }

        if (lookAt != null)
        {
            if (IslockY)
            {
                transform.LookAt(lookAt);
                transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
            }
            else 
            {
                transform.LookAt(transform.position + lookAt.forward);
            }
        }
    }

    

}
