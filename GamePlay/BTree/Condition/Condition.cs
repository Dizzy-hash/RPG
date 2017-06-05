using UnityEngine;
using System.Collections;

namespace BT
{
    public class Condition : BTNode
    {
        public override EBTStatus Step()
        {
            return CheckCondition() ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
        }
    }
}
