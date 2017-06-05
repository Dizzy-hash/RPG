using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ACT
{
    public struct Zone_Cylinder
    {
        public float MaxDis;    //最大有效距离
        public float HAngle;    //水平角度范围
        public float Height;    //圆柱有效高度
    }

    public struct Zone_Box
    {
        public float L;     //长
        public float W;     //宽
        public float H;     //高
    }

    public struct Zone_Triangle
    {

    }

    public class DctZone
    {
        private Transform     mCenter;
        private List<string>  mParams;
        private Character     mCaster;

        private int           mCurCount;
        private bool          mFinish;

        private Zone_Cylinder mZCylinder;
        private Zone_Box      mZBox;
        private Zone_Triangle mZTriangle;

        [SerializeField]
        public EAoeShape      Shape;
        [SerializeField] 
        public Vector3        Offset   = Vector3.zero;
        [SerializeField]
        public Vector3        Euler    = Vector3.zero;
        [SerializeField]
        public EAffect        Affect   = EAffect.Self;
        [SerializeField]
        public int            MaxCount = 1;


        public DctZone(Character caster, Transform center, List<string> attackParams)
        {
            this.mCaster = caster;
            this.mCenter = center;
            this.mParams = attackParams;
            this.mCurCount = 0;
            if (attackParams == null || attackParams.Count == 0)
            {
                this.Shape    = EAoeShape.TYPE_TARGET;
                this.Affect   = EAffect.Self;
                this.MaxCount = 1;
                this.Offset   = Vector3.zero;
                this.Euler    = Vector3.zero;
            }
            else
            {
                this.Shape    = (EAoeShape)Get(0).ToInt32();
                this.Affect   = (EAffect)Get(1).ToInt32();
                this.MaxCount = Get(2).ToInt32();
                this.Offset   = Get(3).ToVector3(true);
                this.Euler    = Get(4).ToVector3(true);
            }
            switch(Shape)
            {
                case EAoeShape.TYPE_BOX:
                    mZBox.L = Get(5).ToFloat();
                    mZBox.W = Get(6).ToFloat();
                    mZBox.H = Get(7).ToFloat();
                    break;
                case EAoeShape.TYPE_CYLINDER:
                    mZCylinder.MaxDis = Get(5).ToFloat();
                    mZCylinder.HAngle = Get(6).ToFloat();
                    mZCylinder.Height = Get(7).ToFloat();
                    break;
                case EAoeShape.TYPE_TRIANGLE:
                    break;
            }
        }

        public void Update(System.Func<Character, bool> callback)
        {
            if (mCurCount >= MaxCount && MaxCount > 0)
            {
                mFinish = true;
                return;
            }
            List<Character> affectList = mCaster.GetAffectCharacters(Affect);
            for (int i = 0; i < affectList.Count; i++)
            {
                Character cc = affectList[i];
                bool success = false;
                switch (Shape)
                {
                    case EAoeShape.TYPE_BOX:
                        if (IsTouchBox(cc))
                        {
                            success = callback(cc);
                        }
                        break;
                    case EAoeShape.TYPE_CYLINDER:
                        if (IsTouchCylinder(cc))
                        {
                            success = callback(cc);
                        }
                        break;
                    case EAoeShape.TYPE_TRIANGLE:
                        if (IsTouchTriangle(cc))
                        {
                            success = callback(cc);
                        }
                        break;
                }
                if (success)
                {
                    mCurCount++;
                }
            }
        }

        public void Reset()
        {
            this.mCurCount = 0;
            this.mFinish = false;
        }

        public bool IsTouchCylinder(Character cc)
        {
            if (cc == null || cc.CacheTransform == null)
            {
                return false;
            }
            if (mZCylinder.HAngle <= 0 || mZCylinder.Height <= 0)
            {
                return false;
            }


            float y = mCenter.position.y;
            float yMax = y + mZCylinder.Height / 2;
            float yMin = y - mZCylinder.Height / 2;
            if (cc.Pos.y + cc.Height <= yMin)
            {
                return false;
            }
            if (cc.Pos.y >= yMax)
            {
                return false;
            }

            Vector3 dirPos = Euler + mCenter.forward;
            dirPos.y = 0;
            float radius = mZCylinder.MaxDis + cc.Radius;
            if (GTTools.GetHorizontalDistance(mCenter.position, cc.Pos) > radius)
            {
                return false;
            }

            Vector3 targetPos = cc.Pos;
            targetPos.y = 0;
            Vector3 centerPos = mCenter.position;
            centerPos.y = 0;
            if (Vector3.Angle(targetPos - centerPos, dirPos) > mZCylinder.HAngle / 2)
            {
                return false;
            }
            return true;
        }

        public bool IsTouchBox(Character cc)
        {
            if (cc == null || cc.CacheTransform == null)
            {
                return false;
            }
            if (mZBox.H <= 0 || mZBox.W <= 0 || mZBox.H <= 0)
            {
                return false;
            }

            float y = mCenter.position.y;
            float yMax = y + mZBox.H;
            float yMin = y;

            if (cc.Pos.y + cc.Height <= yMin)
            {
                return false;
            }
            if (cc.Pos.y >= yMax)
            {
                return false;
            }
            Vector3 targetPos = cc.Pos;
            targetPos.y = 0;
            Vector3 centerPos = mCenter.position;
            centerPos.y = 0;

            Vector3 forward = mCenter.forward + Euler;
            float angle = Vector3.Angle(targetPos - centerPos, forward);
            if (angle > 90)
            {
                return false;
            }
            float w = mZBox.W / 2;
            float checkAngle = Mathf.Atan2(w, mZBox.L) * Mathf.Rad2Deg;
            float distance = Vector3.Distance(targetPos, centerPos);
            if (angle <= checkAngle)
            {
                if (distance > mZBox.L / Mathf.Cos(angle / Mathf.Rad2Deg))
                {
                    return false;
                }
            }
            else
            {
                if (distance > w / Mathf.Sin(angle / Mathf.Rad2Deg))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsTouchTriangle(Character cc)
        {
            return true;
        }

        public string Get(int index)
        {
            return mParams.Count > index ? mParams[index] : string.Empty;
        }
    }
}
