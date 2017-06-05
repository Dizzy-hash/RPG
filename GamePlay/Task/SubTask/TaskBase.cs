using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public interface ITask
{
    void Start();
    void Execute();
    void Finish();
    void Reset();
    void Stop();
}

public abstract class TaskBase<T> :ITask where T:SubTaskBase
{
    public T Data { get; protected set; }
    public ESubTaskState State { get; set; }
    public int TaskID { get; private set; }
    public int TaskStep { get; private set; }

    public virtual void Accept(T pData, int pTaskID, int pStep)
    {
        this.Data = pData;
        this.TaskID = pTaskID;
        this.TaskStep = pStep;
        this.State = ESubTaskState.TYPE_INIT;
    }

    public virtual void Start()
    {

    }
    public virtual void Execute()
    {

    }
    public virtual void Stop()
    {

    }
    public virtual void Reset()
    {

    }

    public virtual void Finish()
    {
        TaskMediator.Instance.OnSubmitDailyTask(TaskID, TaskStep);
    }
}
