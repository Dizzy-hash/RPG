using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace ACT
{
    public class ActAnnular : ActTraceObj
    {
        public Transform RotateTarget
        {
            get; private set;
        }

        public Vector3   RotatePoint
        {
            get; private set;
        }

        public ActAnnular()
        {
            EventType = EActEventType.Subtain;
        }

        protected override bool Trigger()
        {
            base.Trigger();
            if(World)
            {
                this.RotatePoint  = this.Skill.Caster.Avatar.GetBindPosition(CasterBind);
            }
            else
            {
                this.RotateTarget = this.Skill.Caster.FixEuler;
                this.Unit.CacheTransform.parent = RotateTarget;
            }
            return true;
        }

        protected override void Execute()
        {
            base.Execute();
            if(World)
            {
                Vector3 axis = new Vector3(0, this.RotatePoint.y, 0);
                this.Unit.CacheTransform.Rotate(axis, Time.deltaTime * MoveSpeed, Space.Self);
            }
            else
            {
                Vector3 axis = this.RotateTarget.up;
                Vector3 point = new Vector3(this.RotateTarget.position.x, this.Unit.CacheTransform.position.y, this.RotateTarget.position.z);
                this.Unit.CacheTransform.RotateAround(point, Vector3.up, Time.deltaTime * MoveSpeed);
            }
        }
    }
}

