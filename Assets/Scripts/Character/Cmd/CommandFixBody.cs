using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 定身
/// </summary>
public class CommandFixBody : CommandAct
{
    public float LastTime;

    public override Resp Do()
    {
        CmdHandler<CommandFixBody> call = Del as CmdHandler<CommandFixBody>;
        return call == null ? Resp.TYPE_NO : call(this);
    }

    public CommandFixBody Update(float lastTime)
    {
        this.LastTime = lastTime;
        return this;
    }

    public CommandFixBody Update(float lastTime, Callback exit, Callback stop)
    {
        this.ExitCallback = exit;
        this.StopCallback = stop;
        this.LastTime = lastTime;
        return this;
    }
}
