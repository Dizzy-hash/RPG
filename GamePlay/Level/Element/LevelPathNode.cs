using UnityEngine;
using System.Collections;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelPathNode : LevelElement
    {
        public float Time;

        public override void Import(CfgBase pData,bool pBuild)
        {
            MapPathNode data = pData as MapPathNode;
            this.Time        = data.Time;
            this.Pos         = data.Pos;
            this.Euler       = data.Euler;
            this.Build();
            this.SetName();
        }

        public override CfgBase Export()
        {
            MapPathNode data = new MapPathNode();
            data.Time        = Time;
            data.Pos         = Pos;
            data.Euler       = Euler;
            return data;
        }
    }
}

