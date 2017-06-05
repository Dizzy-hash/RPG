using UnityEngine;
using System.Collections;

namespace ACT
{
    public class MoverLinear : Mover
    {
        public MoverLinear(Transform center, float moveSpeed, int speedCurve,Character target) :
            base(center, moveSpeed, speedCurve, target)
        {

        }

        public override void Update()
        {
            base.Update();
            this.mMoveDir = mCenter.forward;
            Vector3 dir = this.mMoveDir;
            dir.Normalize();
            mCenter.position += dir * Time.deltaTime * mMoveSpeed;
        }
    }
}

