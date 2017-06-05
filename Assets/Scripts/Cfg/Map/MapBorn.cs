using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Level
{
    public class MapBorn : MapElement
    {
        public EBattleCamp    Camp;
        public Vector3        Pos    = Vector3.zero;
        public Vector3        Euler  = Vector3.zero;
        public Vector3        Scale  = Vector3.one;

        public override void Read(XmlElement os)
        {
            this.Camp       = os.GetEnum<EBattleCamp>("Camp");
            this.Pos        = os.GetVector3("Pos");
            this.Euler      = os.GetVector3("Euler");
            this.Scale      = os.GetVector3("Scale");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc,os,  "Camp", (int)this.Camp);
            DHelper.Write(doc, os, "Pos",            Pos);
            DHelper.Write(doc, os, "Euler",          Euler);
            DHelper.Write(doc, os, "Scale",          Scale);
        }
    }
}

