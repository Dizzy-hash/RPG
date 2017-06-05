using UnityEngine;
using System.Collections;
using System;

public class ActorAI : IGame
{
    public Actor                         Owner          { get; private set; }
    public EAIState                      AIState        { get; private set; }
    public bool                          Auto           { get; set; }
    public IStateMachine<Actor,EAIState> AIStateMachine { get; private set; }

    public float WARDIST
    {
        get { return 30; }
    }

    public float ATKDIST
    {
        get { return Owner.GetActorSkill().GetMinCastDistance(); }
    }

    public float FOLLOWDIST
    {
        get { return 4; }
    }

    public float FindEnemyTimer = GTDefine.V_INTERVAL_FINDENEMY;

    public ActorAI(Actor pOwner)
    {
        this.Owner = pOwner;
        this.AIStateMachine = new IStateMachine<Actor,EAIState>(pOwner);
        this.AIState = EAIState.AI_IDLE;
        this.Auto    = (Owner is ActorMainPlayer) == false;
        this.AIStateMachine.AddState(EAIState.AI_IDLE, new AIIdleState());
        this.AIStateMachine.AddState(EAIState.AI_FOLLOW, new AIFollowState());
        this.AIStateMachine.AddState(EAIState.AI_FLEE, new AIFleeState());
        this.AIStateMachine.AddState(EAIState.AI_PATROL, new AIPatrolState());
        this.AIStateMachine.AddState(EAIState.AI_ESCAPE, new AIEscapeState());
        this.AIStateMachine.AddState(EAIState.AI_BACK, new AIBackState());
        this.AIStateMachine.AddState(EAIState.AI_FIGHT, new AIFightState());
        this.AIStateMachine.AddState(EAIState.AI_DEAD, new AIDeadState());
        this.AIStateMachine.AddState(EAIState.AI_CHASE, new AIChaseState());
        this.AIStateMachine.AddState(EAIState.AI_GLOBAL, new AIGlobalState());
    }


    public void Startup()
    {
        if (!Auto || Owner.IsDead())
        {
            return;
        }
        IState<Actor, EAIState> initState = this.AIStateMachine.GetState(AIState);
        IState<Actor, EAIState> globalState = this.AIStateMachine.GetState(EAIState.AI_GLOBAL);
        this.AIStateMachine.SetCurState(initState);
        this.AIStateMachine.SetGloState(globalState);
        AIStateMachine.Startup();
    }

    public void Execute()
    {
        if (!Auto || Owner.IsDead())
        {
            return;
        }
        AIStateMachine.Execute();    
    }

    public void ChangeAIState(EAIState pAIState)
    {
        if (AIState != pAIState)
        {
            AIState = pAIState;
            AIStateMachine.ChangeState(pAIState);
        }
    }

    public void ChangeAIMode()
    {
        this.Auto = !Auto;
        if (Auto)
        {
            Startup();
        }
        else
        {
            Stop();
        }
    }

    public void Stop()
    {
        this.AIStateMachine.SetCurState(null);
        this.AIStateMachine.SetGloState(null);
    }

    public void Release()
    {
        AIStateMachine.Release();
    }
}
