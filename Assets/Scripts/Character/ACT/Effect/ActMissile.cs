using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System.Collections.Generic;

namespace ACT
{
    public class ActMissile : ActTraceObj
    {
        public ActMissile()
        {
            EventType = EActEventType.Subtain;
        }


        protected sealed override void Execute()
        {
            base.Execute();
            if (Unit == null || Unit.CacheTransform == null)
            {
                return;
            }
            if (Move != null)
            {
                this.Move.Update();
            }
        }
    }
}