using UnityEngine;
using System.Collections;

namespace ACT
{
    public class MoverPursue : Mover
    {
        public MoverPursue(Transform center, float moveSpeed, int speedCurve,  Character target) :
            base(center, moveSpeed, speedCurve,  target)
        {

        }

        public override void Update()
        {
            base.Update();
            if (mTarget == null)
            {
                this.mMoveDir = mCenter.forward;
                Vector3 dir = this.mMoveDir;
                dir.Normalize();
                mCenter.position += dir * Time.deltaTime * mMoveSpeed;
            }
            else
            {
                float x = mTarget.Pos.x;
                float y = mCenter.position.y;
                float z = mTarget.Pos.z;
                this.mMoveDir = new Vector3(x, y, z) - mCenter.position;
                Vector3 dir = this.mMoveDir;
                dir.Normalize();
                this.mCenter.rotation = Quaternion.LookRotation(dir);
                Vector3 move = dir * Time.deltaTime * mMoveSpeed;
                mCenter.position += move;
            }
        }
    }
}

