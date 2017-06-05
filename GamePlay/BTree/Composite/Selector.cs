using UnityEngine;
using System.Collections;

namespace BT
{

    public class Selector : Composite
    {
        public override EBTStatus Step()
        {
            for (int i = 0; i < mChildren.Count; i++)
            {
                BTNode pNode = mChildren[i];
                EBTStatus pStatus = pNode.Step();
                switch (pStatus)
                {
                    case EBTStatus.BT_RUNNING:
                        {
                            if (mActiveIndex != i && mActiveIndex != -1)
                            {
                                mChildren[mActiveIndex].Clear();
                            }
                            mActiveIndex = i;
                            mPrevioIndex = -1;
                            mIsRunning = true;
                            return EBTStatus.BT_RUNNING;
                        }
                    case EBTStatus.BT_SUCCESS:
                        {
                            if (mActiveIndex != i && mActiveIndex != -1)
                            {
                                mChildren[mActiveIndex].Clear();
                            }
                            pNode.Clear();
                            mActiveIndex = -1;
                            mPrevioIndex = i;
                            mIsRunning = false;
                            return EBTStatus.BT_SUCCESS;
                        }
                    case EBTStatus.BT_FAILURE:
                        {
                            pNode.Clear();
                            return EBTStatus.BT_FAILURE;
                        }
                }
            }

            mIsRunning = false;
            return EBTStatus.BT_FAILURE;
        }

        public override BTNode DeepClone()
        {
            Selector select = new Selector();
            select.CloneChildren(this);
            return select;
        }

    }
}