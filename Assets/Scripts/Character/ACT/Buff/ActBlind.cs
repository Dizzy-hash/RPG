using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActBlind : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandBlind>().Update(Duration).Do();
            return true;
        }
    }
}

