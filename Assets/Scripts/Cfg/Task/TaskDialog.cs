using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    [Serializable]
    public class TaskDialog : DHelper
    {
        public int                NpcID;
        public ETaskDialogRole    Role         = ETaskDialogRole.TYPE_PLAYER;
        public ETaskDialogPos     Pos          = ETaskDialogPos.TYPE_LF;
        public ETaskDialogAction  Action       = ETaskDialogAction.TYPE_NEXT;
        public ETaskDialogContent ContentShow  = ETaskDialogContent.TYPE_NORMAL;
        public string             Content      = string.Empty;
        public string             NpcAnim      = string.Empty;
        public int                VoiceID;
        public float              Delay;

    
        public override void Read(XmlElement os)
        {
            this.NpcID          = os.GetInt("NpcID");
            this.Role           = (ETaskDialogRole)os.GetInt("Role");
            this.Pos            = (ETaskDialogPos)os.GetInt("Pos");
            this.Action         = (ETaskDialogAction)os.GetInt("Action");
            this.ContentShow    = (ETaskDialogContent)os.GetInt("ContentShow");
            this.Content        = os.GetString("Content");
            this.NpcAnim        = os.GetString("NpcAnim");
            this.VoiceID        = os.GetInt("VoiceID");
            this.Delay          = os.GetFloat("Delay");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Role",        (int)Role);
            DHelper.Write(doc, os, "NpcID",       this.NpcID);
            DHelper.Write(doc, os, "Pos",         (int)Pos);
            DHelper.Write(doc, os, "Action",      (int)Action);
            DHelper.Write(doc, os, "ContentShow", (int)ContentShow);
            DHelper.Write(doc, os, "Content",     this.Content);
            DHelper.Write(doc, os, "VoiceID",     this.VoiceID);
            DHelper.Write(doc, os, "Delay",       this.Delay);
            DHelper.Write(doc, os, "NpcAnim",     this.NpcAnim);
        }
    }
}

