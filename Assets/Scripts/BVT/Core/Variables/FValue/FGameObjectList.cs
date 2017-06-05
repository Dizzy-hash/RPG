using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BVT.Core
{
    public class FGameObjectList : FValueObj<List<GameObject>>
    {
#if UNITY_EDITOR
        public override void DrawField()
        {

        }
#endif
    }
}
