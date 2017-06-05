﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;

namespace Cfg.Task
{
    [Serializable]
    public class SubUseSkill : SubTaskBase
    {
        public ESkillPos Pos;
        public int       Times;

        public SubUseSkill()
        {
            Func = ETaskSubFuncType.TYPE_USESKILL;
        }

        public override void Read(XmlElement os)
        {
            base.Read(os);
            this.Pos   = (ESkillPos)os.GetInt("Pos");
            this.Times = os.GetInt("Times");
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            base.Write(doc, os);
            DHelper.Write(doc, os, "Pos",   (int)Pos);
            DHelper.Write(doc, os, "Times",      Times);
        }
    }
}

