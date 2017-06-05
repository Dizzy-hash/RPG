using UnityEngine;
using System.Collections;

public class CharacterBeatDownFSM : CharacterBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        Owner.DoBeatDown(Cmd as CommandBeatDown);
    }

    public override void Exit()
    {
        base.Exit();
        CommandBeatDown c = Cmd as CommandBeatDown;
        if (c.ExitCallback != null)
        {
            c.ExitCallback();
            c.ExitCallback = null;
        }
    }

    public override void Break()
    {
        base.Break();
        CommandBeatDown c = Cmd as CommandBeatDown;
        if (c.StopCallback != null)
        {
            c.StopCallback();
            c.StopCallback = null;
        }
    }
}
