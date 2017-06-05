using UnityEngine;
using System.Collections;
using System;

namespace BT
{
    public class Summon : BTTask
    {
        public EActorType ActorType;
        public Int32 Id;
        public int Count = 1;
        public float MaxRadius = 5;
        public float MinRadius = 2;

        protected override bool Enter()
        {
            for (int i = 0; i < Count; i++)
            {
                Vector3 pos = GTTools.RandomOnCircle(Owner.Pos, MinRadius, MaxRadius);
                GTLevelManager.Instance.AddActor(Id, ActorType, Owner.Camp, pos, Vector3.zero);
            }
            return true;
        }

        protected override void ReadAttribute(string key, string value)
        {
            switch (key)
            {
                case "Type":
                    this.ActorType = (EActorType)value.ToInt32();
                    break;
                case "Id":
                    this.Id = value.ToInt32();
                    break;
                case "Count":
                    this.Count = value.ToInt32();
                    break;
                case "MaxRadius":
                    this.MaxRadius = value.ToFloat();
                    break;
                case "MinRadius":
                    this.MinRadius = value.ToFloat();
                    break;
            }
        }
    }

}
