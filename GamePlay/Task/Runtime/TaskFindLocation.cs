using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskFindLocation : ITask
{
    private TaskLocation mLocation;
    private Callback mOnFinish;
    private Callback mOnLoadSceneCallback;

    public TaskFindLocation()
    {
        GTEventCenter.AddHandler(GTEventID.STOP_AUTOTASK,Stop);
    }

    public void Init(TaskLocation pLocation, Callback callback)
    {
        Reset();
        mLocation = pLocation;
        mOnFinish = callback;
        Start();
    }

    public void Reset()
    {
        mOnFinish = null;
    }

    public void Start()
    {
        if (mLocation.MapID == GTLauncher.Instance.CurMapID)
        {
            LevelData.MainPlayer.Command.Get<CommandRunTo>().Update(mLocation.Pos, mOnFinish).Do();
            return;
        }

        Vector3 pPortalPos = Vector3.zero;
        bool success = GTLevelManager.Instance.FindDestMap(mLocation.MapID, ref pPortalPos);
        if (success == false)
        {
            Finish();
            GTItemHelper.ShowTip("200007");
            return;
        }
    }

    public void Execute()
    {

    }

    public void Stop()
    {

    }

    public void Finish()
    {
        Reset();
    }

    public void Clear()
    {
        GTEventCenter.DelHandler(GTEventID.STOP_AUTOTASK, Stop);
    }
}
