using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActFear : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandFear>().Update(Duration).Do();
            return true;
        }
    }
}
