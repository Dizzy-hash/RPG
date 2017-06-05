using UnityEngine;
using System.Collections;
using System;

public class EffectData
{
    public int    Id;
    public Single Delay;
    public Single LastTime;
    public Single FlySpeed;

    public EFlyType Fly = EFlyType.STAY;
    public EFlyObjDeadType Dead = EFlyObjDeadType.UntilLifeTimeEnd;
    public EEffectBind Bind = EEffectBind.OwnBody;

    public Vector3 Offset;
    public Vector3 Euler;

    public Actor Target;//特效移动朝向
    public Actor Owner; //释放特效的人
    public Transform Parent;
    public bool SetParent = false;
    public string Sound = string.Empty;
}
