using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ACT
{
    public class ActFlyWeapon : ActTraceObj
    {
        [SerializeField]
        public bool PassBody = false;

        public ActFlyWeapon()
        {
            this.EventType = EActEventType.Subtain;
        }
    }
}

