﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorPathFinding:IGame
{
    private Actor mOwner;
    private UnityEngine.AI.NavMeshAgent mNavMeshAgent;
    private UnityEngine.AI.NavMeshPath mNavMeshPath;
    private GameObject mGameObject;
    private Vector3  mDestPosition;
    private Callback mOnFinished;

    public bool CheckReached()
    {
        if(!mNavMeshAgent.enabled)
        {
            return false;
        }
        return mNavMeshAgent.remainingDistance <= 1f && mNavMeshAgent.remainingDistance > 0.01f;
    }

    public ActorPathFinding(Actor owner)
    {
        mOwner = owner;
        mGameObject = mOwner.Obj;
        this.mNavMeshAgent = mGameObject.GET<UnityEngine.AI.NavMeshAgent>();
        this.mNavMeshPath = new UnityEngine.AI.NavMeshPath();
        mNavMeshAgent.enabled = false;
        mNavMeshAgent.radius = mOwner.Radius;
        mNavMeshAgent.height = mOwner.Height;
        mNavMeshAgent.acceleration = 360;
        mNavMeshAgent.angularSpeed = 360;
        mNavMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    public void SetAgentActive(bool active)
    {
        this.mNavMeshAgent.enabled = active;
    }

    public void SetDestPosition(Vector3 dest)
    {
        mDestPosition = dest;
        SetAgentActive(true);
        this.mNavMeshAgent.speed = mOwner.GetAttr(EAttr.RSpeed);
        mNavMeshAgent.SetDestination(mDestPosition);
    }

    public void Execute()
    {
        if (mNavMeshAgent.enabled == false)
        {
            return;
        }
        if(!mOwner.GetActorEffect(EActorEffect.IS_AUTOMOVE))
        {
            return;
        }
        if(CheckReached())
        {
            mOwner.OnArrive();
            if (mOnFinished != null)
            {
                mOnFinished();
                mOnFinished = null;
            }
        }
    }

    public void Startup()
    {

    }

    public void Release()
    {

    }

    public void StopPathFinding()
    {
        if (mNavMeshAgent.enabled == true)
        {
            mNavMeshAgent.Stop();
            SetAgentActive(false);
        }
    }

    public bool CanReachPosition(Vector3 dest)
    {
        Vector3 position = GTTools.NavSamplePosition(dest);
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.CalculatePath(position, mNavMeshPath);
        if (mNavMeshPath.status != UnityEngine.AI.NavMeshPathStatus.PathPartial)
        {
            return true;
        }
        return false;
    }

    public void SetOnFinished(Callback pCallback)
    {
        mOnFinished = pCallback;
    }


}
