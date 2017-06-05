using UnityEngine;
using System.Collections;
using System;
using Protocol;
using ACT;

public class CharacterCtrl : GTSingleton<CharacterCtrl>,ICtrl
{
    public void AddEventListeners()
    {
        GTEventCenter.AddHandler<int, int>    (GTEventID.TYPE_UNLOAD_EQUIP,       OnAvatarChangeEquipAvatar);
        GTEventCenter.AddHandler<int, int>    (GTEventID.TYPE_DRESS_EQUIP,        OnAvatarChangeEquipAvatar);
        GTEventCenter.AddHandler              (GTEventID.TYPE_START_MOUNT,        OnAvatarStartMount);
        GTEventCenter.AddHandler              (GTEventID.TYPE_LEAVE_MOUNT,        OnAvatarLeaveMount);
        GTEventCenter.AddHandler              (GTEventID.TYPE_CHANGE_HEROLEVEL,   OnAvatarUpgradeLevel);
        GTEventCenter.AddHandler              (GTEventID.TYPE_CHANGE_FIGHTVALUE,  OnAvatarChangeFightValue);
        GTEventCenter.AddHandler<int, int>    (GTEventID.TYPE_KILL_MONSTER,       OnAvatarKillMonster);
        GTEventCenter.AddHandler<int, int>    (GTEventID.TYPE_CHANGE_PARTNER,     OnAvatarChangePartner);

        GTEventCenter.AddHandler<float, float>(GTEventID.TYPE_MOVE_JOYSTICK,      OnAvatarMoveJoystick);
        GTEventCenter.AddHandler              (GTEventID.TYPE_STOP_JOYSTICK,      OnAvatarStopJoystick);
        GTEventCenter.AddHandler<ESkillPos>   (GTEventID.TYPE_CAST_SKILL,         OnAvatarCastSkill);
        GTEventCenter.AddHandler              (GTEventID.TYPE_JUMP,               OnAvatarJump);
        GTEventCenter.AddHandler<Vector3>     (GTEventID.TYPE_MOVE_PURSUE,        OnAvatarPursue);
        GTEventCenter.AddHandler<EDeadReason> (GTEventID.TYPE_AVATAR_DEAD,        OnAvatarDead);
    } 

    public void DelEventListeners()
    {
        GTEventCenter.DelHandler<int, int>    (GTEventID.TYPE_UNLOAD_EQUIP,      OnAvatarChangeEquipAvatar);
        GTEventCenter.DelHandler<int, int>    (GTEventID.TYPE_DRESS_EQUIP,       OnAvatarChangeEquipAvatar);
        GTEventCenter.DelHandler              (GTEventID.TYPE_START_MOUNT,       OnAvatarStartMount);
        GTEventCenter.DelHandler              (GTEventID.TYPE_LEAVE_MOUNT,       OnAvatarLeaveMount);
        GTEventCenter.DelHandler              (GTEventID.TYPE_CHANGE_HEROLEVEL,  OnAvatarUpgradeLevel);
        GTEventCenter.DelHandler              (GTEventID.TYPE_CHANGE_FIGHTVALUE, OnAvatarChangeFightValue);
        GTEventCenter.DelHandler<int, int>    (GTEventID.TYPE_KILL_MONSTER,      OnAvatarKillMonster);
        GTEventCenter.DelHandler<int, int>    (GTEventID.TYPE_CHANGE_PARTNER,    OnAvatarChangePartner);

        GTEventCenter.DelHandler<float, float>(GTEventID.TYPE_MOVE_JOYSTICK,     OnAvatarMoveJoystick);
        GTEventCenter.DelHandler              (GTEventID.TYPE_STOP_JOYSTICK,     OnAvatarStopJoystick);
        GTEventCenter.DelHandler<ESkillPos>   (GTEventID.TYPE_CAST_SKILL,        OnAvatarCastSkill);
        GTEventCenter.DelHandler              (GTEventID.TYPE_JUMP,              OnAvatarJump);
        GTEventCenter.DelHandler<Vector3>     (GTEventID.TYPE_MOVE_PURSUE,       OnAvatarPursue);
        GTEventCenter.DelHandler<EDeadReason> (GTEventID.TYPE_AVATAR_DEAD,       OnAvatarDead);
    }

    private void OnAvatarChangePartner(int pos, int id)
    {

    }

    private void OnAvatarKillMonster(int guid, int id)
    {

    }

    private void OnAvatarChangeFightValue()
    {
        if (!CheckMe())
        {
            return;
        }
        XCharacter data = RoleModule.Instance.GetMainPlayer();
        CharacterManager.Main.SyncData(data, ESyncDataType.TYPE_BASEATTR);
    }

    private void OnAvatarUpgradeLevel()
    {
        if (!CheckMe())
        {
            return;
        }
        XCharacter data = RoleModule.Instance.GetMainPlayer();
        CharacterManager.Main.SyncData(data, ESyncDataType.TYPE_BOARD);
    }

    private void OnAvatarStartMount()
    {
        if (!CheckMe())
        {
            return;
        }
        XCharacter data = RoleModule.Instance.GetMainPlayer();
        if (data.Mount == 0)
        {
            CalcCharacterOperateError(Resp.TYPE_RIDE_NONE);
            return;
        }
        if (GTLauncher.Instance.CurrSceneType != ESceneType.TYPE_CITY &&
            GTLauncher.Instance.CurrSceneType != ESceneType.TYPE_WORLD)
        {
            CalcCharacterOperateError(Resp.TYPE_RIDE_NOTDOATSCENE);
            return;
        }
        if (CharacterManager.Main.IsFSMLayer2() || CharacterManager.Main.IsFSMLayer3())
        {
            CalcCharacterOperateError(Resp.TYPE_RIDE_NOTDOATFSM);
            return;
        }
        if (CharacterManager.Main.IsRide)
        {
            CalcCharacterOperateError(Resp.TYPE_RIDE_ING);
            return;
        }
        if (CharacterManager.Main.FSM == FSMState.FSM_SKILL)
        {
            CalcCharacterOperateError(Resp.TYPE_SKILL_CASTING);
            return;
        }
        Vector3    pos      = CharacterManager.Main.Pos;
        Vector3    euler    = CharacterManager.Main.Euler;
        KTransform bornData = KTransform.Create(pos, euler);
        CharacterManager.Main.Mount = CharacterManager.Instance.AddActorNoneSync(data.Mount, EBattleCamp.D, EActorType.MOUNT, bornData);
        CharacterManager.Main.Mount.Host = CharacterManager.Main;
        Resp resp = CharacterManager.Main.Command.Get<CommandRideBegin>().Do();
        CalcCharacterOperateError(resp);
    }

    private void OnAvatarLeaveMount()
    {
        if (!CheckMe())
        {
            return;
        }
        if (CharacterManager.Main.IsFSMLayer2() || CharacterManager.Main.IsFSMLayer3())
        {
            return;
        }
        if (CharacterManager.Main.IsRide == false)
        {
            return;
        }
        CharacterManager.Instance.SetCharacterParent(CharacterManager.Main);
        CharacterManager.Instance.DelActor(CharacterManager.Main.Mount);
        Resp resp = CharacterManager.Main.Command.Get<CommandRideLeave>().Do();
        CharacterManager.Main.Mount = null;
        CalcCharacterOperateError(resp);    
    }

    private void OnAvatarChangeEquipAvatar(int newPos, int tarPos)
    {
        if (!CheckMe())
        {
            return;
        }
        XCharacter data = RoleModule.Instance.GetMainPlayer();
        CharacterManager.Main.SyncData(data, ESyncDataType.TYPE_AVATAR);
    }

    private void OnAvatarJump()
    {
        if (!CheckMe())
        {
            return;
        }
        Resp resp = CharacterManager.Main.Command.Get<CommandJump>().Do();
        CalcCharacterOperateError(resp);
    }

    private void OnAvatarCastSkill(ESkillPos pos)
    {
        if (!CheckMe())
        {
            return;
        }
        ActSkill skill = CharacterManager.Main.Skill.GetSkill(pos);
        if (skill == null)
        {
            return;
        }
        Resp resp = CharacterManager.Main.Command.Get<CommandUseSkill>().Update(skill.ID).Do();
        CalcCharacterOperateError(resp);
        if (resp == Resp.TYPE_YES)
        {
            CharacterManager.Main.UsePowerByCostType(skill.CostType, skill.CostNum);
            GTEventCenter.FireEvent(GTEventID.TYPE_UD_AVATAR_HP);
            GTEventCenter.FireEvent(GTEventID.TYPE_UD_AVATAR_MP);
        }
    }

    private void OnAvatarStopJoystick()
    {
        if (!CheckMe())
        {
            return;
        }
        if (CharacterManager.Main.IsFSMLayer2())
        {
            return;
        }
        Resp resp;
        if (CharacterManager.Main.IsRide)
        {
            resp = CharacterManager.Main.Mount.Command.Get<CommandIdle>().Do();
        }
        else
        {
            resp = CharacterManager.Main.Command.Get<CommandIdle>().Do();
        }
        CalcCharacterOperateError(resp);
    }

    private void OnAvatarMoveJoystick(float x, float y)
    {
        if (!CheckMe())
        {
            return;
        }
        if (CharacterManager.Main.IsFSMLayer2())
        {
            return;
        }
        CameraController cam = GTCameraManager.Instance.MainCamera.GetComponent<CameraController>();
        Quaternion q = new Quaternion();
        float r = Mathf.Deg2Rad * cam.transform.eulerAngles.y * 0.5f;
        q.w = Mathf.Cos(r);
        q.x = 0;
        q.y = Mathf.Sin(r);
        q.z = 0;
        Vector3 motion = q * new Vector3(x, 0, y);
        Resp resp;
        if (CharacterManager.Main.IsRide)
        {
            resp = CharacterManager.Main.Mount.Command.Get<CommandMove>().Update(motion).Do();
        }
        else
        {
            resp = CharacterManager.Main.Command.Get<CommandMove>().Update(motion).Do();
        }
        CalcCharacterOperateError(resp);
    }

    private void OnAvatarPursue(Vector3 destPosition)
    {
        if (!CheckMe())
        {
            return;
        }
        if (CharacterManager.Main.IsFSMLayer2())
        {
            return;
        }
        Resp resp;
        if (CharacterManager.Main.IsRide)
        {
            resp = CharacterManager.Main.Mount.Command.Get<CommandMove>().Update(destPosition, null).Do();
        }
        else
        {
            resp = CharacterManager.Main.Command.Get<CommandMove>().Update(destPosition, null).Do();
        }
        CalcCharacterOperateError(resp);
    }

    private void OnAvatarDead(EDeadReason reason)
    {

    }

    private void OnAvatarBeDamage()
    {

    }

    private void OnAvatarAddBuff()
    {

    }

    private void OnAvatarDelBuff()
    {

    }

    private bool CheckMe()
    {
        if (CharacterManager.Main == null) return false;
        if (CharacterManager.Main.Obj == null) return false;
        return true;
    }

    private void CalcCharacterOperateError(Resp resp)
    {
        if (resp == Resp.TYPE_NO || resp == Resp.TYPE_YES)
        {
            return;
        }
        string key = resp.ToString();
        DLocalString db = ReadCfgLocalString.GetDataById(key);
        if (db == null)
        {
            return;
        }
        GTItemHelper.ShowTip(db.Value);
    }
}
