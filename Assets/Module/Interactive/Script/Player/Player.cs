using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static Player m_instance;
    public static Player instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<Player>();
            return m_instance;
        }
    }

    public CapsuleCollider collider;
    public UnityEngine.AI.NavMeshAgent agent;
    public Animation animCamera;
    public Movement movement;
    public UserInput input;
    public pSfx sfx;


    public void Init() 
    {
        m_instance = this;
    }










    public enum CameraAnim { 
        zoomin,awakefloor
    }
    public void OnAnimCamera(CameraAnim anim)
    {
        string clipname = "";
        switch (anim)
        {
            case CameraAnim.zoomin:
                clipname = "camera-zoomin";
                break;
            case CameraAnim.awakefloor:
                clipname = "camera-awakefloor";
                break;
        }

        transform.localRotation = Quaternion.identity;
        animCamera.Stop();
        animCamera.Play(clipname);
    }

}
