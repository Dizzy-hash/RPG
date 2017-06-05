using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorBeatFlyFSM : ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        Owner.OnBeatFly();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
