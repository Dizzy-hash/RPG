using UnityEngine;
using System.Collections;

/// <summary>
/// 被击飞
/// </summary>
public class CommandBeatFly : CommandAct
{
    public override Resp Do()
    {
        CmdHandler<CommandBeatFly> call = Del as CmdHandler<CommandBeatFly>;
        return call == null ? Resp.TYPE_NO : call(this);
    }
}

