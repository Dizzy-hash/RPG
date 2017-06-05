using UnityEngine;
using System.Collections;
using System;

public class CharacterBaseFSM : IState<Character, FSMState>
{

    public override void Enter()
    {
        if (LastTime > 0)
        {
            GTTimerManager.Instance.AddListener(LastTime, Break);
        }
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        LastTime = 0;
        GTTimerManager.Instance.DelListener(Break);
    }

    public override void Break()
    {
        LastTime = 0;
        Owner.ChangeEmptyFSM();
        GTTimerManager.Instance.DelListener(Break);    
    }
}
