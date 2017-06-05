using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorDeadFSM : ActorBaseFSM
{

    public override void Enter()
    {
        base.Enter();
        Owner.ApplyCharacterCtrl(false);
        Owner.OnDead(Cmd as CommandDead);
    }
}
