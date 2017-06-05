using UnityEngine;
using System.Collections;

namespace ACT
{
    public class ActScopeCylinder : ActScope
    {
        [SerializeField]
        public float MaxDis;    //最大有效距离
        [SerializeField]
        public int   HAngle;    //水平角度范围
        [SerializeField]
        public float Height;    //圆柱有效高度

        protected override bool IsTouch(Character cc, Vector3 hitPoint, Vector3 hitDir)
        {
            if (cc == null || cc.CacheTransform == null)
            {
                return false;
            }
            if (HAngle <= 0 || Height <= 0)
            {
                return false;
            }
            float y = hitPoint.y;
            float yMax = y + Height / 2;
            float yMin = y - Height / 2;
            if (cc.Pos.y + cc.Height <= yMin)
            {
                return false;
            }
            if (cc.Pos.y >= yMax)
            {
                return false;
            }
            Vector3 dirPos = Euler + hitDir;
            dirPos.y = 0;
            float radius = MaxDis + cc.Radius;
            if (GTTools.GetHorizontalDistance(hitPoint, cc.Pos) > radius)
            {
                return false;
            }
            Vector3 targetPos = cc.Pos;
            targetPos.y = 0;
            Vector3 centerPos = hitPoint;
            centerPos.y = 0;
            if (Vector3.Angle(targetPos - centerPos, dirPos) > HAngle / 2)
            {
                return false;
            }
            return true;
        }

        protected override void ShowWarning()
        {
            string path = string.Empty;
            switch (HAngle)
            {
                case 60:
                    path = "Effect/10/skilltips_60du";
                    break;
                case 120:
                    path = "Effect/10/skilltips_120du";
                    break;
                case 180:
                    path = "Effect/10/skilltips_180du";
                    break;
                case 360:
                    path = "Effect/10/skilltips_yuan";
                    break;
            }
            if(string.IsNullOrEmpty(path))
            {
                return;
            }
            GameObject w = GTPoolManager.Instance.GetObject(path);


            if(ApplyByCenter)
            {
                w.transform.parent = ApplyCenter;
                w.transform.localPosition = Vector3.zero;
                w.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                w.transform.forward = ApplyHitDir;
                w.transform.localPosition = ApplyHitPoint;
            }
            w.transform.localScale = new Vector3(MaxDis, Height, MaxDis);
            GTTimerManager.Instance.AddListener(1, () => { GTPoolManager.Instance.ReleaseGo(path, w); });
        }
    }
}

