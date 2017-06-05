using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActFrozen : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandFrost>().Update(Duration).Do();
            return true;
        }
    }
}

