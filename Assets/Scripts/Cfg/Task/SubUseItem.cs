using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    [Serializable]
    public class SubUseItem : SubTaskBase
    {
        public int ID;
        public int Times;

        public SubUseItem()
        {
            Func = ETaskSubFuncType.TYPE_USEITEM;
        }

        public override void Read(XmlElement os)
        {
            base.Read(os);
            this.ID    = os.GetInt("ID");
            this.Times = os.GetInt("Times");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            base.Write(doc, os);
            DHelper.Write(doc, os, "ID",    ID);
            DHelper.Write(doc, os, "Times", Times);
        }
    }
}

