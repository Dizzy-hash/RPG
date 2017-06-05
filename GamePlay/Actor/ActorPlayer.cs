using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ActorPlayer : Actor
{
    public ActorPlayer(int id, int guid, EActorType type, EBattleCamp camp) :base(id, guid, type,camp)
    {

    }

    public override void OnBeginRide()
    {
        this.StopPathFinding();
        this.LoadMount();
        mMount.SetHost(this);
        mActorAction.Play("qicheng",null,true);
        mCharacter.enabled = false;
        this.SetActorEffect(EActorEffect.IS_RIDE, true);
    }

    public override void OnEndRide()
    {
        CacheTransform.parent = GTLevelManager.Instance.GetHolder(EMapHolder.Role).transform;
        CacheTransform.localPosition = GTTools.NavSamplePosition(Pos);
        mCharacter.enabled = true;
        if (mMount!=null)
        {
            GTLevelManager.Instance.DelActor(mMount);
            mMount = null;
        }
        mVehicle = this;
        this.SetActorEffect(EActorEffect.IS_RIDE, false);
        this.SendStateMessage(FSMState.FSM_IDLE);
    }

    public override void MoveTo(Vector3 destPosition)
    {
        if (mVehicle != this)
        {
            mVehicle.MoveTo(destPosition);
            mActorAction.Play("qicheng_run", null, true);
        }
        else
        {
            base.MoveTo(destPosition);
        }
    }

    public override void StopPathFinding()
    {
        if (mVehicle != this)
        {
            mVehicle.StopPathFinding();
        }
        else
        {
            base.StopPathFinding();
        }
    }

    public override void OnForceToMove(CommandMoveTo ev)
    {
        if (mVehicle != this)
        {
            mVehicle.OnForceToMove(ev);
            mActorAction.Play("qicheng_run", null, true);
        }
        else
        {
            base.OnForceToMove(ev);
        }
    }

    public override void OnPursue(CommandRunTo ev)
    {
        if (mVehicle != this)
        {
            mVehicle.OnPursue(ev);
            mActorAction.Play("qicheng_run", null, true);
        }
        else
        {
            base.OnPursue(ev);
        }
    }

    public override void OnIdle()
    {
        if (mVehicle!=this)
        {
            mVehicle.OnIdle();
            mActorAction.Play("qicheng", null, true);
        }
        else
        {
            base.OnIdle();
        }
    }
}