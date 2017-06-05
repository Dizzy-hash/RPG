using UnityEngine;
using System.Collections;
using System;

namespace Cfg.Act
{
    public class ParamJudgeCylinder : ParamJudge
    {
        public float Radius = 5;
        public float Angle = 180;

        public override void Read(string[] array)
        {
            base.Read(array);
            this.Radius = DHelper.ReadFloat(Get(4, array));
            this.Angle = DHelper.ReadFloat(Get(5, array));
        }

        public override void Save(ref string s)
        {
            base.Save(ref s);
            this.Set(4, Radius, ref s);
            this.Set(5, Angle, ref s);
        }
    }
}

