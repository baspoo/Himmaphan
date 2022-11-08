using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class GroundCollider : MonoBehaviour
    {
        public PlatformObj platformObj;
        public void OnEnter()
        {
            platformObj.OnEnter();
        }
    }
}
