using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelMonsterGroup : LevelElement, ILevelRegionComponent
    {
        private HashSet<int> mMonsterGUIDSet = new HashSet<int>();

        public int   MonsterID;
        public float RebornCD = 3;
        public int   MaxCount = 4;

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
            transform.name = "Monster_Group_" + Id;
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            MapMonsterGroup data = pData as MapMonsterGroup;
            Id             = data.Id;
            RebornCD       = data.RebornCD;
            MaxCount       = data.MaxCount;
            MonsterID      = data.MonsterID;
            HolderRegion     pHolder = GTLevelManager.Instance.GetHolder(EMapHolder.Region) as HolderRegion;
            this.Region    = pHolder.FindElement(data.RegionID);
            this.Build();
            this.SetName();
        }

        public override CfgBase Export()
        {
            MapMonsterGroup data = new MapMonsterGroup();
            data.Id        = Id;
            data.RegionID  = RegionID;
            data.RebornCD  = RebornCD;
            data.MaxCount  = MaxCount;
            data.MonsterID = MonsterID;
            return data;
        }

        public override void Init()
        {
            GTEventCenter.AddHandler<int,int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
            for (int i = 0; i < MaxCount; i++)
            {
                CreateMonster();
            }
        }

        private void CreateMonster()
        {
            if (Region == null) return;
            Vector3 pos   = GTTools.RandomOnCircle(10)+Region.Pos;
            Vector3 angle = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
            Actor   actor = GTLevelManager.Instance.AddActor(MonsterID, EActorType.MONSTER, EBattleCamp.B, pos, angle);
            mMonsterGUIDSet.Add(actor.GUID);
        }

        private void OnKillMonster(int guid,int id)
        {
            if (!this.mMonsterGUIDSet.Contains(guid))
            {
                return;
            }
            mMonsterGUIDSet.Remove(guid);
            if(mMonsterGUIDSet.Count<MaxCount)
            {
                Invoke("CreateMonster", RebornCD);
            }
        }

        void OnDestroy()
        {
            CancelInvoke();
            GTEventCenter.DelHandler<int,int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
        }
    }
}
