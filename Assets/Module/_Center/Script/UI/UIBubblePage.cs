using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Center
{

    public class UIBubblePage : MonoBehaviour
    {
        public static UIBubblePage instance;
        public InterfaceRoot root;
        [SerializeField] Transform rootUIBubble;
        [SerializeField] GameObject prefabBubble;
        List<UIBubble> uiBubbles = new List<UIBubble>();
        [SerializeField] Texture imgDefault;
        public void Init()
        {
            instance = this;
        }


        public UIBubble Open(Texture Icon, Transform target3D, Transform page, System.Action action)
        {
            var bub = InterfaceRoot.instance.UIPool<UIBubble>(prefabBubble, page);
            bub.Init(this, Icon != null ? Icon : imgDefault, target3D, action);
            uiBubbles.Add(bub);
            return bub;
        }
        public void Remove(UIBubble bub)
        {
            uiBubbles.Remove(bub);
        }
        public List<UIBubble> Find(string groupName)
        {
            return uiBubbles.FindAll(x => x.group == groupName);
        }
        public void OnClose(string groupName)
        {
            Find(groupName).ForEach(x => x.OnCloseBubble());
        }
        public void OnClose(List<UIBubble> UIBubbles)
        {
            UIBubbles.ForEach(x => x.OnCloseBubble());
        }






    }
}
