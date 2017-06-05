using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ACT
{
    public class ActScope : ActItem
    {
        [SerializeField]
        public EAffect  Affect        = EAffect.Self;
        [SerializeField]
        public int      MaxCount      = 1;
        [SerializeField]
        public Vector3  Offset        = Vector3.zero;
        [SerializeField]
        public Vector3  Euler         = Vector3.zero;
        [SerializeField]
        public bool     Warning       = false;
        [SerializeField]
        public bool     ApplyByCenter = true;

        public ActScope()
        {
            EventType = EActEventType.Instant;
        }

        protected override bool Trigger()
        {
            base.Trigger();
            this.ClearAttackList();
            List<Character> listAffect = Skill.Caster.GetAffectCharacters(Affect);
            for (int i = 0; i < listAffect.Count; i++)
            {
                Character cc = listAffect[i];
                if (ApplyByCenter)
                {
                    if (IsTouch(cc, ApplyCenter))
                    {
                        this.AddInAttackList(cc);
                    }
                }
                else
                {
                    if (IsTouch(cc, ApplyHitPoint, ApplyHitDir))
                    {
                        this.AddInAttackList(cc);
                    }
                }
                if (mTargetList.Count >= MaxCount && MaxCount > 0)
                {
                    break;
                }
            }
            for (int i = 0; i < Children.Count; i++)
            {
                ActItem item = Children[i];
                item.ClearAttackList();
                for (int k = 0; k < mTargetList.Count; k++)
                {
                    item.AddInAttackList(mTargetList[k]);
                }
            }
            if (Warning)
            {
                this.ShowWarning();
            }
            return mTargetList.Count > 0;
        }

        protected virtual bool IsTouch(Character cc, Transform center)
        {
            if(center ==null)
            {
                return false;
            }
            else
            {
                return IsTouch(cc, center.position, center.forward);
            }
        }

        protected virtual bool IsTouch(Character cc, Vector3 hitPoint, Vector3 dir)
        {
            return false;
        }

        protected virtual void ShowWarning()
        {

        }
    }
}

