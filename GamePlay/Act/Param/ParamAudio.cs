using UnityEngine;
using System.Collections;
using System;

namespace Cfg.Act
{
    public class ParamAudio : ParamBase
    {
        public string ClipName = string.Empty;

        public override void Read(string[] array)
        {
            this.ClipName = Get(0, array);
        }

        public override void Save(ref string s)
        {
            this.Set(0, ClipName, ref s);
        }
    }
}
