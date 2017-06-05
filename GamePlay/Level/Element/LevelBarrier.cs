using UnityEngine;
using System.Collections;

namespace LevelDirector
{
    public class LevelBarrier : LevelElement
    {
        public const int BARRIER_WIDTH = 14;

        public  float       Width= BARRIER_WIDTH;
        private Transform   mBody;
        private BoxCollider mCollider;
        private Vector3     mSize;

        public override void Build()
        {
            Width = Width < BARRIER_WIDTH ? BARRIER_WIDTH : Width;
            int count = Mathf.CeilToInt(Width / BARRIER_WIDTH);
            mSize.x = count * BARRIER_WIDTH;
            mSize.y = 4;
            mSize.z = 1.5f;

            mBody = transform.FindChild("Body");
            if (mBody == null)
            {
                mBody = new GameObject("Body").transform;
                mBody.parent = transform;
                mBody.transform.localPosition = Vector3.zero;
                mBody.localEulerAngles = Vector3.zero;
            }
            else
            {
                NGUITools.DestroyChildren(mBody);
            }
            float halfCount = count * 0.5f;
            for (int i = 0; i < count; i++)
            {
                GameObject unit = GTResourceManager.Instance.Instantiate(GTPrefabDefine.PRE_BARRIER);
                if (unit == null)
                {
                    return;
                }
                unit.name = i.ToString();
                Transform trans = unit.transform;
                Vector3 localPosition = Vector3.right * (i - halfCount + 0.5f) * BARRIER_WIDTH;
                localPosition.z = mSize.z * 0.5f;
                trans.localPosition = localPosition;
                trans.SetParent(mBody, false);
            }
            mCollider = gameObject.GET<BoxCollider>();
            mCollider.size = mSize;
            mCollider.center = new Vector3(0, mSize.y * 0.5f, mSize.z * 0.5f);
            NGUITools.SetLayer(gameObject, GTLayerDefine.LAYER_BARRER);

        }

        public override void SetName()
        {
            gameObject.name = "Barrier_" + Id.ToString();
        }

        public override CfgBase Export()
        {
            Cfg.Map.MapBarrier data = new Cfg.Map.MapBarrier();
            data.Id       = Id;
            data.Width    = Width;
            data.Pos      = Pos;
            data.Scale    = Scale;
            data.Euler    = Euler;
            return data;
        }

        public override void Import(CfgBase pData,bool pBuild)
        {
            Cfg.Map.MapBarrier data = pData as Cfg.Map.MapBarrier;
            Id       = data.Id;
            Width    = data.Width;
            Pos      = data.Pos;
            Scale    = data.Scale;
            Euler    = data.Euler;
            this.Build();
            this.SetName();
        }
    }
}