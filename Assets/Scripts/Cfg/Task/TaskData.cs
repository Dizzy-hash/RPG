using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Cfg.Task
{
    [Serializable]
    public class TaskData : DHelper
    {
        public int               Id;
        public string            Name                      = string.Empty;
        public ETaskType         Type                      = ETaskType.NONE;
        public bool              CanbeCancle               = false;
        public bool              CanbeSearch               = false;
        public bool              IsAutoPathFind            = true;
        public bool              IsFinishedTaskCount       = true;
        public bool              IsAutoFinish              = false;
        public int               PreTaskID;
        public List<SubTaskBase> SubTasks                  = new List<SubTaskBase>();

        public override void Read(XmlElement os)
        {
            this.Id                  = os.GetInt("Id");
            this.Name                = os.GetString("Name");
            this.Type                = (ETaskType)os.GetInt("Type");
            this.CanbeCancle         = os.GetBool("CanbeCancle");
            this.CanbeSearch         = os.GetBool("CanbeSearch");
            this.IsAutoPathFind      = os.GetBool("IsAutoPathFind");
            this.IsFinishedTaskCount = os.GetBool("IsFinishedTaskCount");
            this.IsAutoFinish        = os.GetBool("IsAutoFinish");
            this.PreTaskID           = os.GetInt("PreTaskID");
            foreach (var current in GetChilds(os))
            {
                switch (current.Name)
                {
                    case "SubTasks":
                        foreach(var child in GetChilds(current))
                        {
                            string type = child.GetString("Type");
                            switch(type)
                            {
                                case "SubTask":
                                    this.SubTasks.Add(ReadObj<SubTalk>(child));
                                    break;
                                case "SubCollectItem":
                                    this.SubTasks.Add(ReadObj<SubCollectItem>(child));
                                    break;
                                case "SubConvoy":
                                    this.SubTasks.Add(ReadObj<SubConvoy>(child));
                                    break;
                                case "SubGather":
                                    this.SubTasks.Add(ReadObj<SubGather>(child));
                                    break;
                                case "SubInterActive":
                                    this.SubTasks.Add(ReadObj<SubInterActive>(child));
                                    break;
                                case "SubKillMonster":
                                    this.SubTasks.Add(ReadObj<SubKillMonster>(child));
                                    break;
                                case "SubTriggerCutscene":
                                    this.SubTasks.Add(ReadObj<SubTriggerCutscene>(child));
                                    break;
                                case "SubTriggerPlot":
                                    this.SubTasks.Add(ReadObj<SubTriggerPlot>(child));
                                    break;
                                case "SubUseItem":
                                    this.SubTasks.Add(ReadObj<SubUseItem>(child));
                                    break;
                                case "SubUseSkill":
                                    this.SubTasks.Add(ReadObj<SubUseSkill>(child));
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        public override void Write(XmlDocument doc, XmlElement os)
        {
            DHelper.Write(doc, os, "Id",                  Id);
            DHelper.Write(doc, os, "Name",                this.Name);
            DHelper.Write(doc, os, "TaskType",       (int)this.Type);
            DHelper.Write(doc, os, "CanbeCancle",         this.CanbeCancle);
            DHelper.Write(doc, os, "CanbeSearch",         this.CanbeSearch);
            DHelper.Write(doc, os, "IsAutoPathFind",      this.IsAutoPathFind);
            DHelper.Write(doc, os, "IsFinishedTaskCount", this.IsFinishedTaskCount);
            DHelper.Write(doc, os, "IsAutoFinish",        this.IsAutoFinish);
            DHelper.Write(doc, os, "PreTaskID",           this.PreTaskID);
            foreach(var current in this.SubTasks)
            {
                DHelper.Write(doc, os, "Item", current);
            }
        }
    }
}

