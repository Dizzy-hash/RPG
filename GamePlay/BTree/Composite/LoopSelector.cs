using UnityEngine;
using System.Collections;
using System.Xml;

namespace BT
{
    public class LoopSelector : Selector
    {
        public int Times = 1;
        public int Count = 0;

        public override EBTStatus Step()
        {
            EBTStatus pStatus = base.Step();
            switch (pStatus)
            {
                case EBTStatus.BT_SUCCESS:
                    {
                        Count++;
                        base.Clear();
                        break;
                    }

                case EBTStatus.BT_RUNNING:
                    {
                        break;
                    }
                case EBTStatus.BT_FAILURE:
                    {
                        return EBTStatus.BT_FAILURE;
                    }

            }
            return Count >= Times ? EBTStatus.BT_SUCCESS : EBTStatus.BT_RUNNING;
        }

        public override void Clear()
        {
            base.Clear();
            Count = 0;
        }

        protected override void ReadAttribute(string key, string value)
        {
            switch (key)
            {
                case "Times":
                    this.Times = value.ToInt32();
                    break;
            }
        }

        protected override void SaveAttribute(XmlDocument doc, XmlElement xe)
        {
            xe.SetAttribute("Times", Times.ToString());
        }


        public override BTNode DeepClone()
        {
            LoopSelector ls = new LoopSelector();
            ls.Times = this.Times;
            ls.CloneChildren(this);
            return ls;
        }
    }
}

