using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorBaseFSM : IState<Actor, FSMState>
{
    protected float mLastTime;
    protected GTTimer mLastTimer;

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        if (mLastTimer != null)
        {
            GTTimerManager.Instance.DelListener(mLastTimer);
        }
    }

    protected virtual void Break()
    {
        Owner.SendStateMessage(FSMState.FSM_EMPTY);
    }

    protected void Update()
    {

    }
}
