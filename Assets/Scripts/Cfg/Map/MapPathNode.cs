using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Level
{
    public class MapPathNode : DHelper
    {
        public float   Time;
        public Vector3 Pos;
        public Vector3 Euler;

        public override void Read(XmlElement os)
        {
            this.Time       = os.GetFloat("Time");
            this.Pos        = os.GetVector3("Pos");
            this.Euler      = os.GetVector3("Euler");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Time",  this.Time);
            DHelper.Write(doc, os, "Pos",   this.Pos);
            DHelper.Write(doc, os, "Euler", this.Euler);
        }
    }
}

