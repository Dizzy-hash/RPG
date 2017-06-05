using Cfg.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LevelDirector
{
    public class LevelObj : LevelElement
    {
        public string      Name = string.Empty;
        public EMapObjType Type = EMapObjType.TYPE_BUILD;
        public string      Path
        {
            get
            {
                switch (Type)
                {
                    case EMapObjType.TYPE_PLANT:
                        return string.Format("Model/Plant/Plant{0}", this.Id);
                    case EMapObjType.TYPE_BUILD:
                        return string.Format("Model/Build/Build{0}", this.Id);
                    case EMapObjType.TYPE_STONE:
                        return string.Format("Model/Stone/Stone{0}", this.Id);
                }
                return string.Empty;
            }
        }

        private GameObject mBody;

        public override void Build()
        {
            if (mBody != null)
            {
                GameObject.DestroyImmediate(mBody);
            }
            mBody = GTResourceManager.Instance.Load<GameObject>(Path, true);
            if (mBody == null)
            {
                return;
            }
            mBody.transform.ResetLocalTransform(transform);
        }

        public override void SetName()
        {
            if (mBody != null)
            {
                gameObject.name = mBody.name;
            }
        }

        public override void Import(CfgBase pData, bool pBuild)
        {
            MapObj data = pData as MapObj;
            this.Id    = data.Id;
            this.Pos   = data.Pos;
            this.Euler = data.Euler;
            this.Scale = data.Scale;
            this.Name  = data.Name;
            this.Type  = data.Type;
            this.Build();
            this.SetName();
        }

        public override CfgBase Export()
        {
            MapObj data = new MapObj();
            data.Id    = this.Id;
            data.Pos   = this.Pos;
            data.Euler = this.Euler;
            data.Scale = this.Scale;
            data.Name  = this.Name;
            data.Type  = this.Type;
            data.Path  = this.Path;
            return data;
        }
    }
}
