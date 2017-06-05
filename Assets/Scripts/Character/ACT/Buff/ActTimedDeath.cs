using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActTimedDeath : ActBuffItem
    {
        public ActTimedDeath()
        {
            this.EventType = EActEventType.Subtain;
        }

        protected override bool Trigger()
        {
            base.Trigger();
            return true;
        }

        protected override void End()
        {
            base.End();
            this.Carryer.KillMe();
        }
    }
}

