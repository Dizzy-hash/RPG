using UnityEngine;
using System.Collections;

namespace LevelDirector
{
    public class LevelPortal : LevelElement, ILevelRegionComponent
    {
        public int     DestMapID;
        public Vector3 DestPos;
        public bool    DisplayText = false;
        public string  Name        = string.Empty;
        public ECR     CR          = ECR.AND;
        public int     OpenLevel;
        public int     OpenItemID;
        public int     OpenVIP;

        public LevelRegion Region
        {
            get; set;
        }

        public int RegionID
        {
            get { return Region == null ? 0 : Region.Id; }
        }

        public GameObject EffectObj
        {
            get; set;
        }

        public override void Build()
        {
            if (EffectObj == null)
            {
                EffectObj = GTResourceManager.Instance.Load<GameObject>(GTPrefabDefine.PRE_PORTALEFFECT, true);
            }
            if (EffectObj != null)
            {
                EffectObj.transform.ResetLocalTransform(transform);
            }
        }

        public override void SetName()
        {
            gameObject.name = "Portal_" + Id.ToString();
        }

        public override CfgBase Export()
        {
            Cfg.Map.MapPortal data = new Cfg.Map.MapPortal();
            data.Id           = Id;
            data.OpenItemID   = OpenItemID;
            data.OpenLevel    = OpenLevel;
            data.OpenVIP      = OpenVIP;
            data.Name         = Name;
            data.RegionID     = RegionID;
            data.DestMapID    = DestMapID;
            data.DestPos      = DestPos;
            data.DisplayText  = DisplayText;
            data.CR           = CR;
            data.Pos          = Pos;
            data.Euler        = Euler;
            return data;
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            Cfg.Map.MapPortal data = pData as Cfg.Map.MapPortal;
            Id             = data.Id;
            OpenItemID     = data.OpenItemID;
            OpenLevel      = data.OpenLevel;
            OpenVIP        = data.OpenVIP;
            Name           = data.Name;
            DestMapID      = data.DestMapID;
            DestPos        = data.DestPos;
            DisplayText    = data.DisplayText;
            CR             = data.CR;
            Pos            = data.Pos;
            this.Build();
            this.SetName();
            HolderRegion pHolder = GTLevelManager.Instance.GetHolder(EMapHolder.Region) as HolderRegion;
            this.Region = pHolder.FindElement(data.RegionID);
            if (Region != null)
            {
                Pos        = data.Pos;
                Euler      = data.Euler;
                Region.onTriggerEnter = onTriggerEnter;
            }
        }

        void onTriggerEnter(Collider other)
        {
            if (Region == null|| DestMapID == 0)
            {
                return;
            }
            if (other.gameObject.layer != GTLayerDefine.LAYER_AVATAR &&
                other.gameObject.layer != GTLayerDefine.LAYER_MOUNT)
            {
                return;
            }
            if (DestMapID == GTLauncher.Instance.CurMapID)
            {
                LevelData.MainPlayer.GetVehicle().TranslateTo(DestPos, true);
            }
            else
            {
                GTLauncher.Instance.LoadScene(DestMapID);
            }
        }
    }
}

