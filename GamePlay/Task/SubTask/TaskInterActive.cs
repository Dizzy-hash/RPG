using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskInterActive : TaskBase<SubInterActive>
{
    private float mProgress = -1;
    private float mSpeed = 1;

    public override void Accept(SubInterActive pData, int pTaskID, int pStep)
    {
        base.Accept(pData, pTaskID, pStep);
    }

    public override void Start()
    {
        GTTaskManager.Instance.CurFindLocation.Init(Data.Npc.Location, OnFinishPathFind);
    }

    private void OnFinishPathFind()
    {
        GTWindowManager.Instance.OpenWindow(EWindowID.UI_TASKINTERACTIVE);
        UITaskInterActive pWindow = (UITaskInterActive)GTWindowManager.Instance.GetWindow(EWindowID.UI_TASKINTERACTIVE);
        pWindow.Init(Data.Desc,DoCmd);
    }

    private void DoCmd()
    {
        if (mProgress < 0)
        {
            mProgress = 0;
            float length = LevelData.MainPlayer.GetActorAction().GetLength(Data.Cmd);
            if (length == 0)
            {
                length = 1;
            }
            mSpeed = 1 * Time.deltaTime / length;
        }
    }

    public override void Execute()
    {
        if (mProgress < 0)
        {
            return;
        }
        if (mProgress < 1)
        {
            mProgress += mSpeed;
            UITaskInterActive pWindow = (UITaskInterActive)GTWindowManager.Instance.GetWindow(EWindowID.UI_TASKINTERACTIVE);
            pWindow.UpdateProgress(mProgress);
        }
        else
        {
            Finish();
        }

    }

    public override void Finish()
    {
        base.Finish();
        GTWindowManager.Instance.GetWindow(EWindowID.UI_TASKINTERACTIVE).Hide();    
    }

    public override void Reset()
    {
        mProgress = -1;
    }

    public override void Stop()
    {
        mProgress = -1;
    }
}
