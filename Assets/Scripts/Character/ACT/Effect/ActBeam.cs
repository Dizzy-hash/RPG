using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActBeam : ActObj
    {
        [SerializeField]
        public EBind   CasterBind      = EBind.Head;
        [SerializeField]
        public EBind   TargetBind      = EBind.Head;
        [SerializeField]
        public float   MaxDis          = 50;

        public F3DBeam Beam
        {
            get; private set;
        }

        public ActBeam()
        {
            EventType = EActEventType.Subtain;
        }

        protected override bool Trigger()
        {
            base.Trigger();
            this.Unit = GTWorld.Instance.Ect.LoadEffect(ID, 0, Retain);
            this.Beam = this.Unit.CacheTransform.GetComponent<F3DBeam>();
            return true;
        }

        protected override void Execute()
        {
            base.Execute();
            if (Beam == null)
            {
                return;
            }
            Vector3 pos0 = Skill.Caster.Avatar.GetBindPosition(CasterBind);
            Vector3 pos1 = Vector3.zero;
            if (Skill.Target == null || TargetBind == EBind.None)
            {
                Vector3 dir = Skill.Caster.Dir;
                dir.Normalize();
                pos1 = pos0 + dir * MaxDis;
            }
            else
            {
                pos1 = Skill.Target.Avatar.GetBindPosition(TargetBind);
            }
            this.Beam.transform.localPosition = pos0;
            this.Beam.transform.LookAt(pos1);
            this.Beam.MaxBeamLength = Vector3.Distance(pos1, pos0);
        }

        protected override void End()
        {
            base.End();
            if (this.Unit != null)
            {
                this.Unit.Release();
                this.Unit = null;
            }
        }

        public override void Loop()
        {
            if (Status == EActStatus.INITIAL)
            {
                Begin();
            }
            if (PastTime < StTime)
            {
                return;
            }
            if (Status == EActStatus.SELFEND)
            {
                Exit();
                return;
            }
            if (Status == EActStatus.STARTUP)
            {
                Trigger();
            }
            Execute();
            ExecuteChildren();
            if (PastTime >= EdTime && Status == EActStatus.RUNNING)
            {
                End();
            }
        }

        public override void Clear()
        {
            base.Clear();
            if (this.Unit != null)
            {
                this.Unit.Release();
                this.Unit = null;
            }
        }
    }
}

