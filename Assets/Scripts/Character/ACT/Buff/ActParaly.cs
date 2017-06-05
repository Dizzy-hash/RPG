using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActParaly : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandParaly>().Do();
            return true;
        }
    }
}

