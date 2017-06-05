using UnityEngine;
using System.Collections;

public class ActorTurnFSM : ActorBaseFSM
{
    private CommandTurn mTrueCommand;
    private float mTurnSpeed = 10;

    public override void Enter()
    {
        mTrueCommand = Cmd as CommandTurn;
    }

    public override void Execute()
    {
        Quaternion rot = Owner.CacheTransform.localRotation;
        Quaternion toTarget = Quaternion.LookRotation(mTrueCommand.LookDirection);

        if (Quaternion.Angle(rot, toTarget) < 5)
        {
            if (mTrueCommand.OnFinish != null)
            {
                mTrueCommand.OnFinish();
                mTrueCommand.OnFinish = null;
            }
            return;
        }
        float rtime = GTLauncher.TimeScale;
        if (rtime != 0)
        {
            rot = Quaternion.Slerp(rot, toTarget, mTurnSpeed / rtime * Time.deltaTime);
        }

        Vector3 euler = rot.eulerAngles;
        euler.z = 0;
        euler.x = 0;
        Owner.CacheTransform.localRotation = Quaternion.Euler(euler);
    }

    public override void Exit()
    {

    }
}
