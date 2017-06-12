using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GameBattleState : GameBaseState
{

    public override void Enter()
    {
        base.Enter();
        GTWindowManager.Instance.OpenWindow(EWindowID.UI_HOME);
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
