using UnityEngine;
using System.Collections;
using System;

namespace BVT.Core
{
    [Serializable]
    public class ShareGameObject : ShareValue
    {
        [SerializeField]
        public GameObject value;

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object v, bool syncToData = true)
        {
            this.value = (GameObject)v;
            base.SetValue(v, syncToData);
        }

        public override void SetKey(string key)
        {
            this.SetKey<FGameObject>(key);
        }
    }
}
