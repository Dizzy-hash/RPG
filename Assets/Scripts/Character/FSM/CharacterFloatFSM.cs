using UnityEngine;
using System.Collections;

public class CharacterFloatFSM : CharacterBaseFSM
{
    public override void Enter()
    {
        Owner.DoFloat(Cmd as CommandFloat);
    }

    public override void Exit()
    {
        base.Exit();
        CommandFloat c = Cmd as CommandFloat;
        if (c.ExitCallback != null)
        {
            c.ExitCallback();
            c.ExitCallback = null;
        }
    }

    public override void Break()
    {
        base.Break();
        CommandFloat c = Cmd as CommandFloat;
        if (c.StopCallback != null)
        {
            c.StopCallback();
            c.StopCallback = null;
        }
    }
}
