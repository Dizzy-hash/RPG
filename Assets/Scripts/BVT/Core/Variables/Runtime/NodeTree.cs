using UnityEngine;
using System.Collections;
using BVT.Core;
using System.Collections.Generic;
using BVT.Core;

namespace BVT.Core
{
    public class NodeTree : MonoBehaviour
    {
        [SerializeField]
        public int               ID;
        [SerializeField]
        public string            Name       = string.Empty;
        [SerializeField]
        public string            Desc       = string.Empty;
        [SerializeField]
        public bool              Loop       = false;
        [SerializeField]
        public Node              First;
        [SerializeField]
        public List<Node>        AllGraphs  = new List<Node>();

        public bool              IsRunning { get; private set; }
        public bool              IsPause   { get; private set; }
        public System.Action     PostGUI   { get; set; }

        private Object           mCurrentSelection = null;
        private NodeBlackboard   mBlackboard       = null;
        private Transform        mNodesRoot        = null;


        public Node              AddGraph(System.Type type)
        {
            Node child = NodeFactory.CreateNode(this, type);
            child.hideFlags = HideFlags.HideInHierarchy;
            AddGraphToTree(child);
            return child;
        }

        public Node              AddGraphToTree(Node child)
        {
            child.transform.parent = NodesRoot;
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.localEulerAngles = Vector3.zero;
            child.Tree = this;
            AllGraphs.Add(child);
            if (child.CanAsFirst)
            {
                this.First = First == null ? child : this.First;
            }
            this.UpdateGraphIDs();
            child.OnCreate();
            return child;
        }

        public Node              DelGraph(Node graph)
        {
            if (graph.Parent != null)
            {
                graph.Parent.DelChild(graph);
            }
            if (graph.Children.Count > 0)
            {
                for (int i = 0; i < graph.Children.Count; i++)
                {
                    graph.Children[i].Parent = null;
                }
            }
            this.AllGraphs.Remove(graph);
            this.First = First == graph ? null : graph;
            this.UpdateFirst();
            this.UpdateGraphIDs();
            DestroyImmediate(graph.gameObject);
            return null;
        }

        public Object            FocusedNode
        {
            get { return mCurrentSelection; }
            set
            {
                GUIUtility.keyboardControl = 0;
                mCurrentSelection = value;
            }
        }

        public Node              FocusedGraph
        {
            get
            {
                if (FocusedNode is Node)
                    return FocusedNode as Node;
                else
                    return null;
            }
        }

        public NodeBlackboard    Blackboard
        {
            get
            {
                if (mBlackboard == null)
                {
                    mBlackboard = gameObject.GetComponent<NodeBlackboard>();
                }
                if (mBlackboard == null)
                {
                    mBlackboard = gameObject.AddComponent<NodeBlackboard>();
                    mBlackboard.hideFlags = HideFlags.HideInInspector;
                }
                return mBlackboard;
            }
        }

        public Transform         NodesRoot
        {
            get
            {
                if (mNodesRoot == null)
                {
                    mNodesRoot = transform.Find("Nodes");
                }
                if (mNodesRoot == null)
                {
                    mNodesRoot = new GameObject("Nodes").transform;
                    mNodesRoot.transform.parent = transform;
                    mNodesRoot.transform.localPosition = Vector3.zero;
                }
                mNodesRoot.hideFlags = HideFlags.HideInHierarchy;
                return mNodesRoot;
            }
        }

        public void              UpdateGraphIDs()
        {
            int minID = 1;
            if (First != null)
            {
                First.ID = minID;
                First.name = minID.ToString();
            }
            foreach (var current in AllGraphs)
            {
                if (current != First)
                {
                    minID++;
                    current.ID = minID;
                    current.name = minID.ToString();
                }
            }    
        }

        public void              UpdateFirst()
        {
            if (this.First != null)
            {
                return;
            }
            for (int i = 0; i < AllGraphs.Count; i++)
            {
                if(AllGraphs[i].CanAsFirst)
                {
                    this.First = AllGraphs[i];
                    break;
                }
            }
        }

        void Update()
        {
            if (First == null)
            {
                return;
            }
            First.OnTick();
            if (Loop && (First.State == ENST.SUCCESS || First.State == ENST.FAILURE))
            {
                First.OnReset();
            }
        }
    }
}
