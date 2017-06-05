using UnityEngine;
using System.Collections;

namespace ACT
{
    public enum EMoveType
    {
        TYPE_LINEAR      = 0,
        TYPE_PROJECTILE  = 1,
        TYPE_PURSUE      = 2,
        TYPE_PARABOL     = 3,
    }

    public class MoverHelper
    {
        public static Mover MakeMove(EMoveType type, Transform moveTrans, float moveSpeed, int speedCurve, Character target)
        {
            switch (type)
            {
                case EMoveType.TYPE_LINEAR:
                    return new MoverLinear(moveTrans, moveSpeed, speedCurve, target);
                case EMoveType.TYPE_PARABOL:
                    return new MoverParabol(moveTrans, moveSpeed, speedCurve, target);
                case EMoveType.TYPE_PURSUE:
                    return new MoverPursue(moveTrans, moveSpeed, speedCurve, target);
                default:
                    return null;
            }
        }
    }
}

