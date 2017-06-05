using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActSleep : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandSleep>().Update(Duration).Do();
            return true;
        }
    }
}

