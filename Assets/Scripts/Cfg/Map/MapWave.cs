using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace Cfg.Level
{
    public class MapWave : MapElement
    {
        public int                       Index;
        public string                    IndexName = string.Empty;
        public float                     Delay;
        public EMonsterWaveSpawn         Spawn;
        public int                       AddBuffID;
        public List<MapMonster>      Monsters = new List<MapMonster>();


        public override void Read(XmlElement os)
        {
            this.Id              = os.GetInt("Id");
            this.Index           = os.GetInt("Index");
            this.IndexName       = os.GetString("IndexName");
            this.Delay           = os.GetFloat("Delay");
            this.Spawn           = (EMonsterWaveSpawn)os.GetInt("Spawn");
            this.AddBuffID       = os.GetInt("AddBuffID");
            this.Monsters        = ReadList<MapMonster>(os);
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",           Id);
            DHelper.Write(doc, os, "Index",        Index);
            DHelper.Write(doc, os, "IndexName",    IndexName);
            DHelper.Write(doc, os, "Delay",        Delay);
            DHelper.Write(doc, os, "Spawn",   (int)Spawn);
            DHelper.Write(doc, os, "AddBuffID",    AddBuffID);
            DHelper.Write(doc, os, "Monsters",     Monsters);
        }
    }
}

