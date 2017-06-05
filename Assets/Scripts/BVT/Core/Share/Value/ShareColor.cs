using UnityEngine;
using System.Collections;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareColor : ShareValue
    {
        [SerializeField]
        public Color value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (Color)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FColor>(key);
        }
    }
}