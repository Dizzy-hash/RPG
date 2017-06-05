using UnityEngine;
using System.Collections;
using System;

namespace LevelDirector
{
    public class LevelBorn : LevelElement
    {
        public EBattleCamp Camp = EBattleCamp.A;
        private GameObject mBody;

        public override void Build()
        {
            if(mBody==null)
            {
                mBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mBody.transform.ResetLocalTransform(transform);
            }
            MeshRenderer render = mBody.GetComponent<MeshRenderer>();
            if (render == null)
            {
                return;
            }
            if (render.sharedMaterial != null)
            {
                Shader shader = Shader.Find("Custom/TranspUnlit");
                render.sharedMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            }
            switch (Camp)
            {
                case EBattleCamp.A:
                    render.sharedMaterial.color = Color.green;
                    break;
                case EBattleCamp.B:
                    render.sharedMaterial.color = Color.blue;
                    break;
                case EBattleCamp.C:
                    render.sharedMaterial.color = Color.yellow;
                    break;
            }
        }

        public override void SetName()
        {
            gameObject.name = "Born_" + Camp.ToString();
        }

        public override CfgBase Export()
        {
            Cfg.Map.MapBorn data = new Cfg.Map.MapBorn();
            data.Camp   = Camp;
            data.Pos    = Pos;
            data.Scale  = Scale;
            data.Euler  = Euler;
            return data;
        }

        public override void Import(CfgBase pData, bool pBuild)
        {
            Cfg.Map.MapBorn data = pData as Cfg.Map.MapBorn;
            Camp = data.Camp;
            Pos = data.Pos;
            Scale = data.Scale;
            Euler = data.Euler;
            this.Build();
            this.SetName();
        }
    }
}
