using UnityEngine;
using System.Collections;
using System;

namespace Cfg.Act
{
    public class ParamJudge : ParamBase
    {
        public EAoeArea Type = EAoeArea.TYPE_CYLINDER;
        public EActPosition ActPos = EActPosition.Caster;
        public Vector3 Offset = Vector3.zero;
        public Vector3 Euler = Vector3.zero;

        public override void Save(ref string s)
        {
            this.Set(0, (int)Type, ref s);
            this.Set(1, (int)ActPos, ref s);
            this.Set(2, Offset, ref s);
            this.Set(3, Euler, ref s);
        }

        public override void Read(string[] array)
        {
            this.Type = (EAoeArea)DHelper.ReadInt32(Get(0, array));
            this.ActPos = (EActPosition)DHelper.ReadInt32(Get(1, array));
            this.Offset = DHelper.ReadVector3(Get(2, array));
            this.Euler = DHelper.ReadVector3(Get(3, array));
        }
    }
}

