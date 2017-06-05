using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    [Serializable]
    public class TaskConvoyNpc : DHelper
    {
        [SerializeField]
        public int NpcID;

        public override void Read(XmlElement os)
        {
            this.NpcID = os.GetInt("NpcID");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "NpcID", NpcID);
        }
    }

}
