using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorStunFSM : ActorBaseFSM
{

    public override void Enter()
    {
        base.Enter();
        Owner.ApplyRootMotion(false);

        CommandStun cmd = Cmd as CommandStun;
        Owner.OnStun(cmd.LastTime);
    }

    public override void Exit()
    {
        base.Exit();
        Owner.ApplyRootMotion(true);
    }
}
