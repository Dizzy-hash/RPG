using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorFrostFSM : ActorBaseFSM
{
    public override void Enter()
    {
        CommandFrost ev = Cmd as CommandFrost;
        if (ev.LastTime > 0)
        {
            GTTimerManager.Instance.AddListener(ev.LastTime, Break);
        }
        Owner.ApplyRootMotion(false);
        Owner.ApplyAnimator(false);
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        Owner.ApplyRootMotion(true);
        Owner.ApplyAnimator(true);
    }
}
