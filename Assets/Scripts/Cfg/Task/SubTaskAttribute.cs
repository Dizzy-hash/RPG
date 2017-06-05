using UnityEngine;
using System.Collections;
using System;

namespace Cfg.Task
{
    public class SubTaskAttribute : Attribute
    {
        public string            Label = string.Empty;
        public ETaskSubFuncType  Func = ETaskSubFuncType.TYPE_TALK;
    }
}

