using UnityEngine;
using System.Collections;

public class ActorMineFSM:ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        CommandMine ev = Cmd as CommandMine;
        if (ev.LastTime > 0)
        {
            GTTimerManager.Instance.AddListener(ev.LastTime, Break);
        }
        Owner.OnMine(ev);
    }
}
