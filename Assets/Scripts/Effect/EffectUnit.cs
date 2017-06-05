using UnityEngine;
using System.Collections;
using System;
using Protocol;
using System.Collections.Generic;

public class EffectUnit : IObj
{
    public int                                 ID             { get; set; }
    public int                                 GUID           { get; set; }
    public Transform                           CacheTransform { get; set; }
    public DEffect                             Cfg            { get; private set; }
    public EffectView                          View           { get; private set; }
    public Transform                           Parent         { get; private set; }
    public bool                                Retain         { get; private set; }
    public Callback<Collider>                  OnTriggerEnter { get; set; }

    public Vector3                             Euler
    {
        get { return CacheTransform.localEulerAngles; }
        set { CacheTransform.localEulerAngles = value; }
    }

    public Vector3                             Pos
    {
        get { return CacheTransform.position; }
        set { CacheTransform.position = value; }
    }

    public Vector3                             Scale
    {
        get { return CacheTransform.localScale; }
        set { CacheTransform.localScale = value; }
    }

    public EffectUnit(int id, int guid, KTransform data, Transform parent, bool retain)
    {
        this.ID = id;
        this.GUID = guid > 0 ? guid : GTGUID.NewGUID();
        this.Parent = parent;
        this.Retain = retain;
        this.Cfg = ReadCfgEffect.GetDataById(id);
        GameObject go = GTPoolManager.Instance.GetObject(Cfg.Path);
        if (go == null)
        {
            return;
        }
        this.CacheTransform = go.transform;
        NGUITools.SetLayer(go, GTLayer.LAYER_EFFECT);
        this.CacheTransform.parent = this.Parent != null ? Parent : null;
        this.CacheTransform.localScale = data.Scale;
        this.CacheTransform.localEulerAngles = data.Euler;
        this.CacheTransform.localPosition = data.Pos;
        this.View = this.CacheTransform.gameObject.GET<EffectView>();
        this.View.Id = ID;
        this.View.GUID = GUID;
        this.View.Unit = this;
    }

    public void Release()
    {
        GTWorld.Instance.Ect.Effects.Remove(this.GUID);
        Destroy();
    }

    public void Release(float time = 0)
    {
        GTTimerManager.Instance.AddListener(time, Release);
    }

    public void Destroy()
    {
        if (this.CacheTransform != null)
        {
            GTPoolManager.Instance.ReleaseGo(Cfg.Path, this.CacheTransform.gameObject, Retain);
        }
    }
}
