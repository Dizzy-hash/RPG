using UnityEngine;
using System.Collections;

public class ActorJumpFSM : ActorBaseFSM
{
    public override void Enter()
    {
        Owner.OnJump();
    }
}
