using UnityEngine;
using System.Collections;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelMine : LevelElement
    {
        public float  RebornCD = 5;
        public int    DropItemCount = 1;


        private int        mMineGUID=0;
        private GameObject mBody;

        public override void Import(CfgBase pData, bool pBuild)
        {
            MapMine data   = pData as MapMine;
            Id             = data.Id;
            Pos            = data.Pos;
            Euler          = data.Euler;
            RebornCD       = data.RebornCD;
            DropItemCount  = data.DropItemCount;
            this.Build();
            this.SetName();
        }

        public override CfgBase Export()
        {
            MapMine data = new MapMine();
            data.Id            = Id;
            data.Pos           = Pos;
            data.Euler         = Euler;
            data.RebornCD      = RebornCD;
            data.DropItemCount = DropItemCount;
            return data;
        }

        public override void SetName()
        {
            gameObject.name = "Mine_" + Id.ToString();
        }

        public override void Build()
        {
            NGUITools.DestroyChildren(transform);
            string path = GTTools.Format("Model/Mine/{0}", Id);
            mBody = GTResourceManager.Instance.Load<GameObject>(path, true);
            if(mBody==null)
            {
                return;
            }
            mBody.transform.ResetLocalTransform(transform);
        }
    }
}