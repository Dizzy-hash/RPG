using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 沉默
/// </summary>
public class CommandSilent : CommandAct
{
    public float LastTime;

    public override Resp Do()
    {
        CmdHandler<CommandSilent> call = Del as CmdHandler<CommandSilent>;
        return call == null ? Resp.TYPE_NO : call(this);
    }

    public CommandSilent Update(float lastTime)
    {
        this.LastTime = lastTime;
        return this;
    }

    public CommandSilent Update(float lastTime, Callback exit, Callback stop)
    {
        this.ExitCallback = exit;
        this.StopCallback = stop;
        this.LastTime = lastTime;
        return this;
    }
}
