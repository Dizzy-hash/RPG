using UnityEngine;
using System.Collections;
using Protocol;
using ACT;

public class DctHelper
{
    public static bool CalcDamage(Character attacker, Character defender, ActSkill skill, EDamageType type, float percent, int fixValue, bool ignoreDefense)
    {
        XDamage dmg = new XDamage();
        dmg.AttackerName = attacker.Name;
        dmg.DefenderName = defender.Name;
        dmg.Skill = skill.Name;
        dmg.Type = (int)type;
        switch (type)
        {
            case EDamageType.TYPE_PHYSICS:
                dmg.Value = (int)(percent * attacker.CurrAttr.AP + fixValue);
                break;
            case EDamageType.TYPE_DARK:
                dmg.Value = (int)(percent * (attacker.CurrAttr.AP + attacker.CurrAttr.BAP) + fixValue);
                break;
            case EDamageType.TYPE_ICE:
                dmg.Value = (int)(percent * (attacker.CurrAttr.AP + attacker.CurrAttr.IAP) + fixValue);
                break;
            case EDamageType.TYPE_LIGHT:
                dmg.Value = (int)(percent * (attacker.CurrAttr.AP + attacker.CurrAttr.LAP) + fixValue);
                break;
            case EDamageType.TYPE_FIRE:
                dmg.Value = (int)(percent * (attacker.CurrAttr.AP + attacker.CurrAttr.FAP) + fixValue);
                break;
        }
        dmg.Value = (int)(UnityEngine.Random.Range(0.80f, 0.95f) * dmg.Value);
        CalcDamage(attacker, defender, dmg, ignoreDefense);
        return true;
    }

    public static bool CalcDamage(Character attacker, Character defender, XDamage dmg, bool ignoreDefense)
    {
        int hurtValue = dmg.Value;
        Vector3 defenderPos = defender.Pos;
        defender.BeDamage(hurtValue);
        if (hurtValue > 0 && attacker.IsMain)
        {
            bool crit = hurtValue % 3 == 0;
            if(crit)
            {
                GTWorld.Instance.Fly.Show(hurtValue.ToString(), defenderPos, EFlyWordType.TYPE_AVATAR_CRIT);
            }
            else
            {
                GTWorld.Instance.Fly.Show(hurtValue.ToString(), defenderPos, EFlyWordType.TYPE_AVATAR_HURT);
            }
        }
        if (defender.IsMain)
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_UD_AVATAR_HP);
        }
        return true;
    }

    public static bool CalcHeal(Character attacker, Character defender, ActSkill skill, float percent, int fixValue)
    {
        int calcValue = (int)(percent * attacker.CurrAttr.AP + fixValue);
        int healValue = defender.AddHP(calcValue);
        if (healValue > 0 && attacker.IsMain)
        {
            GTWorld.Instance.Fly.Show(healValue.ToString(), defender.Pos, EFlyWordType.TYPE_AVATAR_HEAL);
        }
        if (defender.IsMain)
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_UD_AVATAR_HP);
        }
        return true;
    }

    public static bool CalcBackMagic(Character attacker, Character defender, ActSkill skill, float percent, int fixValue)
    {
        int calcValue = (int)(percent * attacker.CurrAttr.AP + fixValue);
        int healValue = defender.AddMP(calcValue);
        if (healValue > 0 && attacker.IsMain)
        {
            GTWorld.Instance.Fly.Show(healValue.ToString(), defender.Pos, EFlyWordType.TYPE_AVATAR_BACKMAGIC);
        }
        if (defender.IsMain)
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_UD_AVATAR_MP);
        }
        return true;
    }

    public static bool CalcSuckBlood(Character attacker, Character defender, int fixValue, float percent)
    {
        int calcValue = (int)(percent * attacker.CurrAttr.AP + fixValue);
        Vector3 defenderPos = defender.Pos;
        int hurtValue = defender.BeDamage(calcValue);
        int healValue = attacker.AddHP(hurtValue);
        if (hurtValue > 0 && attacker.IsMain)
        {
            GTWorld.Instance.Fly.Show(hurtValue.ToString(), defenderPos, EFlyWordType.TYPE_AVATAR_HURT);
        }
        if (healValue > 0 && attacker.IsMain)
        {
            GTWorld.Instance.Fly.Show(healValue.ToString(), defenderPos, EFlyWordType.TYPE_AVATAR_HEAL);
        }
        if (defender.IsMain || attacker.IsMain)
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_UD_AVATAR_HP);
        }
        return true;
    }

    public static bool CalcAddBuff(Character attacker, Character defender, int buffID)
    {
        defender.Skill.AddBuff(buffID, attacker);
        return true;
    }


}
