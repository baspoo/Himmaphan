using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactive.Player
{
    public class PlayerClient : MonoBehaviour
    {
        public static PlayerClient client;

        //Properties
        public Data PlayerData;
        [System.Serializable]
        public class Data {
            public string NickName;
        }
        public pInterface pInterface;
        public pMovement pMovement;
        public EventHandle eventOnmouseclick;
        public pSfx pSfx;
        public LineProjection line;
        public CapsuleCollider collider;
        public UnityEngine.AI.NavMeshAgent agent;
        public Avatar.AvatarObj avatarObj;
        public Transform rootAvatar;
        public Transform rootShadow;


        public void Init() 
        {
            pMovement.Init(this );
            pSfx.Init(this);
            avatarObj.Init(this);
            pInterface.Init(this);
            CameraControl.CameraEngine.instance.OnCameraFollowCharacter(this, true);
        }
        public void Destroy()
        {

            //--> Clear Avatar
            if (avatarObj != null)
                avatarObj.Remove();

            this.pInterface.Clean( );
            this.pMovement.Clean();
            line.enabled = false;

            //--> Remove Client
            Destroy(gameObject);
        }

    }
}