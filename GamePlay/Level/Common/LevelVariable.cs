using UnityEngine;
using System.Collections;
using System;

namespace LevelDirector
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
    public class LevelVariable : Attribute
    {
        public enum ModeType
        {
            None,
            Unity,
            Collection,
            Custom
        }

        public string Label            = string.Empty;
        public ModeType Mode           = ModeType.None;
        public Type MapVariableType    = null;
        public Type KeyType            = null;
        public Type ValueType          = null;
    }
}

