using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandle : MonoBehaviour
{

    static SceneHandle m_instance;
    public static SceneHandle instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<SceneHandle>();
            return m_instance;
        }
    }


    public Player player;
    public PoolManager pool;
    public SceneLighting lighting;
    public Minimap minimap;
    public CameraManager cameraManager;
    public Transform effectRoot;


    public void Init()
    {
        player.Init();
        minimap.Init();
        lighting.Init();
        cameraManager.Init();
    }


    public void ApplyPlist() 
    {
        //player.agent.speed = PlayingData.Inst.PlistData.config.playerMoveSpeed;
        //cameraManager.cinemachineVirtualCamera.m_Lens.FieldOfView = PlayingData.Inst.PlistData.config.cameraFieldOfView;
        //cameraManager.dragCamera.PitchSensitivity = PlayingData.Inst.PlistData.config.pitchSensitivity;
        //cameraManager.dragCamera.YawSensitivity = PlayingData.Inst.PlistData.config.yawSensitivity;

        //InterfaceRoot.instance.uiCamera.orthographicSize = PlayingData.Inst.PlistData.config.guiScaleFactor;
        //InterfaceRoot.instance.uiRoot.manualWidth = PlayingData.Inst.PlistData.config.guiWidth;
        //InterfaceRoot.instance.uiRoot.manualHeight = PlayingData.Inst.PlistData.config.guiHeight;
    }



    public void BeginFloor()
    {
        var floor = (PlayingData.Inst.EventData.floors != null && PlayingData.Inst.EventData.floors.Count > 0) ? PlayingData.Inst.EventData.floors[0] : null; ;
    }
    public void LoadFloor(Data.FloorData floor)
    {

    }


}