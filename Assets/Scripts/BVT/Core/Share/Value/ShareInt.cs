using UnityEngine;
using System.Collections;
using BVT.Core;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareInt : ShareValue
    {
        [SerializeField]
        public int value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (int)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FInt>(key);
        }
    }
}
