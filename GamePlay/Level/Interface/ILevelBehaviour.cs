using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LevelDirector
{
    public interface ILevelBehaviour
    {      
        void    Init();
        void    Destroy();
        void    Import(CfgBase pdata, bool pBuild);
        CfgBase Export();
        void    Build();
        void    SetName();
    }
}

