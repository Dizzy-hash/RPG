using UnityEngine;
using System.Collections;

public class AIFollowState : AIBaseState
{
    public override void Enter()
    {

    }

    public override void Execute()
    {
        Actor host = Owner.GetHost();
        if (host == null)
        {
            return;
        }
        Vector3 pos = Owner.GetHost().GetPartnerPosBySort(Owner.Sort);
        Owner.Command.Get<CommandRunTo>().Update(pos, OnFollowFinished).Do();
    }

    private void OnFollowFinished()
    {
        AI.ChangeAIState(EAIState.AI_IDLE);
    }

    public override void Exit()
    {
        Owner.GotoEmptyFSM();
    }
}
