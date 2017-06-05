using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace ACT
{
    public class ActMove : ActItem
    {
        public ActMove()
        {
            this.EventType = EActEventType.Subtain;
        }

        protected override bool Trigger()
        {
            Vector3 endValue = Skill.Caster.Pos + Skill.Caster.Dir * 5;
            Skill.Caster.CacheTransform.DOMove(endValue, Duration);
            return base.Trigger();
        }
    }
}

