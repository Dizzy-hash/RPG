using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorMainPlayer : ActorPlayer
{
    public ActorMainPlayer(int id, int guid, EActorType type, EBattleCamp camp) : base(id, guid, type, camp)
    {

    }

    public override void Load(XTransform data)
    {
        base.Load(data);
        GTEventCenter.AddHandler<int, int>(GTEventID.RECV_UNLOAD_EQUIP,ChangeEquipAvatar);
        GTEventCenter.AddHandler<int, int>(GTEventID.RECV_DRESS_EQUIP, ChangeEquipAvatar);
        GTEventCenter.AddHandler(GTEventID.RECV_PLAYER_END_MOUNT, OnEndRide);
        GTEventCenter.AddHandler(GTEventID.CHANGE_HEROLEVEL, OnUpgradeLevel);
        GTEventCenter.AddHandler(GTEventID.CHANGE_FIGHTVALUE, OnChangeFightValue);
        GTEventCenter.AddHandler<int,int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
        GTEventCenter.AddHandler<int,int>(GTEventID.RECV_CHANGE_PARTNER, OnChangePartner);
    }

    private void OnChangePartner(int pos, int id)
    {
        mActorCard.SetPartnerByPos(pos, id);
        switch (pos)
        {
            case 1:
                GTLevelManager.Instance.DelActor(this.Partner1);
                break;
            case 2:
                GTLevelManager.Instance.DelActor(this.Partner2);
                break;
            case 3:
                GTLevelManager.Instance.DelActor(this.Partner3);
                break;
        }
        GTLevelManager.Instance.AddPartner(this, pos, id);
    }

    private void OnKillMonster(int guid,int id)
    {
        CfgActor db = GTConfigManager.Instance.RdCfgActor.GetCfgById(id);
        if(db.Exp>0)
        {
            RoleMediator.Instance.TryAddHeroExp(db.Exp);
        }
    }

    private void OnUpgradeLevel()
    {
        this.GetActorCard().SetLevel();
        GTBoardManager.Instance.Refresh(this);
        EffectData data = new EffectData();
        data.Owner = this;
        data.Id = GTEffectDefine.EFFECT_UPGRADE;
        data.LastTime = 3;
        data.Bind = EEffectBind.OwnFoot;
        data.Dead = EFlyObjDeadType.UntilLifeTimeEnd;
        data.Parent = CacheTransform;
        data.SetParent = true;
        EffectBase effect = GTEffectManager.Instance.CreateEffect(data);
    }

    public override void UpdateCurrAttr(bool init = false)
    {
        base.UpdateCurrAttr(init);
        GTEventCenter.FireEvent(GTEventID.UPDATE_AVATAR_ATTR);
    }

    public override void Execute()
    {
        base.Execute();
        if (GTLauncher.Instance.CurrSceneType == ESceneType.TYPE_WORLD)
        {
            GTEventCenter.FireEvent(GTEventID.CHANGE_ACTOR_POS, Pos);
        }
    }

    private void OnChangeFightValue()
    {
        InitAttr();
    }

    public override void Destroy()
    {
        base.Destroy();
        GTEventCenter.DelHandler<int, int>(GTEventID.RECV_UNLOAD_EQUIP, ChangeEquipAvatar);
        GTEventCenter.DelHandler<int, int>(GTEventID.RECV_DRESS_EQUIP, ChangeEquipAvatar);
        GTEventCenter.DelHandler(GTEventID.RECV_PLAYER_END_MOUNT, OnEndRide);
        GTEventCenter.DelHandler(GTEventID.CHANGE_HEROLEVEL, OnUpgradeLevel);
        GTEventCenter.DelHandler(GTEventID.CHANGE_FIGHTVALUE, OnChangeFightValue);
        GTEventCenter.DelHandler<int, int>(GTEventID.RECV_KILL_MONSTER, OnKillMonster);
        GTEventCenter.DelHandler<int, int>(GTEventID.RECV_CHANGE_PARTNER, OnChangePartner);
    }
}
