using UnityEngine;
using System.Collections;

namespace Cfg.Act
{
    public class ParamJudgeSphere : ParamJudge
    {
        public float Radius = 5;

        public override void Read(string[] array)
        {
            base.Read(array);
            this.Radius = DHelper.ReadFloat(Get(4, array));
        }

        public override void Save(ref string s)
        {
            base.Save(ref s);
            this.Set(4, Radius, ref s);
        }
    }
}
