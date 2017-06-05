using UnityEngine;
using System.Collections;
using System;

namespace LVL
{
    public class LevelElement : MonoBehaviour, ILevelElement
    {
        [LevelVariable]
        public int             ID        { get; set; }
        [LevelVariable]
        public int             GUID      { get; set; }

        [LevelVariable]
        public Vector3         Pos
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        [LevelVariable]
        public Vector3         Scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        [LevelVariable]
        public Vector3         Euler
        {
            get { return transform.eulerAngles; }
            set { transform.eulerAngles = value; }
        }
    }
}
