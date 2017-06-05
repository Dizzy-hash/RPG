using UnityEngine;
using System.Collections;
using System;
using Cfg.Level;

namespace LVL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LevelElementVariable : Attribute
    {
        public Type CfgType { get; set; }
    }
}
