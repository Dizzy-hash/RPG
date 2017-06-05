using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    [Serializable]
    public class TaskCollectionItem : DHelper
    {
        public int             ID;
        public int             Count;
        public int             NpcID;
        public float           DropRate = 1;
        public TaskLocation    Location = new TaskLocation();

        public override void Read(XmlElement os)
        {
            this.ID        = os.GetInt("ID");
            this.Count     = os.GetInt("Count");
            this.NpcID     = os.GetInt("NpcID");
            this.DropRate  = os.GetFloat("DropRate");
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
            DHelper.Write(doc, os, "ID",          ID);
            DHelper.Write(doc, os, "Count",       Count);
            DHelper.Write(doc, os, "NpcID",       NpcID);
            DHelper.Write(doc, os, "DropRate",    DropRate);
            DHelper.Write(doc, os, "Location",    Location);
        }
    }
}

