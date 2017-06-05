using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskUseItem : TaskBase<SubUseItem>
{
    private int mCurTimes = 0;

    public override void Accept(SubUseItem pData, int pTaskID, int pStep)
    {
        base.Accept(pData, pTaskID, pStep);
        GTEventCenter.AddHandler<int,int>(GTEventID.RECV_USE_ITEM, OnUseItem);
    }

    public override void Start()
    {

    }

    public override void Execute()
    {

    }

    public override void Finish()
    {
        base.Finish();
        GTEventCenter.DelHandler<int, int>(GTEventID.RECV_USE_ITEM, OnUseItem);
    }

    public override void Reset()
    {
        mCurTimes = 0;
    }

    public override void Stop()
    {

    }

    private void OnUseItem(int id,int num)
    {
        if (id == Data.ID)
        {
            mCurTimes = mCurTimes + num;
        }
        if (mCurTimes >= Data.Times)
        {
            mCurTimes = Data.Times;
            Finish();
        }
    }
}
