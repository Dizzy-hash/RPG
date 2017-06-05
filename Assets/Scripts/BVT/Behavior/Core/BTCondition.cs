using UnityEngine;
using System.Collections;
using BVT.Core;

namespace BVT.Behaviour
{
    public class BTCondition : BTTask
    {
        [NodeVariable]
        public bool Invert = false;

        public sealed override bool OnEnter()
        {
            base.OnEnter();
            return Invert ? !Check() : Check();
        }

        public virtual         bool Check()
        {
            return true;
        }
    }
}
