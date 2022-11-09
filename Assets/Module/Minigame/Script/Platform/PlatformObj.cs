using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame { 
    public class PlatformObj : BasePool
    {


#if UNITY_EDITOR
        public void FindGround(PlatformObj platform) 
        {
            foreach (var obj in platform.floor.GetAllNode())
            {
                var item = obj.GetComponent<GroundCollider>();
                if (item != null)
                {
                    if (obj.tag == "Ground" && obj.GetComponent<Collider2D>() != null)
                    {
                        item.platformObj = platform;
                    }
                    else 
                    {
                        DestroyImmediate(item);
                    }
                }
                else 
                {
                    if (obj.tag == "Ground" && obj.GetComponent<Collider2D>() != null) 
                    {
                        item = obj.gameObject.AddComponent<GroundCollider>();
                        item.platformObj = platform;
                    }
                }
            }
        }
        public RuntimeBtn View = new RuntimeBtn((r) => {
            PlatformObj platform = r.Gameobject.GetComponent<PlatformObj>();
            platform.FindGround(platform);

            if (platform.objects.GetAllParent().Count == 0)
            {
                Debug.Log("Genarateion");
                foreach (var node in platform.Nodes) 
                {
                    var obj = node.objectId.notnull()? GameStore.instance.objectData.Find(node.type, node.objectId) : GameStore.instance.objectData.FindRandom(node.type);
                    if (obj != null)
                    {
                       var root = obj.gameObject.Create(platform.objects);
                       node.Snap(root.transform);
                    }
                }
            }
        });
        public RuntimeBtn Remove = new RuntimeBtn((r) => {
            PlatformObj platform = r.Gameobject.GetComponent<PlatformObj>();
            platform.FindGround(platform);
            platform.objects.DesAllParent();
        });
        public RuntimeBtn Save = new RuntimeBtn((r) => {
            PlatformObj platform = r.Gameobject.GetComponent<PlatformObj>();
            platform.FindGround(platform);

            List<Node> Nodes = new List<Node>();
            foreach (var obj in platform.objects.GetAllParent()) 
            {
                var item = obj.GetComponent<CollectBase>();
                if (item != null) 
                {
                    Nodes.Add(new Node() {
                        objectId= (item.type != CollectType.Booster)? item.objectId : null,
                        type = item.type,
                        location = item.transform.localPosition,
                        scale = item.transform.localScale,
                        rotate = item.transform.localRotation.eulerAngles
                    });
                }
            }
            if (Nodes.Count>0)
                platform.Nodes = Nodes;

        });
#endif




   

        public List<Node> Nodes = new List<Node>();
        [System.Serializable]
        public class Node 
        {
            public Vector3 location;
            public Vector3 scale;
            public Vector3 rotate;
            public string objectId;
            public CollectType type;
            public void Snap(Transform root) 
            {
                root.localPosition = location;
                root.localScale = scale;
                root.localEulerAngles = rotate;
            }

            GameObject m_gameobject;
            public GameObject Load ( )
            {
                if (objectId.isnull()) 
                {
                    var collect = GameStore.instance.objectData.FindRandom(type);
                    return collect != null ? collect.gameObject : null;
                }


                if (m_gameobject == null) 
                {
                    var collect = GameStore.instance.objectData.Find(type, objectId);
                    if (collect != null) m_gameobject = collect.gameObject;
                }
                return m_gameobject;
            }
        }



     
        public Transform floor;
        public Transform objects;
        PlatformManager manager;
        bool isReady;
        bool isEnter;

        public void Init(PlatformManager manager)
        {
            isReady = true;
            isEnter = false;
            this.manager = manager;


            Genarate();
        }



        public void OnEnter() 
        {
            if (isReady && !isEnter) 
            {
                isEnter = true;
                foreach (var pool in m_pools)
                {
                    pool.OnEnter();
                }
            }
        }



        List<CollectBase> m_pools = new List<CollectBase>();
        void Genarate() 
        {
            m_pools.Clear();
            objects.DesAllParent();
            foreach (var node in Nodes)
            {

                if (node.type == CollectType.Booster) 
                {
                    if (manager.IsCanGenBooster())
                    {

                    }
                    else 
                    {
                        continue;
                    }
                }

                var obj = node.Load();
                if (obj != null)
                {
                    var pool = obj.gameObject.Pool(objects).GetComponent<CollectBase>();
                    node.Snap(pool.transform);
                    pool.Init(this);
                    m_pools.Add(pool);
                }
            }
        }
        public void RemoveCollect(CollectBase obj)
        {
            obj.Destroy();
            m_pools.Remove(obj);
        }
        public void Clear()
        {
            isReady = false;
            Destroy();
            foreach (var pool in m_pools)
            {
                pool.Destroy();
            }
            m_pools.Clear();
        }


        private void Update()
        {
            if (isReady) 
            {
                var dist = Player.PlayerData.player.transform.position.x - transform.position.x;
                if (dist > manager.farToTrash) 
                {
                    isReady = false;
                    manager.Trash(this);
                }
            }
        }


    }
}