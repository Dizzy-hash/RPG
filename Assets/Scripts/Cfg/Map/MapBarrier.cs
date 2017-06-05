using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System;

namespace Cfg.Level
{
    public class MapBarrier : MapElement
    {
        public float        Width = 7;
        public Vector3      Pos   = Vector3.zero;
        public Vector3      Euler = Vector3.zero;
        public Vector3      Scale = Vector3.one;

        public override void Read(XmlElement os)
        {
            this.Id      = os.GetInt("Id");
            this.Width   = os.GetFloat("Euler");
            this.Pos     = os.GetVector3("Pos");
            this.Euler   = os.GetVector3("Euler");
            this.Scale   = os.GetVector3("Scale");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",      this.Id);
            DHelper.Write(doc, os, "Width",   this.Width);
            DHelper.Write(doc, os, "Pos",     this.Pos);
            DHelper.Write(doc, os, "Euler",   this.Euler);
            DHelper.Write(doc, os, "Scale",   this.Scale);
        }
    }
}

