using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ACT
{
    public class ActSystem : IGameLoop
    {
        private List<ActItem> mRunItems = new List<ActItem>();
        private List<ActItem> mDelItems = new List<ActItem>();

        public void Run(ActItem item)
        {
            lock (mRunItems)
            {
                mRunItems.Add(item);
            }
        }

        public void Execute()
        {
            for (int i = 0; i < mRunItems.Count; i++)
            {
                ActItem item = mRunItems[i];
                item.Loop();
                if (item.Status == EActStatus.INITIAL || item.Status == EActStatus.SUCCESS)
                {
                    mDelItems.Add(item);
                }
            }
            for(int i=0;i<mDelItems.Count;i++)
            {
                ActItem item = mDelItems[i];
                mRunItems.Remove(item);
            }
            mDelItems.Clear();
        }

        public void Release()
        {
            mDelItems.Clear();
            mRunItems.Clear();
        }
    }
}

