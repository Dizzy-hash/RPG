using UnityEngine;
using System.Collections;
using BVT.Core;
using System.Collections.Generic;

namespace BVT.Core
{
    public class NodeTreeManager : BVTMonoSingleton<NodeTreeManager>
    {
        private List<System.Action> mUpdateCallbacks = new List<System.Action>();

        public void AddListener(System.Action callback)
        {
            mUpdateCallbacks.Add(callback);
        }

        public void DelListener(System.Action callback)
        {
            mUpdateCallbacks.Remove(callback);
        }

        void Update()
        {
            for (int i = 0; i < mUpdateCallbacks.Count; i++)
            {
                mUpdateCallbacks[i].Invoke();
            }
        }
    }
}

