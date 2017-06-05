using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT
{
    public class BeatBack : BTTask
    {
        protected override bool Enter()
        {
            base.Enter();
            List<Actor> list = (List<Actor>)BTTreeManager.Instance.GetData(this, GTDefine.BT_JUDGE_LIST);
            if (list == null)
            {
                return false;
            }
            for(int i=0;i<list.Count;i++)
            {
                Actor actor = list[i];
                actor.Command.Get<CommandBeatBack>().Do();
            }
            return true;
        }

        protected override EBTStatus Execute()
        {
            return EBTStatus.BT_SUCCESS;
        }

        public override BTNode DeepClone()
        {
            BeatBack data = new BeatBack();
            return data;
        }
    }
}

