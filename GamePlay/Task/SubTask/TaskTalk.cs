using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskTalk : TaskBase<SubTalk>
{
    private Int32 mTalkIndex = -1;

    public override void Accept(SubTalk pData, int pTaskID, int pStep)
    {
        base.Accept(pData, pTaskID, pStep);
    }

    public override void Start()
    {
        State = ESubTaskState.TYPE_ENTER;
        GTTaskManager.Instance.CurFindLocation.Init(Data.Npc.Location, OnFinishPathFind);
    }

    private void OnFinishPathFind()
    {
        GTWindowManager.Instance.OpenWindow(EWindowID.UI_TASKTALK);
        DoNextStep();
    }

    public override void Execute()
    {
        State = ESubTaskState.TYPE_STEP;
    }

    public void DoNextStep()
    {
        if (mTalkIndex<Data.Dialogs.Count-1)
        {
            mTalkIndex++;
            TaskDialog pDialog = Data.Dialogs[mTalkIndex];
            GTWindowManager.Instance.GetWindow<UITaskTalk>(EWindowID.UI_TASKTALK).ShowTalk(pDialog);
        }
        else
        {
            Finish();
        }
    }

    public override void Finish()
    {
        base.Finish();
        State = ESubTaskState.TYPE_EXIT;
        GTWindowManager.Instance.GetWindow<UITaskTalk>(EWindowID.UI_TASKTALK).Hide();
    }

    public override void Stop()
    {
        mTalkIndex = -1;
        State = ESubTaskState.TYPE_STOP;
        GTWindowManager.Instance.GetWindow<UITaskTalk>(EWindowID.UI_TASKTALK).Hide();
    }

    public override void Reset()
    {

    }
}
