using UnityEngine;
using System.Collections;

public class CharacterFearFSM : CharacterBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        Owner.DoFear(Cmd as CommandFear);
    }

    public override void Exit()
    {
        base.Exit();
        CommandFear c = Cmd as CommandFear;
        if (c.ExitCallback != null)
        {
            c.ExitCallback();
            c.ExitCallback = null;
        }
    }

    public override void Break()
    {
        base.Break();
        CommandFear c = Cmd as CommandFear;
        if (c.StopCallback != null)
        {
            c.StopCallback();
            c.StopCallback = null;
        }
    }
}
