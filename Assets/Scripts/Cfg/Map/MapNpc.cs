using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Level
{
    public class MapNpc : MapElement
    {
        public Vector3                Pos;
        public Vector3                Euler;
        public Vector3                Scale = Vector3.one;
        public List<string>           Talks = new List<string>();

        public override void Read(XmlElement os)
        {
            this.Id     = os.GetInt("Id");
            this.Pos    = os.GetVector3("Pos");
            this.Euler  = os.GetVector3("Euler");
            this.Scale  = os.GetVector3("Scale");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",     this.Id);
            DHelper.Write(doc, os, "Pos",    this.Pos);
            DHelper.Write(doc, os, "Euler",  this.Euler);
            DHelper.Write(doc, os, "Scale",  this.Scale);
        }
    }
}

