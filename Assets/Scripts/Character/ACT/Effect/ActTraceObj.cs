using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System.Collections.Generic;

namespace ACT
{
    public class ActTraceObj : ActObj
    {
        [SerializeField]
        public EBind            CasterBind     = EBind.Head;
        [SerializeField]
        public EMoveToward      MoveToward     = EMoveToward.MoveToTarget;
        [SerializeField]
        public float            MoveSpeed      = 30;
        [SerializeField]
        public EMoveType        MoveType       = 0;
        [SerializeField]
        public int              MoveSpeedCurve = 0;
        [SerializeField]
        public EAffect          Affect         = EAffect.Enem;

        public Mover            Move
        {
            get; private set;
        }

        public ActTraceObj()
        {
            this.EventType = EActEventType.Subtain;
        }

        public override void Begin()
        {
            base.Begin();
            this.RunClone();
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
                ExecuteChildren();
            }
            else
            {
                if (Status == EActStatus.STARTUP)
                {
                    if (!Trigger())
                    {
                        Exit();
                    }
                }
                Execute();
                if (PastTime >= EdTime && Status == EActStatus.RUNNING)
                {
                    Exit();
                }
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

        protected override bool Trigger()
        {
            if (World)
            {
                Transform p = Skill.Caster.Avatar.GetBindTransform(CasterBind);
                Unit = GTWorld.Instance.Ect.LoadEffect(ID, 0, Retain);
                Unit.CacheTransform.parent = p;
                Unit.CacheTransform.localPosition = Offset;
                Unit.CacheTransform.localEulerAngles = Euler;
                Unit.CacheTransform.parent = null;
                Unit.CacheTransform.localScale = Scale;
                Unit.CacheTransform.localEulerAngles = Euler + Skill.Caster.Euler;
            }
            else
            {
                Transform p = Skill.Caster.Avatar.GetBindTransform(CasterBind);
                Unit = GTWorld.Instance.Ect.LoadEffect(ID, 0, KTransform.Create(Offset, Euler, Scale), p, Retain);
            }
            Move = MoverHelper.MakeMove(MoveType, Unit.CacheTransform, MoveSpeed, MoveSpeedCurve,  Skill.Caster.Target);
            Unit.OnTriggerEnter = DoTrigger;
            return true;
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

        protected override void Exit()
        {
            base.Exit();
            if (this.Unit != null)
            {
                this.Unit.Release();
                this.Unit = null;
            }
        }

        protected virtual void DoTrigger(Collider other)
        {
            CharacterView view = other.GetComponent<CharacterView>();
            if (view != null)
            {
                switch(MoveToward)
                {
                    case EMoveToward.MoveToTarget:
                        if (Skill.Caster.Match(Affect, Skill.Target) && view.Owner == Skill.Target)
                        {
                            End();
                        }
                        break;
                    case EMoveToward.MoveToForward:
                        if (Skill.Caster.Match(Affect, view.Owner))
                        {
                            for (int i = 0; i < Children.Count; i++)
                            {
                                Children[i].ApplyHitPoint = other.ClosestPoint(Unit.CacheTransform.position);
                                Children[i].ApplyHitDir = Vector3.up;
                                Children[i].ClearAttackList();
                                Children[i].AddInAttackList(view.Owner);
                            }
                            End();
                        }
                        break;
                }
            }
        }
    }
}