using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorSkillFSM : ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        CommandUseSkill ev = Cmd as CommandUseSkill;

        if(ev.LastTime>0)
        {
            GTTimerManager.Instance.AddListener(ev.LastTime, Break);
        }
        Owner.ApplyRootMotion(false);
        Owner.OnUseSkill(ev);
    }

    public override void Exit()
    {
        base.Exit();
        Owner.ApplyRootMotion(true);
    }
}
