using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorRunFSM : ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        if(Cmd is CommandRunTo)
        {
            CommandRunTo ev = Cmd as CommandRunTo;
            Owner.OnPursue(ev);
        }
        else
        {
            CommandMoveTo ev = Cmd as CommandMoveTo;
            Owner.OnForceToMove(ev);
        }
    }

    public override void Execute()
    {
        base.Execute();
    }
}
