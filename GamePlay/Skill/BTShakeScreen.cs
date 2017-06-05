using UnityEngine;
using System.Collections;

namespace BT
{
    public class ShakeScreen : Action
    {
        protected override bool Enter()
        {
            base.Enter();
            Camera cam = GTCameraManager.Instance.MainCamera;
            GTCameraManager.Instance.SwitchCameraEffect(ECameraType.SHAKE, cam, null);
            return true;
        }
    }
}