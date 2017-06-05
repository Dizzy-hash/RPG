using UnityEngine;
using System.Collections;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareComponent : ShareValue
    {
        [SerializeField]
        public Component value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (Component)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FComponent>(key);
        }
    }
}