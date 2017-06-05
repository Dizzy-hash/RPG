using UnityEngine;
using System.Collections;
using BVT.Core;
using BVT.Core;

namespace BVT.Behaviour
{
    [NodeAttribute(Type = "条件节点", Label = "CheckDistance")]
    public class CheckDistance : BTCondition
    {
        [SerializeField]
        public ShareGameObject Source;
        [SerializeField]
        public ShareGameObject Target;
        [SerializeField]
        public ShareFloat      Distance;
        [NodeVariable]
        public bool            IgnoreY  = false;

        public override void OnCreate()
        {
            this.Source   = NodeFactory.CreateShareObject<ShareGameObject>(Tree.Blackboard);
            this.Target   = NodeFactory.CreateShareObject<ShareGameObject>(Tree.Blackboard);
            this.Distance = NodeFactory.CreateShareObject<ShareFloat>(Tree.Blackboard);
        }

        public override bool Check()
        {
            if (Source.value == null)
            {
                return false;
            }
            if (Target.value == null)
            {
                return false;
            }
            if (Distance.value <= 0)
            {
                return false;
            }
            Vector3 srcPos = Source.value.transform.position;
            Vector3 tarPos = Target.value.transform.position;
            if (!IgnoreY)
            {
                return Vector3.Distance(srcPos, tarPos) < Distance.value;
            }
            else
            {
                srcPos.y = 0;
                tarPos.y = 0;
                return Vector3.Distance(srcPos, tarPos) < Distance.value;
            }
        }

#if UNITY_EDITOR

        public override void DrawNodeWindowContents()
        {
            if (IgnoreY)
            {
                string s = string.Format("if Distance <{0} From {1} to {2}", Distance.ToEncode(), Source.ToEncode(), Target.ToEncode());
                GUILayout.Label(s);
            }
            else
            {
                string s = string.Format("if Distance3D <{0} From {1} to {2}", Distance.ToEncode(), Source.ToEncode(), Target.ToEncode());
                GUILayout.Label(s);
            }
        }

        public override void DrawNodeInspectorGUI()
        {
            base.DrawNodeInspectorGUI();
            BVTEditorHelper.DrawCoolTitle("ShareObjects");
            this.Source.    DrawGUI("Source");
            this.Target.    DrawGUI("Target");
            this.Distance.  DrawGUI("Distance");
        }
#endif
    }
}
