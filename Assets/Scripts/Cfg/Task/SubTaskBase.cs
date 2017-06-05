using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    public class SubTaskBase : DHelper
    {
        public string           Desc = string.Empty;
        public ETaskSubFuncType Func = ETaskSubFuncType.TYPE_TALK;

        public override void Read(XmlElement os)
        {
            this.Func = (ETaskSubFuncType)os.GetInt("Func");
            this.Desc = os.GetString("Desc");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Func", (int)Func);
            DHelper.Write(doc, os, "Desc",      Desc);
        }
    }
}