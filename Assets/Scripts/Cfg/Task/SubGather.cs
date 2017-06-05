using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

namespace Cfg.Task
{
    public class SubGather : SubTaskBase
    {
        public int          ID;
        public int          Count;
        public TaskLocation Location = new TaskLocation();

        public SubGather()
        {
            Func = ETaskSubFuncType.TYPE_GATHER;
        }

        public override void Read(XmlElement os)
        {
            base.Read(os);
            this.ID    = os.GetInt("ID");
            this.Count = os.GetInt("Count");
            foreach (var current in GetChilds(os))
            {
                switch (current.Name)
                {
                    case "Location":
                        this.Location = ReadObj<TaskLocation>(current);
                        break;
                }
            }
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            base.Write(doc, os);
            DHelper.Write(doc, os, "ID",       ID);
            DHelper.Write(doc, os, "Count",    Count);
            DHelper.Write(doc, os, "Location", Location);
        }
    }
}
