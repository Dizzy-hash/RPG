using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActClone : ActItem
    {
        public ActClone()
        {
            EventType = EActEventType.Instant;
        }

        protected override bool Trigger()
        {
            this.RunClone();
            return true;
        }
    }
}