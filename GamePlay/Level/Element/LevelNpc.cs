using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelDirector
{
    public class LevelNpc: LevelElement
    {
        [SerializeField]
        public List<string> Talks = new List<string>();
        public GameObject   Body;

        public override void Build()
        {
            if (Body == null)
            {
                Body = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Body.transform.ResetLocalTransform(transform);
            }
            MeshRenderer render = Body.GetComponent<MeshRenderer>();
            if (render == null)
            {
                return;
            }
            if (render.sharedMaterial != null)
            {
                Shader shader = Shader.Find("Custom/TranspUnlit");
                render.sharedMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            }
            render.sharedMaterial.color = Color.cyan;
        }

        public override void SetName()
        {
            gameObject.name = "Npc_" + Id.ToString();
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            Cfg.Map.MapNpc data = pData as Cfg.Map.MapNpc;
            Id     = data.Id;
            Pos    = data.Pos;
            Euler  = data.Euler;
            Talks  = data.Talks;
            Scale  = data.Scale;
        }

        public override CfgBase Export()
        {
            Cfg.Map.MapNpc data = new Cfg.Map.MapNpc();
            data.Id     = Id;
            data.Pos    = Pos;
            data.Euler  = Euler;
            data.Talks  = Talks;
            data.Scale  = Scale;
            return data;
        }
    }
}
