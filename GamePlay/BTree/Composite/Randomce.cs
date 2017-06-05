using UnityEngine;
using System.Collections;

namespace BT
{
    public class Randomce : Composite
    {
        public override EBTStatus Step()
        {
            if (mActiveIndex == -1)
            {
                mActiveIndex = UnityEngine.Random.Range(0, mChildren.Count);
            }

            if (mChildren.Count > 0)
            {
                BTNode pNode = mChildren[mActiveIndex];
                EBTStatus pStatus = pNode.Step();
                switch (pStatus)
                {
                    case EBTStatus.BT_RUNNING:
                        {
                            mIsRunning = true;
                            return EBTStatus.BT_RUNNING;
                        }
                    case EBTStatus.BT_SUCCESS:
                        {
                            pNode.Clear();
                            mActiveIndex = -1;
                            mPrevioIndex = mActiveIndex;
                            mIsRunning = false;
                            return EBTStatus.BT_SUCCESS;
                        }
                    case EBTStatus.BT_FAILURE:
                        {
                            pNode.Clear();
                            mActiveIndex = -1;
                            mPrevioIndex = -1;
                            mIsRunning = false;
                            return EBTStatus.BT_FAILURE;
                        }
                }
            }
            mActiveIndex = -1;
            mPrevioIndex = -1;
            mIsRunning = false;
            return EBTStatus.BT_FAILURE;
        }

        public override BTNode DeepClone()
        {
            Randomce rc = new Randomce();
            rc.CloneChildren(this);
            return rc;
        }
    }
}
