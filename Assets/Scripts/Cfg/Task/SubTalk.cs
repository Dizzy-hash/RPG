using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    [Serializable]
    public class SubTalk: SubTaskBase
    {
        public List<TaskDialog> Dialogs = new List<TaskDialog>();
        public TaskNpc          Npc     = new TaskNpc();

        public SubTalk()
        {
            Func = ETaskSubFuncType.TYPE_TALK;
        }

        public override void Read(XmlElement os)
        {
            base.Read(os);
            foreach (var current in GetChilds(os))
            {
                switch (current.Name)
                {
                    case "Npc":
                        this.Npc     = ReadObj<TaskNpc>(current);
                        break;
                    case "Dialogs":
                        this.Dialogs = ReadList<TaskDialog>(current);
                        break;
                }
            }
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            base.Write(doc, os);
            DHelper.Write(doc, os, "Npc",     Npc);
            DHelper.Write(doc, os, "Dialogs", Dialogs);
        }
    }
}

