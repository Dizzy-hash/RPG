using UnityEngine;
using System.Collections;
using BVT.Core;

namespace BVT.Behaviour
{
    public class BTComposite : Node
    {
        public override int  MaxChildCount { get { return -1;   } }
        public override bool CanAsFirst    { get { return true; } }
    }
}

