using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorRebornFSM : ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        CommandReborn ev = Cmd as CommandReborn;
        if (ev.LastTime > 0)
        {
            GTTimerManager.Instance.AddListener(ev.LastTime, Break);
        }
        Owner.OnReborn();
    }

}

