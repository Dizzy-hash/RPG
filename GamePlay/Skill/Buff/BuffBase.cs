using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class BuffBase
{
    public int Id { get; private set; }
    public CfgBuff Data { get; private set; }
    public int CurOverlayNum { get; private set; }
    public bool IsDead { get; private set; }

    private float mStartTimer;
    private float mIntervalTimer;
    private Actor mOwner;
    private Actor mCaster;
    private EffectBase mParticle;
    private CfgBuffAttr mBuffAttr;


    public BuffBase(int id, Actor owner, Actor caster)
    {
        this.Id = id;
        this.Data = GTConfigManager.Instance.RdCfgBuff.GetCfgById(id);
        this.mOwner = owner;
        this.mCaster = caster;
        if (this.Data.ResultAttr > 0)
        {
            mBuffAttr = GTConfigManager.Instance.RdCfgBuffAttr.GetCfgById(this.Data.ResultAttr);
        }
        this.Enter();
    }

    public void AddEffect()
    {
        if(Data.EffectID==0)
        {
            return;
        }
        EffectData data = new EffectData();
        data.Bind = Data.EffectBind;
        data.Dead = EFlyObjDeadType.UntilLifeTimeEnd;
        data.Fly = EFlyType.STAY;
        data.LastTime = Data.LifeTime;
        data.Id = Data.EffectID;
        data.Owner = mOwner;
        data.Parent = mOwner.CacheTransform;
        data.SetParent = true;
        mParticle=GTEffectManager.Instance.CreateEffect(data);
    }

    public void ChangeModel()
    {
        if(Data.ChangeModelScale>GTDefine.V_INTERVAL_VALUE)
        {
            Vector3 to = mOwner.CacheTransform.localScale * Data.ChangeModelScale;
            mOwner.CacheTransform.DOScale(to, 1);
        }
    }

    public void Enter()
    {
        if (IsDead) return;
        Refresh();
        ChangeModel();
        AddEffect();
    }

    public void Update()
    {
        if (mOwner.IsDead() || IsDead)
        {
            return;
        }

        if (Data.LifeTime > 0 && Time.realtimeSinceStartup-mStartTimer > Data.LifeTime)
        {
            IsDead = true;
        }

        if (!IsDotOrHot())
        {
            return;
        }

        if (mBuffAttr != null && mIntervalTimer > Data.ResultInterval)
        {
            TriggerIntervalEvent();
            mIntervalTimer = 0;
        }
        else
        {
            mIntervalTimer += Time.deltaTime;
        }
    }

    public void Refresh()
    {
        if(mOwner.IsDead())
        {
            return;
        }   
        if(mParticle!=null)
        {
            mParticle.Reset();
        }
        mStartTimer = Time.realtimeSinceStartup;
    }

    public void Overlay()
    {
        if(CurOverlayNum<Data.MaxOverlayNum)
        {
            CurOverlayNum++;
        }
        Refresh();
    }

    public float GetLeftTime()
    {
        if(Data.LifeTime<0)
        {
            return Data.LifeTime;
        }
        float sec = Time.realtimeSinceStartup - mStartTimer;
        return sec >= Data.LifeTime ? 0 : Data.LifeTime - sec;
    }

    public void Release()
    {
        float size = Data.ChangeModelScale;
        if(size>GTDefine.V_INTERVAL_VALUE)
        {
            mOwner.CacheTransform.DOScale(size, 1);
        }
        if(mParticle!=null)
        {
            mParticle.Release();
        }
    }

    public bool IsDotOrHot()
    {
        return Data.Result == EActType.TYPE_LDDATTR ||
               Data.Result == EActType.TYPE_LUBATTR;
    }

    public void TriggerIntervalEvent()
    {
        if(mBuffAttr!=null)
        {
            return;
        }

        for(int i=0;i<mBuffAttr.Attrs.Count;i++)
        {
            CfgValueType data = mBuffAttr.Attrs[i];
            TriggerIntervalEventByBuffAttr(data);
        }
    }

    public void TriggerIntervalEventByBuffAttr(CfgValueType data)
    {
        int current = mOwner.GetCurrAttr().GetAttr(data.Attr);
        int changeValue = 0;

        switch (data.ValueType)
        {
            case EValueType.FIX:
                {
                    changeValue = data.Value;
                }
                break;
            case EValueType.PER:
                {
                    changeValue = Mathf.FloorToInt((data.Value / 10000f + 1) * current);
                }
                break;
            case EValueType.COM:
                {
                    if (data.Attr == EAttr.HP)
                    {
                        int maxHP = mOwner.GetCurrAttr().GetAttr(EAttr.MaxHP);
                        changeValue = Mathf.FloorToInt((data.Value / 10000f) * maxHP);
                    }
                    else
                    {
                        int maxMP = mOwner.GetCurrAttr().GetAttr(EAttr.MaxMP);
                        changeValue = Mathf.FloorToInt((data.Value / 10000f) * maxMP);
                    }
                }
                break;
        }


        switch(Data.Result)
        {
            case EActType.TYPE_LDDATTR:
                {
                    if (data.Attr == EAttr.MP)
                    {
                        mOwner.AddMP(changeValue,true);
                    }
                    else if (data.Attr == EAttr.HP)
                    {
                        mOwner.AddHP(changeValue,true);
                    }
                }
                break;
            case EActType.TYPE_LUBATTR:
                {
                    if (data.Attr == EAttr.MP)
                    {
                        mOwner.UseMP(changeValue);
                    }
                    else if (data.Attr == EAttr.HP)
                    {
                        mOwner.BeDamage(mCaster, changeValue);
                    }
                }
                break;
        }
    }

    public void SetParticleEnabled(bool enabled)
    {
        if(mParticle==null|| mParticle.CacheTransform == null)
        {
            return;
        }
        mParticle.Obj.SetActive(enabled);
    }
}
