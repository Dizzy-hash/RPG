using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskKillMonster : TaskBase<SubKillMonster>
{
    private int mCurCount = 0;

    public override void Accept(SubKillMonster pData, int pTaskID, int pStep)
    {
        base.Accept(pData, pTaskID, pStep);
        GTEventCenter.AddHandler<int,int>(GTEventID.RECV_KILL_MONSTER,OnKillMonster);
    }

    private void OnKillMonster(int guid, int id)
    {
        if(id==Data.ID)
        {
            mCurCount++;
        }
        if(mCurCount>=Data.Count)
        {
            Finish();
        }
    }

    public override void Start()
    {
        Callback pAutoAttack = delegate () { };
        GTTaskManager.Instance.CurFindLocation.Init(Data.Location, pAutoAttack);
    }

    public override void Execute()
    {

    }

    public override void Finish()
    {
        base.Finish();
        GTEventCenter.DelHandler<int, int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
    }

    public override void Reset()
    {
        mCurCount = 0;
    }

    public override void Stop()
    {

    }
}
