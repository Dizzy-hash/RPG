using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActAddAttr : ActBuffItem
    {
        [SerializeField]
        public EAttr ID       = EAttr.MAXHP;
        [SerializeField]
        public float Percent  = 1;
    }
}
