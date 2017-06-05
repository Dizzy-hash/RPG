using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ACT
{
    public class ActLighting : ActObj
    {
        [SerializeField]
        public EBind CasterBind = EBind.Head;
        [SerializeField]
        public EBind TargetBind = EBind.Head;

        public ActLighting()
        {
            this.EventType = EActEventType.Subtain;
        }

        protected override bool Trigger()
        {
            base.Trigger();
            this.Unit = GTWorld.Instance.Ect.LoadEffect(ID, 0, Retain);
            return true;
        }

        protected override void Execute()
        {
            base.Execute();
            this.BindBone(Unit, Skill.Caster, Skill.Target);
        }

        protected void BindBone(EffectUnit unit, Character src, Character tar)
        {
            Transform bindBoneSt = GTTools.GetBone(unit.CacheTransform, "Bone001");
            Transform bindBoneEd = GTTools.GetBone(unit.CacheTransform, "Bone002");
            if (src != null)
            {
                Transform bindBone = src.Avatar.GetBindTransform(CasterBind);
                bindBoneSt.position = bindBone != null ? bindBone.position : (src.Pos + Vector3.up * 2);
            }
            if (tar != null)
            {
                Transform bindBone = tar.Avatar.GetBindTransform(TargetBind);
                bindBoneEd.position = bindBone != null ? bindBone.position : (tar.Pos + Vector3.up * 2);
            }
            else
            {
                bindBoneEd.position = src.Pos + src.Dir * 20;
            }
        }
    }
}
