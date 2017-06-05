using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LevelDirector
{
    public class LevelContainerBase<T> : LevelElement, ILevelContainer where T : LevelElement
    {
        public List<T> Elements
        {
            get
            {
                mList.Clear();
                GetAllComponents<T>(transform, mList);
                return mList;
            }
        }

        public virtual T AddElement()
        {
            T pElem = new GameObject().AddComponent<T>();
            pElem.transform.parent = transform;
            pElem.Build();
            pElem.SetName();
            return pElem;
        }

        public T FindElement(int id)
        {
            for(int i=0;i<Elements.Count;i++)
            {
                if(Elements[i].Id==id)
                {
                    return Elements[i];
                }
            }
            return null;
        }

        private List<T> mList = new List<T>();
    }
}

