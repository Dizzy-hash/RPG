using UnityEngine;
using System.Collections;
using BVT.Core;
using System.Collections.Generic;

namespace BVT.Core
{
    public interface INode
    {
        void OnCreate();
        bool OnEnter();
        void OnTick();
        void OnReset();
        void OnExit(ENST state);
    }
}

