using UnityEngine;
using System.Collections;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareFloat : ShareValue
    {
        [SerializeField]
        public float value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (float)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FFloat>(key);
        }
    }
}