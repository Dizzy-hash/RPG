using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cfg.Map;

namespace LevelDirector
{
    public class LevelEvent : LevelElement
    {
        public EMapTrigger             Type               = EMapTrigger.TYPE_NONE;
        public bool                    Active             = true;
        public ECR                     Relation1          = ECR.AND;
        public bool                    UseIntervalTrigger = false;
        public ECR                     Relation2          = ECR.AND;
        [SerializeField]
        public List<MapEventCondition> Conditions1        = new List<MapEventCondition>();//首次触发条件
        [SerializeField]
        public List<MapEventCondition> Conditions2        = new List<MapEventCondition>();//间隔触发条件
        public int                     TriggerNum = 1;
        public float                   TriggerInterval = 0;
        public float                   TriggerDelay = 0;

        public override void SetName()
        {
            transform.name = "Event_" + Type.ToString() + "_" + Id;
        }

        public override void Import(CfgBase pData, bool pBuild)
        {
            MapEvent data = pData as MapEvent;
            Type                = data.Type;
            Id                  = data.Id;
            Relation1           = data.Relation1;
            Conditions1         = data.Conditions1;
            TriggerDelay        = data.TriggerDelay;
            Active              = data.Active;
            if (data.Conditions2 != null)
            {
                Relation2       = data.Relation2;
                Conditions2     = data.Conditions2;
                TriggerNum      = data.TriggerNum;
                TriggerInterval = data.TriggerInterval;
            }
            this.Build();
            this.SetName();
        }

        public override CfgBase Export()
        {
            MapEvent data = new MapEvent();
            data.Type                = Type;
            data.Id                  = Id;
            data.Relation1           = Relation1;
            data.Conditions1         = Conditions1;
            data.TriggerDelay        = TriggerDelay;
            data.Active              = Active;
            if (UseIntervalTrigger)
            {
                data.Relation2       = Relation2;
                data.Conditions2     = Conditions2;
                data.TriggerNum      = TriggerNum;
                data.TriggerInterval = TriggerInterval;
            }
            return data;
        }
    }
}

