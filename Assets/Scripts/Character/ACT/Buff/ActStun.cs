using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActStun : ActBuffItem
    {
        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandStun>().Update(Duration).Do();
            return true;
        }
    }
}

