using UnityEngine;
using System.Collections;
using BVT.Core;
using BVT.Core;

namespace BVT.Behaviour
{
    public class BTTask : Node
    {
        public override int  MaxChildCount { get { return 0; } }

        public override bool CanAsFirst    { get { return false; } }
    }
}
