using UnityEngine;
using System.Collections;
using BVT.Core;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareBool : ShareValue
    {
        [SerializeField]
        public bool value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (bool)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FBool>(key);
        }
    }
}