using UnityEngine;
using System.Collections;


/// <summary>
/// 蹦
/// </summary>
public class CommandJump : ICommand
{
    public override Resp Do()
    {
        CmdHandler<CommandJump> call = Del as CmdHandler<CommandJump>;
        return call == null ? Resp.TYPE_NO : call(this);
    }
}