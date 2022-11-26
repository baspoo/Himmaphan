using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactive {
    public class Manager : MonoBehaviour
    {

        private IEnumerator Start()
        {
            yield return StartCoroutine( Center.ManagerCenter.Init(Center.RootManager.SceneName.Interactive));
            yield return new WaitForEndOfFrame();

            camera.Init();
            player.Init();
        }

        public Interactive.Player.PlayerClient player;
        public Interactive.CameraControl.CameraEngine camera;




    }
}
