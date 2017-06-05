using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ACT
{
    public class ActDot : ActBuffInterval
    {
        [SerializeField]
        public EDamageType  Type                 = EDamageType.TYPE_PHYSICS;
        [SerializeField] 
        public float        TickPercent          = 1;
        [SerializeField]
        public Int32        TickFixValue         = 100;
        [SerializeField]
        public bool         IgnoreDefense        = false;
    }
}

