using UnityEngine;
using System.Collections;

public class ActorInterActiveFSM : ActorBaseFSM
{
    public override void Enter()
    {
        base.Enter();
        CommandInterActive ev = Cmd as CommandInterActive;
        if (ev.LastTime > 0)
        {
            GTTimerManager.Instance.AddListener(ev.LastTime, Break);
        }
        Owner.OnInterActive(ev);
    }
}
