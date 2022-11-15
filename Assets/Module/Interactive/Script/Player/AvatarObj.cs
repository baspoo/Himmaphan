using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Interactive.Player.Avatar
{
    public class AvatarObj : BasePool
    {
        public AvatarAnim avatarAnim;
        public PlayerClient client => m_client;
        PlayerClient m_client;
        public void Init(PlayerClient client)
        {
            m_client = client;
            m_client.pMovement.onChangeState.Add(avatarAnim.StartAnimation);
            avatarAnim.Init(this);
        }
        public void Init()
        {
            avatarAnim.Init(this);
        }
        public void Remove( ) 
        {
            Destroy();
        }

        public void LookAt(Transform target)
        {
            if (target != null)
            {
                transform.LookAt(target);
                transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
            }
            else 
            {
                transform.localRotation = Quaternion.identity;
            }
        }

        public enum Combline
        {
            equip,
            mergeTexture,
            skinMesh
        }
        public enum MeshType
        {
            none,
            head,
            body,
            bodylow,
            hands,
            feet
        }
        public SkinnedMeshRenderer RendererHead;
        public SkinnedMeshRenderer RendererBody;
        public SkinnedMeshRenderer RendererBodyLow;
        public SkinnedMeshRenderer RendererHands;
        public SkinnedMeshRenderer RendererFeets;
        public List<BodyData> JointsBody = new List<BodyData>() 
        {
            new BodyData(){ name="root", type = BodyType.root },
            new BodyData(){ name="head", type = BodyType.head },
            new BodyData(){ name="earL", type = BodyType.earL },
            new BodyData(){ name="earR", type = BodyType.earR },
            new BodyData(){ name="face", type = BodyType.face },
            new BodyData(){ name="handL", type = BodyType.handL },
            new BodyData(){ name="handR", type = BodyType.handR },
            new BodyData(){ name="footL", type = BodyType.footL },
            new BodyData(){ name="footR", type = BodyType.footR },
            new BodyData(){ name="bag", type = BodyType.bag },
            new BodyData(){ name="body", type = BodyType.body },
        };
        [System.Serializable]
        public class BodyData
        {
            public string name;
            public BodyType type;
            public Transform trans;
        }
        public enum BodyType
        {
            root,
            head,
            earL,
            earR,
            face,
            handL,
            handR,
            footL,
            footR,
            bag,
            body
        }
        public Transform FindJoint(BodyType type)
        {
            return JointsBody.Find(x => x.type == type).trans;
        }



    }
}