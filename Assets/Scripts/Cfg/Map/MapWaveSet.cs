using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Level
{
    public class MapWaveSet : MapElement
    {
        public int                  AddBuffID;
        public List<MapWave>        Waves = new List<MapWave>();

        public override void Read(XmlElement os)
        {
            this.Id         = os.GetInt("Id");
            this.AddBuffID  = os.GetInt("AddBuffID");
            this.Waves      = ReadList<MapWave>(os);
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",        this.Id);
            DHelper.Write(doc, os, "AddBuffID", this.AddBuffID);
            DHelper.Write(doc, os, "Waves",     this.Waves);
        }
    }
}

