using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Level
{
    public class MapObj : MapElement
    {
        public EMapObjType       Type    = EMapObjType.TYPE_BUILD;
        public string            Name    = string.Empty;
        public string            Path    = string.Empty;
        public Vector3           Pos     = Vector3.zero;
        public Vector3           Euler   = Vector3.zero;
        public Vector3           Scale   = Vector3.one;

        public override void Read(XmlElement os)
        {
            this.Id     = os.GetInt("Id");
            this.Type   = (EMapObjType)os.GetInt("Type");
            this.Name   = os.GetString("Name");
            this.Path   = os.GetString("Path");
            this.Pos    = os.GetVector3("Name");
            this.Euler  = os.GetVector3("Path");
            this.Scale  = os.GetVector3("Scale");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",     this.Id);
            DHelper.Write(doc, os, "Type",   (int)this.Type);
            DHelper.Write(doc, os, "Name",   this.Name);
            DHelper.Write(doc, os, "Path",   this.Path);
            DHelper.Write(doc, os, "Pos",    this.Pos);
            DHelper.Write(doc, os, "Euler",  this.Euler);
            DHelper.Write(doc, os, "Scale",  this.Scale);
        }
    }
}

