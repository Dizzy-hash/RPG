using UnityEngine;
using System.Collections;
using System;

namespace ACT
{
    public class Mover
    {
        protected Transform      mCenter;
        protected Character      mTarget;
        protected float          mMoveSpeed;
        protected Vector3        mMoveDir;
        protected AnimationCurve mMoveSpeedCurve;
        protected float          mMoveStartTime;
        protected float          mMoveStartSpeed;

        public Mover(Transform center, float moveSpeed, int speedCurve, Character target)
        {
            this.mCenter = center;
            this.mTarget = target;
            this.mMoveSpeed = moveSpeed;
            this.mMoveStartSpeed = moveSpeed;
            this.mMoveSpeedCurve = GetCurve(speedCurve);
            this.mMoveStartTime = Time.time;
        }

        public virtual void Update()
        {
            if (mMoveSpeedCurve != null)
            {
                float time = Time.time - mMoveStartTime;
                mMoveSpeed = mMoveSpeedCurve.Evaluate(time) * this.mMoveStartSpeed;
            }
        }

        public AnimationCurve GetCurve(int id)
        {
            DCurve db = null;
            if (id > 0)
            {
                db = ReadCfgCurve.GetDataById(id);
            }
            if (db != null)
            {
                return ECurve.Get(db.Path);
            }
            return null;
        }
    }
}

