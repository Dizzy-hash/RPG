using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cfg.Task;
using System;

public class GTTaskManager : GTSingleton<GTTaskManager>
{
    private List<int> mThreadTaskList = new List<int>();
    private List<int> mBranchTaskList = new List<int>();
    private List<int> mDailyTaskList = new List<int>();

    public ITask             CurThreadTask      { get; private set; }
    public ITask             CurBranchTask      { get; private set; }
    public TaskData          CurThreadCfg;
    public TaskData          CurBranchCfg;
    public XThreadTask       CurThreadData      { get; private set; }
    public XBranchTask       CurBranchData      { get; private set; }
    public TaskFindLocation  CurFindLocation    { get; private set; }
    public int               SelectAwardIndex   { get; set; }

    public override void Init()
    {
        Dictionary<int, CfgTask>.Enumerator em = GTConfigManager.Instance.RdCfgTask.Dict.GetEnumerator();
        while (em.MoveNext())
        {
            int key = em.Current.Key;
            ETaskType type = em.Current.Value.Type;
            switch (type)
            {
                case ETaskType.THREAD:
                    mThreadTaskList.Add(key);
                    break;
                case ETaskType.BRANCH:
                    mBranchTaskList.Add(key);
                    break;
                case ETaskType.DAILY:
                    mDailyTaskList.Add(key);
                    break;
            }
        }
        CurThreadCfg = new TaskData();
        CurBranchCfg = new TaskData();
        CurFindLocation = new TaskFindLocation();
        SelectAwardIndex = 0;
    }

    public void InitData()
    {
        InitThreadTaskData();
        InitBranchTaskData();
    }

    private void CreateSubTask(SubTaskBase pData,int pTaskID,int pTaskStep)
    {
        switch (pData.Func)
        {
            case ETaskSubFuncType.TYPE_TALK:
                {
                    TaskTalk task = new TaskTalk();
                    CurThreadTask = task;
                    task.Accept(pData as SubTalk, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_HUNTER:
                {
                    TaskKillMonster task = new TaskKillMonster();
                    CurThreadTask = task;
                    task.Accept(pData as SubKillMonster, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_COLLECT:
                {
                    TaskCollectItem task = new TaskCollectItem();
                    CurThreadTask = task;
                    task.Accept(pData as SubCollectItem, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_INTERACTIVE:
                {
                    TaskInterActive task = new TaskInterActive();
                    CurThreadTask = task;
                    task.Accept(pData as SubInterActive, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_CONVOY:
                {
                    TaskConvoy task = new TaskConvoy();
                    CurThreadTask = task;
                    task.Accept(pData as SubConvoy, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_STORY:
                {
                    TaskTriggerPlot task = new TaskTriggerPlot();
                    CurThreadTask = task;
                    task.Accept(pData as SubTriggerPlot, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_CUTSCENE:
                {
                    TaskTriggerCutscene task = new TaskTriggerCutscene();
                    CurThreadTask = task;
                    task.Accept(pData as SubTriggerCutscene, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_USEITEM:
                {
                    TaskUseItem task = new TaskUseItem();
                    CurThreadTask = task;
                    task.Accept(pData as SubUseItem, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_USESKILL:
                {
                    TaskUseSkill task = new TaskUseSkill();
                    CurThreadTask = task;
                    task.Accept(pData as SubUseSkill, pTaskID, pTaskStep);
                }
                break;
            case ETaskSubFuncType.TYPE_GATHER:
                {
                    TaskGather task = new TaskGather();
                    CurThreadTask = task;
                    task.Accept(pData as SubGather, pTaskID, pTaskStep);
                }
                break;
            default:
                {
                    CurThreadTask = null;
                }
                break;
        }

    }

    private void LoadTaskScriptByTaskID(ref TaskData data, int pTaskID)
    {
        data.SubTasks.Clear();
        data.Load(GTTools.Format("Text/Task/{0}", pTaskID));
    }

    private void InitThreadTaskData()
    {
        this.CurThreadData = GTDataManager.Instance.DataThreadTask.GetDataById(CfgTask.TASK_THREAD);
        if(this.CurThreadData==null)
        {
            this.CurThreadData = new XThreadTask();
        }
        if(CurThreadData.CurTaskID==0)
        {
            CurThreadData.CurTaskID = mThreadTaskList[0];
        }
        this.LoadTaskScriptByTaskID(ref CurThreadCfg,CurThreadData.CurTaskID);
        this.LoadThreadTask();
    }

    private void LoadThreadTask()
    {
        if (CurThreadData.CurTaskStep>=CurThreadCfg.SubTasks.Count)
        {
            CurThreadData.CurTaskID++;
            CurThreadData.CurTaskStep = 0;
            this.LoadTaskScriptByTaskID(ref CurThreadCfg, CurThreadData.CurTaskID);
        }
        if (CurThreadCfg.SubTasks.Count > 0 && CurThreadData.CurTaskStep < CurThreadCfg.SubTasks.Count)
        {
            CreateSubTask(CurThreadCfg.SubTasks[CurThreadData.CurTaskStep], CurThreadData.CurTaskID, CurThreadData.CurTaskStep);
        }
        else
        {
            CurThreadTask = null;
        }
    }

    private void InitBranchTaskData()
    {

    }

    public List<int> GetAllTaskByTaskType(ETaskType type)
    {
        switch (type)
        {
            case ETaskType.THREAD:
                return mThreadTaskList;
            case ETaskType.BRANCH:
                return mBranchTaskList;
            case ETaskType.DAILY:
                return mDailyTaskList;
        }
        return null;
    }

    public List<int> GetDisplayTaskListByType(ETaskType type)
    {
        List<int> list = new List<int>();
        switch (type)
        {
            case ETaskType.THREAD:
                {
                    if (CurThreadTask != null)
                    {
                        list.Add(CurThreadData.CurTaskID);
                    }
                }
                break;
            case ETaskType.BRANCH:
                {
                    if (CurBranchData != null)
                    {
                        list.Add(CurBranchData.Id);
                    }
                }
                break;
            case ETaskType.DAILY:
                {
                    List<int> pTypeTaskList = GetAllTaskByTaskType(type);
                    for (int i = 0; i < pTypeTaskList.Count; i++)
                    {
                        int key = pTypeTaskList[i];
                        CfgTask db = GTConfigManager.Instance.RdCfgTask.GetCfgById(key);
                        XDailyTask data = GTDataManager.Instance.DataDailyTask.GetDataById(key);
                        if (data == null ||
                            data.State == (int)ETaskState.QUEST_DOING ||
                            data.State == (int)ETaskState.QUEST_CANSUBMIT)
                        {
                            list.Add(key);
                        }
                    }
                }
                break;
        }
        return list;
    }

    public void TriggerTaskByTargetType(ETaskTargetType type,int count)
    {
        Dictionary<int, CfgTask>.Enumerator em = GTConfigManager.Instance.RdCfgTask.Dict.GetEnumerator();
        int pRoleLevel = GTDataManager.Instance.GetCurRole().Level;
        while (em.MoveNext())
        {
            CfgTask db = em.Current.Value;
            int taskID = em.Current.Key;
            if (db.TargetType != type)
            {
                continue;
            }
            if (db.MinRquireLevel > pRoleLevel)
            {
                continue;
            }
            if (db.MaxRquireLevel > 0 && pRoleLevel > db.MaxRquireLevel)
            {
                continue;
            }
            switch (em.Current.Value.Type)
            {
                case ETaskType.DAILY:
                    {
                        XDailyTask data = GTDataManager.Instance.DataDailyTask.GetDataById(taskID);
                        if (data == null)
                        {
                            data = new XDailyTask();
                            data.Id = taskID;
                            data.State = (int)ETaskState.QUEST_DOING;       
                        }
                        data.Count += count;
                        if (data.Count >= db.Condition)
                        {
                            data.Count = count;
                            data.State = (int)ETaskState.QUEST_CANSUBMIT;
                        }
                        GTDataManager.Instance.DataDailyTask.Update(taskID, data);
                    }
                    break;
            }

        }
        em.Dispose();
    }

    public void RecvSubmitTask(int pTaskID,int pSubIndex)
    {
        CfgTask db = GTConfigManager.Instance.RdCfgTask.GetCfgById(pTaskID);
        switch(db.Type)
        {
            case ETaskType.THREAD:
                {
                    CurThreadData.CurTaskStep++;
                    GTDataManager.Instance.DataThreadTask.Update(CfgTask.TASK_THREAD, CurThreadData);
                    LoadThreadTask();
                    GTEventCenter.FireEvent(GTEventID.UPDATE_THREAD_TASK_STATE);
                    if (pSubIndex<CurThreadCfg.SubTasks.Count)
                    {
                        return;
                    }
                }
                break;
            case ETaskType.BRANCH:
                {

                }
                break;
            case ETaskType.DAILY:
                {
                    XDailyTask data = GTDataManager.Instance.DataDailyTask.GetDataById(pTaskID);
                    data.State = (int)ETaskState.QUEST_HASSUBMIT;
                    GTDataManager.Instance.DataDailyTask.Update(pTaskID, data);
                }
                break;
        }

        if (db.RewardMoneyNum > 0)
        {
            GTDataManager.Instance.AddMoney(1, db.RewardMoneyNum);
        }
        if (db.RewardExpNum > 0)
        {
            RoleMediator.Instance.TryAddHeroExp(db.RewardExpNum);
        }
        if (db.AwardID > 0)
        {
            GTAward.Instance.OnReceiveAward(db.AwardID, true);
        }
    }

    public void DoThreadNext()
    {
        if(CurThreadTask!=null)
        {
            CurThreadTask.Start();
        }
    }

    public void DoBranchNext()
    {
        if (CurBranchTask != null)
        {
            CurBranchTask.Start();
        }
    }

    public void Step()
    {

    }

}
