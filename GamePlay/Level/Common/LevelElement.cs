using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace LevelDirector
{
    public class LevelElement : MonoBehaviour, ILevelBehaviour, IObj
    {
        public int     Id      { get; set; }
        public float   LifeTime = -1;

        public Vector3 Pos
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector3 Scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        public Vector3 Euler
        {
            get { return transform.eulerAngles; }
            set { transform.eulerAngles = value; }
        }

        public virtual void Init() { }

        public virtual void SetName() { }

        public virtual void Build() { }

        public virtual void Import(CfgBase pData, bool build) { }

        public virtual void Destroy() { }

        public virtual CfgBase Export() { return null; }

        public static void GetAllComponents<T>(Transform trans, List<T> pList) where T : Component
        {
            if (trans == null) return;
            for (int i = 0; i < trans.childCount; i++)
            {
                T t = trans.GetChild(i).GetComponent<T>();
                if (t != null)
                {
                    pList.Add(t);
                }
            }
        }
    }
}
