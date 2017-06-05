using UnityEngine;
using System.Collections;
using System;
using ACT;
using System.Collections.Generic;

public class CharacterSkill : ICharacterComponent
{
    private Character                       mCharacter;
    private ActSkillContainer               mContainer;
    private ActSkill                        mCurrentSkill;
    private ActSkill                        mPassiveSkill;
    private List<ActSkill>                  mNormalAttacks;
    private Dictionary<ESkillPos, ActSkill> mSkillAttacks;
    private Dictionary<int, ActBuff>        mBuffs;
    private int                             mComboIndex = 0;

    public CharacterSkill(Character c)
    {
        this.mCharacter = c;
        this.mContainer = new ActSkillContainer();
        this.mBuffs = new Dictionary<int, ActBuff>();
        this.mSkillAttacks = new Dictionary<ESkillPos, ActSkill>();
        this.mNormalAttacks = new List<ActSkill>();
        this.mContainer.ActorID = c.ID;
        this.mContainer.LoadDoc();
        for (int i = 0; i < mContainer.Skills.Count; i++)
        {
            ActSkill skill = mContainer.Skills[i];
            if (skill.Pos == ESkillPos.Skill_0)
            {
                mNormalAttacks.Add(skill);
            }
            else
            {
                mSkillAttacks[skill.Pos] = skill;
            }
        }
    }

    public void     StartupComponent()
    {
        
    }

    public void     ExecuteComponent()
    {
        if (mCurrentSkill != null)
        {
            mCurrentSkill.Loop();
        }
        if (mCurrentSkill != null && mCurrentSkill.Status == EActStatus.SUCCESS)
        {
            ExitCurrentSkill();
        }
    }

    public void     ReleaseComponent()
    {

    }

    public void     ExitCurrentSkill()
    {
        if (mCurrentSkill != null)
        {
            mCurrentSkill.Clear();
            mCurrentSkill = null;
        }
    }

    public void     StopCurrentSkill()
    {
        if (mCurrentSkill != null)
        {
            mCurrentSkill.Clear();
            mCurrentSkill = null;
        }
    }

    public void     UseSkillById(int id)
    {
        ActSkill skill = GetSkill(id);
        if (skill == null)
        {
            return;
        }
        if (skill.Pos == ESkillPos.Skill_0)
        {
            if (mComboIndex < mNormalAttacks.Count - 1)
            {
                mComboIndex++;
            }
            else
            {
                mComboIndex = 0;
            }
        }
        mCurrentSkill = skill;
        mCurrentSkill.Caster = mCharacter;
        mCurrentSkill.Target = mCharacter.FindEnemy();
    }

    public ActSkill GetSkill(int id)
    {
        for (int i = 0; i < mContainer.Skills.Count; i++)
        {
            if (mContainer.Skills[i].ID == id)
            {
                return mContainer.Skills[i];
            }
        }
        return null;
    }

    public ActSkill GetSkill(ESkillPos pos)
    {
        if (pos == ESkillPos.Skill_0)
        {
            return mNormalAttacks.Count > mComboIndex ? mNormalAttacks[mComboIndex] : null;
        }
        else
        {
            ActSkill skill = null;
            mSkillAttacks.TryGetValue(pos, out skill);
            return skill;
        }
    }

    public ActSkill GetCurrent()
    {
        return mCurrentSkill;
    }

    public ActSkill GetPassive()
    {
        return mPassiveSkill;
    }

    public bool AddBuff(int id, Character caster)
    {
        Debug.LogError("Add Buff " + id);
        return true;
    }

    public bool     DelBuff(int id)
    {
        return false;
    }
}
