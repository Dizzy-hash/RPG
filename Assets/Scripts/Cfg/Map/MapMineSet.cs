using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Level
{
    public class MapMineSet : MapElement
    {
        public int           RegionID;
        public List<MapMine> Mines = new List<MapMine>();

        public override void Read(XmlElement os)
        {
            this.Id            = os.GetInt("Id");
            this.RegionID      = os.GetInt("RegionID");
            this.Mines         = ReadList<MapMine>(os);
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",       Id);
            DHelper.Write(doc, os, "RegionID", Id);
            DHelper.Write(doc, os, "Mines",    Mines);
        }
    }

}
