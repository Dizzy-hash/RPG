using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cfg.Map;
using System;

namespace LevelDirector
{
    public class LevelMineGroup : LevelContainerBase<LevelMine>, ILevelRegionComponent
    {
        public int   MineID;
        public float RebornCD = 3;

        public LevelRegion Region
        {
            get; set;
        }

        public int RegionID
        {
            get { return Region == null ? 0 : Region.Id; }
        }

        public override void SetName()
        {
            transform.name = "Mine_Group_" + Id;
        }

        public override void Import(CfgBase pData, bool pBuild)
        {
            MapMineGroup data = pData as MapMineGroup;
            Id = data.Id;
            HolderRegion pHolder = GTLevelManager.Instance.GetHolder(EMapHolder.Region) as HolderRegion;
            this.Region = pHolder.FindElement(data.RegionID);
            for(int i=0;i<data.Mines.Count;i++)
            {
                GameObject go = NGUITools.AddChild(gameObject);
                LevelMine pMine = go.AddComponent<LevelMine>();
                pMine.Import(data.Mines[i], pBuild);
            }
            this.Build();
            this.SetName();
        }

        public override CfgBase Export()
        {
            MapMineGroup data = new MapMineGroup();
            data.Id       = Id;
            data.RegionID = RegionID;
            for (int i = 0; i < Elements.Count; i++)
            {
                data.Mines.Add(Elements[i].Export() as MapMine);
            }
            return data;
        }
    }
}