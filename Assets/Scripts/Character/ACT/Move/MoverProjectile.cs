using UnityEngine;
using System.Collections;

namespace ACT
{
    public class MoveProjectile : Mover
    {
        public MoveProjectile(Transform center, float moveSpeed, int speedCurve, Character target) :
            base(center, moveSpeed, speedCurve, target)
        {

        }

        public override void Update()
        {
            base.Update();
        }
    }
}

