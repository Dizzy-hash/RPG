using UnityEngine;
using System.Collections;

public class TaskMediator : GTSingleton<TaskMediator>
{
    public void OnSubmitDailyTask(int id,int step)
    {
        CfgTask db = GTConfigManager.Instance.RdCfgTask.GetCfgById(id);
        if (db == null)
        {
            GTItemHelper.ShowTip("200001");
            return;
        }
        XRole role = GTDataManager.Instance.GetCurRole();
        if (db.MinRquireLevel>GTDataManager.Instance.GetCurRole().Level)
        {
            GTItemHelper.ShowTip("200002");
            return;
        }

        if (db.MaxRquireLevel>0&& role.Level>db.MaxRquireLevel)
        {
            GTItemHelper.ShowTip("200002");
            return;
        }

        XDailyTask data = GTDataManager.Instance.DataDailyTask.GetDataById(id);
        if(data!=null)
        {
            if(data.State==(int)ETaskState.QUEST_HASSUBMIT)
            {
                GTItemHelper.ShowTip("200003");
                return;
            }
            if(data.Count<db.Condition)
            {
                GTItemHelper.ShowTip("200004");
                return;
            }
        }

        GTNetPacketData request = new GTNetPacketData();
        request.ID = GTMessageID.MSG_SUBMIT_TASK;
        request.Parameters["ID"] = id;
        request.Parameters["Step"] = step;
        GTNetworkManager.Instance.SendMessage(request);
    }
}
