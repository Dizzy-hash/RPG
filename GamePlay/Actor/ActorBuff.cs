using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActorBuff:IGame
{
    private List<int> mDelBuffList = new List<int>();
    private List<int> mAddBuffList = new List<int>();
    private Dictionary<int, BuffBase> mBuffMap = new Dictionary<int, BuffBase>();
    private Actor mOwner;

    public ActorBuff(Actor owner)
    {
        this.mOwner = owner;
    }

    public void AddBuff(int id,Actor caster)
    {
        if (mOwner.IsDead()) { return; }
        CfgBuff db = GTConfigManager.Instance.RdCfgBuff.GetCfgById(id);
        if (db == null) { return; }

        BuffBase commonBuff = FindBuff(id);
        if (commonBuff != null)
        {
            switch (commonBuff.Data.OverlayType)
            {
                case EBuffOverlayType.Overlay:
                    commonBuff.Overlay();
                    break;
                case EBuffOverlayType.Reset:
                    commonBuff.Refresh();
                    break;
                case EBuffOverlayType.Cancle:
                    mDelBuffList.Add(commonBuff.Id);
                    break;
            }
            return;
        }
        Resp reply = Resp.N;
        switch (db.Result)
        {
            case EActType.TYPE_ADDATTR:
            case EActType.TYPE_SUBATTR:
            case EActType.TYPE_LDDATTR:
            case EActType.TYPE_LUBATTR:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_SUPER:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_VARIATION:
                {
                    reply = mOwner.Command.Get<CommandVariation>().Update(db.LifeTime, db.ChangeModelID).Do();
                }
                break;
            case EActType.TYPE_STUN:
                {
                    reply = mOwner.Command.Get<CommandStun>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_FIXBODY:
                {
                    reply = mOwner.Command.Get<CommandFixBody>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_STEALTH:
                {
                    reply = mOwner.Command.Get<CommandStealth>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_FROZEN:
                {
                    reply = mOwner.Command.Get<CommandFrost>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_BLIND:
                {
                    reply = mOwner.Command.Get<CommandBlind>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_SILENT:
                {
                    reply = mOwner.Command.Get<CommandSilent>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_SLEEP:
                {
                    reply = mOwner.Command.Get<CommandSleep>().Update(db.LifeTime).Do();
                }
                break;
            case EActType.TYPE_ABSORB:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_WILD:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_DIVIVE:
                {
                    RemoveAllDebuff();
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_PARALY:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_FEAR:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_REFLEX:
                {
                    reply = Resp.Y;
                }
                break;
            case EActType.TYPE_DEAD:
                {
                    reply = Resp.Y;
                }
                break;
        }

        if(reply==Resp.Y)
        {
            BuffBase buff = new BuffBase(id, mOwner, caster);
            mBuffMap.Add(id, buff);
        }

        mOwner.UpdateCurrAttr();
    }

    private BuffBase FindBuff(int id)
    {
        BuffBase buff = null;
        mBuffMap.TryGetValue(id, out buff);
        return buff;
    }

    private void RemoveAllDot()
    {
        foreach (var current in mBuffMap)
        {
            if (current.Value.Data.Result == EActType.TYPE_LUBATTR)
            {
                mDelBuffList.Add(current.Key);
            }
        }
    }

    private void RemoveAllDebuff()
    {
        foreach (var current in mBuffMap)
        {
            if (current.Value.Data.BuffType == EBuffType.Debuff)
            {
                mDelBuffList.Add(current.Key);
            }
        }
    }

    private void RemoveAllControl()
    {
        foreach (var current in mBuffMap)
        {
            if (IsControlBuff(current.Value))
            {
                mDelBuffList.Add(current.Key);
            }
        }
    }

    private void RemoveAllBuff()
    {
        foreach (var current in mBuffMap)
        {
            mDelBuffList.Add(current.Key);
        }
    }

    private bool IsControlBuff(BuffBase buff)
    {
        switch (buff.Data.Result)
        {
            case EActType.TYPE_BLIND:
            case EActType.TYPE_FEAR:
            case EActType.TYPE_FIXBODY:
            case EActType.TYPE_FROZEN:
            case EActType.TYPE_PARALY:
            case EActType.TYPE_SLEEP:
            case EActType.TYPE_STUN:
            case EActType.TYPE_VARIATION:
                return true;
        }
        return false;
    }

    public void Execute()
    {
        if (mBuffMap.Count > 0)
        {
            foreach(var current in mBuffMap)
            {
                current.Value.Update();
                if (current.Value.IsDead)
                {
                    mDelBuffList.Add(current.Key);
                }
            }
        }
        for (int i = 0; i < mDelBuffList.Count; i++)
        {
            int id = mDelBuffList[i];
            if (mBuffMap.ContainsKey(id))
            {
                mBuffMap[id].Release();
                mBuffMap.Remove(id);
            }
        }
        if (mDelBuffList.Count > 0)
        {
            mOwner.UpdateCurrAttr();
            mDelBuffList.Clear();
        }

        GTEventCenter.FireEvent(GTEventID.UPDATE_AVATAR_BUFF);
    }

    public Dictionary<int,BuffBase> GetAllBuff()
    {
        return mBuffMap;
    }

    public void SetAllParticleEnabled(bool enabled)
    {
        Dictionary<int, BuffBase>.Enumerator em = mBuffMap.GetEnumerator();
        while(em.MoveNext())
        {
            BuffBase buff = em.Current.Value;
            buff.SetParticleEnabled(enabled);
        }
        em.Dispose();
    }

    public void Startup()
    {

    }

    public void Release()
    {
        RemoveAllBuff();
    }
}
