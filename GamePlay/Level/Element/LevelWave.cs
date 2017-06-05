using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelWave : LevelContainerBase<LevelMonster>
    {
        public int                       Index;
        public string                    IndexName = string.Empty;
        public float                     Delay;
        public EMonsterWaveSpawn         Spawn     = EMonsterWaveSpawn.TYPE_ALONG;

        public override void SetName()
        {
            gameObject.name = "Wave_Index_" + Index.ToString();
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            MapMonsterWave data = pData as MapMonsterWave;
            Index        = data.Index;
            IndexName    = data.IndexName;
            Delay        = data.Delay;
            Spawn        = data.Spawn;
            this.Build();
            this.SetName();
            if(pBuild)
            {
                for (int i = 0; i < data.Monsters.Count; i++)
                {
                    GameObject go = NGUITools.AddChild(gameObject);
                    LevelMonster pCall = go.AddComponent<LevelMonster>();
                    pCall.Import(data.Monsters[i],pBuild);
                }
            }
        }

        public override CfgBase Export()
        {
            MapMonsterWave data = new MapMonsterWave();
            data.Index       = Index;
            data.IndexName   = IndexName;
            data.Delay       = Delay;
            data.Spawn       = Spawn;
            for (int i = 0; i < Elements.Count; i++)
            {
                data.Monsters.Add(Elements[i].Export() as MapMonster);
            }
            return data;
        }
    }
}

