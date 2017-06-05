using UnityEngine;
using System.Collections;
using System;
using System.Xml;

namespace Cfg.Act
{
    public class ParamSpawnObj : ParamBase
    {
        public Int32   ObjID;
        public float   Life;

        public override void Save(ref string s)
        {
            this.Set(0, ObjID, ref s);
            this.Set(1, Life, ref s);
        }

        public override void Read(string[] array)
        {
            this.ObjID = DHelper.ReadInt32(Get(0, array));
            this.Life  = DHelper.ReadFloat(Get(1, array));
        }
    }
}

