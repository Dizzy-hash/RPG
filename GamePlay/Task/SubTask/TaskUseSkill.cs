using UnityEngine;
using System.Collections;
using Cfg.Task;
using System;

public class TaskUseSkill : TaskBase<SubUseSkill>
{
    private int mCurTimes = 0;

    public override void Accept(SubUseSkill pData, int pTaskID, int pStep)
    {
        base.Accept(pData, pTaskID, pStep);
        GTEventCenter.AddHandler<ESkillPos>(GTEventID.REQT_CAST_SKILL, OnUseSkill);
    }

    private void OnUseSkill(ESkillPos pos)
    {
        if(Data.Pos==pos)
        {
            mCurTimes++;
        }
        if(mCurTimes>=Data.Times)
        {
            Finish();
        }
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
        GTEventCenter.DelHandler<ESkillPos>(GTEventID.REQT_CAST_SKILL, OnUseSkill);
    }

    public override void Reset()
    {
        mCurTimes = 0;
    }

    public override void Stop()
    {

    }
}