using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class CameraEngine : MonoBehaviour
    {


        public Camera mainCamera;
        public Cinemachine.CinemachineBrain cinemachineBrain;
        public Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
        public void Init()
        {
            cinemachineBrain.enabled = false;
            cinemachineVirtualCamera.enabled = false;
        }
        public void StartGame()
        {
            cinemachineBrain.enabled = true;
            cinemachineVirtualCamera.enabled = true;
        }
        public void GameOver()
        {
            cinemachineBrain.enabled = false;
            cinemachineVirtualCamera.enabled = false;
        }
    }
}
