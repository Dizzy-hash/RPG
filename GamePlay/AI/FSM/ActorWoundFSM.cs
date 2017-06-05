using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorWoundFSM : ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        Owner.OnWound();
        Owner.ApplyRootMotion(false);
    }

    public override void Exit()
    {
        base.Exit();
        Owner.ApplyRootMotion(true);
    }
}
