using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActVariation : ActBuffItem
    {
        [SerializeField]
        public int VariModelID;

        protected override bool Trigger()
        {
            base.Trigger();
            this.Carryer.Command.Get<CommandVariation>().Update(Duration, VariModelID).Do();
            return true;
        }


    }
}

