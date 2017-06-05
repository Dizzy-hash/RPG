using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace Cfg.Level
{
    public class MapConfig : DHelper
    {
        public int                   Id;
        public float                 Delay;
        public string                MapName         = string.Empty;
        public string                MapPath         = string.Empty;
        public bool                  AllowRide       = true;
        public bool                  AllowPK         = true;
        public bool                  AllowTrade      = true;
        public bool                  AllowFight      = true;
        public List<MapBorn>         Borns           = new List<MapBorn>();
        public List<MapBarrier>      Barriers        = new List<MapBarrier>();
        public List<MapPortal>       Portals         = new List<MapPortal>();
        public List<MapRegion>       Regions         = new List<MapRegion>();
        public List<MapMonsterSet>   MonsterSets     = new List<MapMonsterSet>();
        public List<MapWaveSet>      WaveSets        = new List<MapWaveSet>();
        public List<MapNpc>          Npcs            = new List<MapNpc>();
        public List<MapMineSet>      MineSets        = new List<MapMineSet>();
        public List<MapObj>          Objs            = new List<MapObj>();

        public override void Read(XmlElement os)
        {
            this.Id             = os.GetInt("Id");
            this.Delay          = os.GetFloat("Delay");
            this.MapName        = os.GetString("MapName");
            this.MapPath        = os.GetString("MapPath");
            this.AllowRide      = os.GetBool("AllowRide");
            this.AllowPK        = os.GetBool("AllowPK");
            this.AllowTrade     = os.GetBool("AllowTrade");
            this.AllowFight     = os.GetBool("AllowFight");
            foreach(var current in GetChilds(os))
            {
                switch(current.Name)
                {
                    case "Borns":
                        this.Borns         = ReadList<MapBorn>(current);
                        this.A             = this.Borns.Find((item) => { return item.Camp == EBattleCamp.A; });
                        this.B             = this.Borns.Find((item) => { return item.Camp == EBattleCamp.B; });
                        this.C             = this.Borns.Find((item) => { return item.Camp == EBattleCamp.C; });
                        break;
                    case "Barriers":
                        this.Barriers      = ReadList<MapBarrier>(current);
                        break;
                    case "Portals":
                        this.Portals       = ReadList<MapPortal>(current);
                        break;
                    case "Regions":
                        this.Regions       = ReadList<MapRegion>(current);
                        break;
                    case "MonsterSets":
                        this.MonsterSets   = ReadList<MapMonsterSet>(current);
                        break;
                    case "WaveSets":
                        this.WaveSets      = ReadList<MapWaveSet>(current);
                        break;
                    case "Npcs":
                        this.Npcs          = ReadList<MapNpc>(current);
                        break;
                    case "MineSets":
                        this.MineSets      = ReadList<MapMineSet>(current);
                        break;
                    case "Objs":
                        this.Objs         = ReadList<MapObj>(current);
                        break;
                }
            }
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",            Id);
            DHelper.Write(doc, os, "Delay",         Delay);
            DHelper.Write(doc, os, "MapName",       MapName);
            DHelper.Write(doc, os, "MapPath",       MapPath);
            DHelper.Write(doc, os, "AllowRide",     AllowRide);
            DHelper.Write(doc, os, "AllowPK",       AllowPK);
            DHelper.Write(doc, os, "AllowTrade",    AllowTrade);
            DHelper.Write(doc, os, "AllowFight",    AllowFight);
            DHelper.Write(doc, os, "Borns",         Borns);
            DHelper.Write(doc, os, "Barriers",      Barriers);
            DHelper.Write(doc, os, "Portals",       Portals);
            DHelper.Write(doc, os, "Regions",       Regions);
            DHelper.Write(doc, os, "MonsterSets",   MonsterSets);
            DHelper.Write(doc, os, "MineSets",      MineSets);
            DHelper.Write(doc, os, "WaveSets",      WaveSets);
            DHelper.Write(doc, os, "Npcs",          Npcs);
            DHelper.Write(doc, os, "Objs",          Objs);
        }

        public MapBorn A { get; private set; }
        public MapBorn B { get; private set; }
        public MapBorn C { get; private set; }
    }
}

