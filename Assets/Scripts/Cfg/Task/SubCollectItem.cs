using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Task
{
    [Serializable]
    public class SubCollectItem : SubTaskBase
    {
        public List<TaskCollectionItem> Items = new List<TaskCollectionItem>();

        public SubCollectItem()
        {
            Func = ETaskSubFuncType.TYPE_COLLECT;
        }

        public override void Read(XmlElement os)
        {
            base.Read(os);
            foreach (var current in GetChilds(os))
            {
                switch (current.Name)
                {
                    case "Items":
                        this.Items = ReadList<TaskCollectionItem>(current);
                        break;
                }
            }
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            base.Write(doc, os);
            DHelper.Write(doc, os, "Items", Items);
        }
    }
}

