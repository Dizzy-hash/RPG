using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Level
{
    public class MapEventCondition : DHelper
    {
        public ETriggerCondition Type = ETriggerCondition.TYPE_ENTER_REGION;
        public string            Args = string.Empty;
        
        public override void Read(XmlElement os)
        {
            this.Type = (ETriggerCondition)os.GetInt("Type");
            this.Args = os.GetString("Args");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Type", (int)Type);
            DHelper.Write(doc, os, "Args",      Args);
        }
    }
}
