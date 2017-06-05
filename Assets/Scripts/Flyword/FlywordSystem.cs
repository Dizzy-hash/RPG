using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class FlywordSystem : IGameLoop
{
    private Queue<FlywordData> mQueue    = new Queue<FlywordData>();
    private int                timer     = 2;
    private int                interval  = 2;
    public const string        FLYWORD   = "Guis/HUD/UIFlyword";

    public void Execute()
    {
        if (mQueue.Count == 0)
        {
            return;
        }
        if (timer > interval)
        {
            FlywordData item = mQueue.Dequeue();
            Play(item.value, item.pos, item.type);
            timer = 0;
        }
        else
        {
            timer++;
        }
    }

    public void Release()
    {
        mQueue.Clear();
    }

    public void Show(string value, Vector3 pos, EFlyWordType type)
    {
        FlywordData data = new FlywordData(value, pos, type);
        mQueue.Enqueue(data);
    }

    void Play(string value, Vector3 pos, EFlyWordType type)
    {
        string path = FLYWORD;
        GameObject go = GTPoolManager.Instance.GetObject(path);
        UIFlyText flyword = go.GET<UIFlyText>();
        flyword.gameObject.layer = GTLayer.LAYER_UI;
        flyword.Path = FLYWORD;
        Vector3 pos_3d = pos;
        Vector2 pos_screen = GTCameraManager.Instance.MainCamera.WorldToScreenPoint(pos_3d);
        Vector3 pos_ui = GTCameraManager.Instance.NGUICamera.ScreenToWorldPoint(pos_screen);
        pos_ui.z = 0;
        switch (type)
        {
            case EFlyWordType.TYPE_ENEMY_HURT:
                flyword.TextColor = Color.red;
                flyword.Text = "-" + value;
                break;
            case EFlyWordType.TYPE_ENEMY_CRIT:
                flyword.TextColor = Color.red;
                flyword.TextEnlarge = 1.2f;
                flyword.Text = "-" + value;
                break;
            case EFlyWordType.TYPE_AVATAR_CRIT:
                flyword.TextColor = Color.yellow;
                flyword.TextEnlarge = 1.2f;
                flyword.Text = "爆击 " + value;
                break;
            case EFlyWordType.TYPE_AVATAR_HURT:
                flyword.TextColor = Color.yellow;
                flyword.Text = value;
                break;
            case EFlyWordType.TYPE_AVATAR_HEAL:
                flyword.TextColor = Color.green;
                flyword.Text = "+" + value;
                break;
            case EFlyWordType.TYPE_AVATAR_BACKMAGIC:
                flyword.TextColor = new Color(0, 0.7f, 1);
                flyword.Text = "+" + value;
                break;
            case EFlyWordType.TYPE_PARTNER_HURT:
            case EFlyWordType.TYPE_PARTNER_CRIT:
                flyword.TextColor = Color.white;
                flyword.Text = value;
                break;
        }
        flyword.Init(pos_ui);
    }
}
