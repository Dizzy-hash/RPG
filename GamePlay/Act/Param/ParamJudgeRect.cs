using UnityEngine;
using System.Collections;

namespace Cfg.Act
{
    public class ParamJudgeRect : ParamJudge
    {
        public float Width = 5;
        public float Length = 7;

        public override void Read(string[] array)
        {
            base.Read(array);
            this.Width = DHelper.ReadFloat(Get(4, array));
            this.Length = DHelper.ReadFloat(Get(5, array));
        }

        public override void Save(ref string s)
        {
            base.Save(ref s);
            this.Set(4, Width, ref s);
            this.Set(5, Length, ref s);
        }
    }
}
