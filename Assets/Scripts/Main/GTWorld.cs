using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LVL;
using ACT;

public class GTWorld : GTMonoSingleton<GTWorld>
{
    public EffectSystem  Ect     { get; private set; }
    public FlywordSystem Fly     { get; private set; }
    public HUDSystem     HUD     { get; private set; }
    public ActSystem     Act     { get; private set; }

    public override void SetDontDestroyOnLoad(Transform parent)
    {
        base.SetDontDestroyOnLoad(parent);
        CharacterManager.Instance.SetDontDestroyOnLoad(transform);
        Ect = new EffectSystem();
        Fly = new FlywordSystem();
        HUD = new HUDSystem();
        Act = new ActSystem();
    }

    public void EnterWorld(int mapID)
    {
        LevelManager.Instance.EnterWorld(mapID);
    }

    public void Release()
    {
        Act.Release();
        HUD.Release();
        Fly.Release();
        Ect.Release();
        LevelManager.  Instance.LeaveWorld();
    }

    void Update()
    {
        Act.Execute();
        HUD.Execute();
        Fly.Execute();
    }
}