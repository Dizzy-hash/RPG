using UnityEngine;
using System.Collections;

namespace ACT
{
    public class MoverParabol : Mover
    {
        public MoverParabol(Transform center, float moveSpeed, int speedCurve,  Character target) :
            base(center, moveSpeed, speedCurve, target)
        {

        }

        public override void Update()
        {
            base.Update();
            Vector3 pos = mTarget.Pos + new Vector3(0, mTarget.Height / 2, 0);
            Quaternion to = Quaternion.LookRotation(pos- mCenter.position, Vector3.up);
            mCenter.rotation = Quaternion.Slerp(mCenter.rotation, to, Time.deltaTime * 4);
            mCenter.Translate(mCenter.forward * Time.deltaTime * this.mMoveSpeed);
            this.mMoveSpeed += 2 * Time.deltaTime;
        }
    }
}

