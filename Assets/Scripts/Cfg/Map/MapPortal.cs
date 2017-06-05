using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Level
{
    public class MapPortal: MapElement
    {
        public string     Name = string.Empty;
        public int        RegionID;
        public int        DestMapID;
        public Vector3    DestPos;
        public bool       DisplayText;
        public Vector3    Pos;
        public Vector3    Euler;
        public ECR        CR = ECR.AND;
        public int        OpenLevel;
        public int        OpenItemID;
        public int        OpenVIP;

        public override void Read(XmlElement os)
        {
            this.Id           = os.GetInt("Id");
            this.Name         = os.GetString("Name");
            this.RegionID     = os.GetInt("RegionID");
            this.DestMapID    = os.GetInt("DestMapID");
            this.DestPos      = os.GetVector3("RegionID");
            this.DisplayText  = os.GetBool("DisplayText");
            this.Pos          = os.GetVector3("Pos");
            this.Euler        = os.GetVector3("Euler");
            this.CR           = (ECR)os.GetInt("CR");
            this.OpenLevel    = os.GetInt("OpenLevel");
            this.OpenItemID   = os.GetInt("OpenItemID");
            this.OpenVIP      = os.GetInt("OpenVIP");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc,os,  "Id",          this.Id);
            DHelper.Write(doc, os, "Name",        this.Name);
            DHelper.Write(doc, os, "RegionID",    this.RegionID);
            DHelper.Write(doc, os, "DestMapID",   this.DestMapID);
            DHelper.Write(doc, os, "DestPos",     this.DestPos);
            DHelper.Write(doc, os, "DisplayText", this.DisplayText);
            DHelper.Write(doc, os, "Pos",         this.Pos);
            DHelper.Write(doc, os, "Euler",       this.Euler);
            DHelper.Write(doc, os, "CR",     (int)this.CR);
            DHelper.Write(doc, os, "OpenLevel",   this.OpenLevel);
            DHelper.Write(doc, os, "OpenItemID",  this.OpenItemID);
            DHelper.Write(doc, os, "OpenVIP",     this.OpenVIP);
        }
    }
}

