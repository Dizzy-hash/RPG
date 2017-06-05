using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskGather : TaskBase<SubGather>
{
    private int mCurCount = 0;

    public override void Accept(SubGather pData, int pTaskID, int pStep)
    {
        base.Accept(pData, pTaskID, pStep);
        GTEventCenter.AddHandler<int, int, int>(GTEventID.RECV_GATHER_MINE, OnRecvGatherMine);
    }

    private void OnRecvGatherMine(int guid, int id,int count)
    {
        if (id == Data.ID)
        {
            mCurCount++;
        }
        if (mCurCount >= Data.Count)
        {
            Finish();
        }
    }

    public override void Start()
    {
        GTTaskManager.Instance.CurFindLocation.Init(Data.Location, OnFinishPathFind);
    }

    private void OnFinishPathFind()
    {

    }

    public override void Execute()
    {

    }

    public override void Finish()
    {
        base.Finish();
        GTEventCenter.DelHandler<int, int, int>(GTEventID.RECV_GATHER_MINE, OnRecvGatherMine);
    }

    public override void Reset()
    {
        mCurCount = 0;
    }

    public override void Stop()
    {

    }
}
