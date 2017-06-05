using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Task
{
    [Serializable]
    public class SubConvoy : SubTaskBase
    {
        public TaskConvoyNpc       ConvoyNpc   = new TaskConvoyNpc();
        public TaskLocation        SrcLocation = new TaskLocation();
        public TaskLocation        TarLocation = new TaskLocation();

        public SubConvoy()
        {
            Func = ETaskSubFuncType.TYPE_CONVOY;
        }

        public override void Read(XmlElement os)
        {
            base.Read(os);
            foreach (var current in GetChilds(os))
            {
                switch (current.Name)
                {
                    case "ConvoyNpc":
                        this.ConvoyNpc   = ReadObj<TaskConvoyNpc>(current);
                        break;
                    case "SrcLocation":
                        this.SrcLocation = ReadObj<TaskLocation>(current);
                        break;
                    case "TarLocation":
                        this.TarLocation = ReadObj<TaskLocation>(current);
                        break;
                }
            }
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            base.Write(doc, os);
            DHelper.Write(doc, os, "ConvoyNpc",   ConvoyNpc);
            DHelper.Write(doc, os, "SrcLocation", SrcLocation);
            DHelper.Write(doc, os, "TarLocation", TarLocation);
        }
    }
}

