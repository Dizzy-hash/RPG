using UnityEngine;
using System.Collections;
using BVT.Core;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareVector3 : ShareValue
    {
        [SerializeField]
        public Vector3 value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (Vector3)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FVector3>(key);
        }
    }
}
