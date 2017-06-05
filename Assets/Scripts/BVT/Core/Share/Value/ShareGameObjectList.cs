using UnityEngine;
using System.Collections;
using BVT.Core;
using System;
using System.Collections.Generic;

namespace BVT.Core
{
    [Serializable]
    public class ShareGameObjectList : ShareValue
    {
        [SerializeField]
        public List<GameObject> value;

        public override object GetValue()
        {
            return value;
        }

        public override void   SetValue(object v, bool syncToData = true)
        {
            this.value = (List<GameObject>)v;
            base.SetValue(v, syncToData);
        }

        public override void   SetKey(string key)
        {
            this.SetKey<FGameObjectList>(key);
        }
    }
}