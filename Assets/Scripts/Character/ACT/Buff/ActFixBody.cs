using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActFixBody : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandFixBody>().Update(Duration).Do();
            return true;
        }
    }
}

