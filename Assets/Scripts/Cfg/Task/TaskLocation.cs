using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Cfg.Task
{
    [Serializable]
    public class TaskLocation : DHelper
    {
        public int     MapID;
        public Vector3 Pos;
        public Vector3 Euler;

        public override void Read(XmlElement os)
        {
            this.MapID    = os.GetInt("MapID");
            this.Pos      = os.GetVector3("Pos");
            this.Euler    = os.GetVector3("Euler");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "MapID", this.MapID);
            DHelper.Write(doc, os, "Pos",   this.Pos);
            DHelper.Write(doc, os, "Euler", this.Euler);
        }
    }
}
