using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace ACT
{
    public class ActScale : ActBuffItem
    {
        [SerializeField]
        public float Size;
        [SerializeField]
        public float FadeTime;

        public ActScale()
        {
            this.EventType = EActEventType.Subtain;
        }

        protected override bool Trigger()
        {
            base.Trigger();
            float endValue = Size <= 0 ? 1 : Size;
            if (Duration > FadeTime * 2)
            {
                Carryer.ObjTrans.DOScale(endValue, FadeTime);
            }
            else
            {
                Carryer.ObjTrans.localScale = Vector3.one * endValue;
            }
            return true;
        }

        protected override void Execute()
        {
            base.Execute();
            if ((Duration > FadeTime * 2) && (PastTime > EdTime - FadeTime))
            {
                Carryer.ObjTrans.DOScale(1, FadeTime);
            }
        }

        protected override void End()
        {
            base.End();
            Carryer.ObjTrans.localScale = Vector3.one;           
        }
    }
}