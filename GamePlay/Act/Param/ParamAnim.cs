using UnityEngine;
using System.Collections;
using System;

namespace Cfg.Act
{
    public class ParamAnim : ParamBase
    {
        public string AnimName = string.Empty;

        public override void Read(string[] array)
        {
            this.AnimName = Get(0, array);
        }

        public override void Save(ref string s)
        {
            this.Set(0, AnimName, ref s);
        }
    }
}
