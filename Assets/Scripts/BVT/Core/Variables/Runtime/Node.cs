using UnityEngine;
using System;
using System.Collections.Generic;

namespace BVT.Core
{
    public class Node : MonoBehaviour, INode
    {
        [SerializeField]
        public int                 ID;
        [SerializeField]
        public Node                Parent       = null;
        [SerializeField]
        public List<Node>          Children     = new List<Node>();
        [SerializeField]
        public Rect                NodeRect     = new Rect(100, 300, 100, 40);
        [SerializeField]
        public string              NodeComment  = string.Empty;

        public ENST                State                  { get; private set; }
        public bool                Running                { get; private set; }
        public float               StartupTime            { get; private set; }
        public float               ElapsedTime            { get { return Time.time - StartupTime; } }

        public virtual int         MaxChildCount          { get { return -1; } }
        public virtual bool        CanAsFirst             { get { return true; } }
        public bool                CanAsParent            { get { return MaxChildCount != 0; } }
        public virtual bool        AutoDrawNodeInspector  { get { return true; } }

        public NodeTree            Tree                   { get; set; }
        public string              NodeIconName           { get; protected set; }
        public Texture2D           NodeIcon               { get; protected set; }
        public Node                FirstChild             { get { return Children.Count > 0 ? Children[0] : null; } }

        void OnEnable()
        {
            if (Tree == null)
            {
                Tree = GetComponentInParent<NodeTree>();
            }
        }

        public virtual void AddChild(Node child)
        {
            if (child == null || child.Parent != null)
            {
                return;
            }
            if (Children.Contains(child) == false)
            {
                Children.Add(child);
                child.Parent = this;
            }
        }

        public virtual void DelChild(Node child)
        {
            if (child == null)
            {
                return;
            }
            child.Parent = null;
            Children.Remove(child);
        }

        public virtual void OnCreate()
        {

        }

        public virtual bool OnEnter()
        {
            this.StartupTime = Time.time;
            return true;
        }

        public virtual ENST OnExecute()
        {
            return ENST.RUNNING;
        }

        public void         OnTick()
        {
            if (State == ENST.INITIAL)
            {
                bool checkCondition = OnEnter();
                this.State = checkCondition ? ENST.RUNNING : ENST.FAILURE;
            }
            if (State == ENST.RUNNING)
            {
                ENST nodeStatus = OnExecute();
                this.Running    = nodeStatus == ENST.RUNNING;
                this.State      = nodeStatus;
            }
            else
            {
                this.Running = false;
            }
        }

        public virtual void OnReset()
        {
            this.State = ENST.INITIAL;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnReset();
            }
        }

        public virtual void OnExit(ENST state)
        {
            this.Running = false;
            this.State = state;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnExit(state);
            }
        }


#if UNITY_EDITOR
        public static Color   SuccessColor = new Color(0.4f, 0.7f, 0.2f);
        public static Color   FailureColor = new Color(1.0f, 0.4f, 0.4f);
        public static Color   RunningColor = Color.yellow;
        public static Color   RestingColor = new Color(0.5f, 0.5f, 0.8f, 0.8f);
        public static Color   MistakeColor = Color.red;
        public static Color   ConnectColor = new Color(0.5f, 0.5f, 0.8f, 0.8f);
        public static Vector2 MinSize      = new Vector2(100, 40);
        public static float   LineSize     = 5;
        public static Port    ClickedPort  = null;

        public void DrawGUI()
        {
            if (Tree == null)
            {
                Tree = GetComponentInParent<NodeTree>();
            }
            if (Tree.First == this)
            {
                GUI.Box(new Rect(NodeRect.x, NodeRect.y - 20, NodeRect.width, 20), "<b>START</b>");
            }
            DrawNodeWindow();
            DrawNodeComments();
            DrawNodeConnections();
            BVTUndo.CheckDirty(this);
        }

        void DrawNodeComments()
        {
            if (BVTSettings.ShowNodeComment)
            {
                var rect = new Rect();
                var height = new GUIStyle("textArea").CalcHeight(new GUIContent(NodeComment), NodeRect.width);
                rect = new Rect(NodeRect.x, NodeRect.yMax + 5, NodeRect.width, height);
                if (!string.IsNullOrEmpty(NodeComment))
                {
                    GUI.color = new Color(1, 1, 1, 0.6f);
                    GUI.backgroundColor = new Color(1f, 1f, 1f, 0.2f);
                    GUI.Box(rect, NodeComment, "label");
                    GUI.backgroundColor = Color.white;
                    GUI.color = Color.white;
                }
            }
        }

        void DrawNodeConnections()
        {
            Event e = Event.current;

            if (ClickedPort != null && (e.type == EventType.MouseUp || e.type == EventType.Used))
            {
                if (NodeRect.Contains(e.mousePosition))
                {
                    if (ClickedPort.Parent != this)
                    {
                        ClickedPort.Parent.AddChild(this);
                        ClickedPort = null;
                    }
                }
            }

            if (CanAsParent == false)
            {
                return;
            }

            var nodeOutputBox = new Rect(NodeRect.x, NodeRect.yMax - 4, NodeRect.width, 12);
            GUI.Box(nodeOutputBox, "", BVTGUIStyle.BVT_SimpleBox);
            if (MaxChildCount == -1 || Children.Count < MaxChildCount)
            {
                for (int i = 0; i < Children.Count + 1; i++)
                {
                    float x = (NodeRect.width / (Children.Count + 1)) * (i + 0.5f) + NodeRect.xMin;
                    float y = NodeRect.yMax + 6;

                    Rect portRect = new Rect(0, 0, 10, 10);
                    portRect.center = new Vector2(x, y);
                    GUI.Box(portRect, "", BVTGUIStyle.BVT_NodePort);

                    if (e.button == 0 && e.type == EventType.MouseDown && portRect.Contains(e.mousePosition))
                    {
                        ClickedPort = new Port(i, this, portRect.center);
                    }
                }
            }

            if (ClickedPort != null && ClickedPort.Parent == this)
            {
                UnityEditor.Handles.DrawBezier(ClickedPort.Pos, e.mousePosition, ClickedPort.Pos, e.mousePosition, new Color(0.5f, 0.5f, 0.8f, 0.8f), null, 2);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Node child = Children[i];
                float x = (NodeRect.width / (Children.Count + 1)) * (i + 0.5f) + NodeRect.xMin;
                float y = NodeRect.yMax + 6;
                var srcPos = new Vector2(x, y);
                var tarPos = child.NodeRect.center;
                DrawNodeConnect(srcPos, tarPos, this, child);
            }
        }

        void DrawNodeConnect(Vector3 lineFr, Vector3 lineTo, Node parent, Node child)
        {
            float tangentX = Mathf.Abs(lineFr.x - lineTo.x);
            float tangentY = Mathf.Abs(lineFr.y - lineTo.y);

            GUI.color = ConnectColor;
            Rect arrowRect = new Rect(0, 0, 20, 20);
            arrowRect.center = lineTo;

            Vector3 lineFrTangent = Vector3.zero;
            Vector3 lineToTangent = Vector3.zero;

            if (lineFr.x <= parent.NodeRect.x)
                lineFr = new Vector3(-tangentX, 0, 0);

            if (lineFr.x >= parent.NodeRect.xMax)
                lineFr = new Vector3(tangentX, 0, 0);

            if (lineFr.y <= parent.NodeRect.y)
                lineFrTangent = new Vector3(0, -tangentY, 0);

            if (lineFr.y >= parent.NodeRect.yMax)
                lineFrTangent = new Vector3(0, tangentY, 0);

            if (lineTo.x <= child.NodeRect.x)
            {
                lineToTangent = new Vector3(-tangentX, 0, 0);
                GUI.Box(arrowRect, "", BVTGUIStyle.BVT_Arrow_Left);
            }

            if (lineTo.x >= child.NodeRect.xMax)
            {
                lineToTangent = new Vector3(tangentX, 0, 0);
                GUI.Box(arrowRect, "", BVTGUIStyle.BVT_Arrow_Right);
            }

            if (lineTo.y <= child.NodeRect.y)
            {
                lineToTangent = new Vector3(-tangentY, 0, 0);
                GUI.Box(arrowRect, "", BVTGUIStyle.BVT_Arrow_Top);
            }

            if (lineTo.y >= child.NodeRect.yMax)
            {
                lineToTangent = new Vector3(tangentY, 0, 0);
                GUI.Box(arrowRect, "", BVTGUIStyle.BVT_Arrow_Bottom);
            }
            GUI.color = Color.white;

            switch (child.State)
            {
                case ENST.FAILURE:
                    ConnectColor = FailureColor;
                    break;
                case ENST.SUCCESS:
                    ConnectColor = SuccessColor;
                    break;
                case ENST.INITIAL:
                    ConnectColor = RestingColor;
                    break;
                case ENST.RUNNING:
                    ConnectColor = RunningColor;
                    break;
            }

            UnityEditor.Handles.DrawBezier(lineFr, lineTo, lineFr + lineFrTangent, lineTo + lineToTangent, ConnectColor, null, LineSize);
        }

        void DrawNodeWindow()
        {
            GUI.color = (Tree.FocusedNode == this) ? new Color(0.9f, 0.9f, 1) : Color.white;

            GUI.Box(NodeRect, "", BVTGUIStyle.BVT_WindowShadow);
            NodeRect = GUILayout.Window(ID, NodeRect, DrawNodeWindowGUI, string.Empty, BVTGUIStyle.BVT_Window);
            GUI.Box(NodeRect, "", BVTGUIStyle.BVT_WindowShadow);
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.Box(new Rect(NodeRect.x + 6, NodeRect.y + 6, NodeRect.width, NodeRect.height), "", BVTGUIStyle.BVT_WindowShadow);

            if (Application.isPlaying)
            {
                switch (State)
                {
                    case ENST.SUCCESS:
                        GUI.color = SuccessColor;
                        break;
                    case ENST.RUNNING:
                        GUI.color = RunningColor;
                        break;
                    case ENST.FAILURE:
                        GUI.color = FailureColor;
                        break;
                    case ENST.INITIAL:
                        GUI.color = RestingColor;
                        break;
                }
                GUI.Box(NodeRect, "", BVTGUIStyle.BVT_WindowHighlight);
            }
            else
            {
                if (Tree.FocusedNode == this)
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.8f, 0.8f);
                    GUI.Box(NodeRect, "", BVTGUIStyle.BVT_WindowHighlight);
                }
            }
            GUI.color = Color.white;
        }

        void DrawNodeWindowGUI(int id)
        {
            Event e = Event.current;
            HandNodeWindowEvents(e);
            DrawNodeWindowName();
            DrawNodeWindowStatus();
            DrawNodeWindowContentStyle();
            DrawNodeWindowContents();
            HandNodeWindowContextMenu(e);
            GUI.DragWindow();
        }

        void DrawNodeWindowName()
        {
            NodeNameAttribute attr = BVTEditorHelper.GetAttribute<NodeNameAttribute>(this.GetType());
            string nodeName = attr == null ? string.Format("<b>{0}</b>",GetType().Name) : attr.Name;
            GUILayout.Label(nodeName, BVTGUIStyle.STYLE_CENTERLABEL);
        }

        void DrawNodeWindowStatus()
        {
            Rect markRect = new Rect(5, 5, 15, 15);
            switch (State)
            {
                case ENST.SUCCESS:
                    GUI.color = SuccessColor;
                    GUI.Box(markRect, "", BVTGUIStyle.BVT_Checkmark);
                    break;
                case ENST.FAILURE:
                    GUI.color = FailureColor;
                    GUI.Box(markRect, "", BVTGUIStyle.BVT_XMark);
                    break;
                case ENST.RUNNING:
                    GUI.color = RunningColor;
                    GUI.Box(markRect, "", BVTGUIStyle.BVT_ClockMark);
                    break;
            }
        }

        void DrawNodeWindowContentStyle()
        {
            GUI.color = Color.white;
            GUI.skin = null;
            GUI.skin.label.richText = true;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        }

        void HandNodeWindowEvents(Event e)
        {
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                Tree.FocusedNode = this;
            }
            if (e.button == 0 && e.type == EventType.MouseDrag && e.control)
            {
                PanNode(e.delta);
            }
            if (Tree.FocusedNode == this && e.keyCode == KeyCode.Delete && e.type == EventType.KeyDown)
            {
                Tree.PostGUI += () => { Tree.DelGraph(this); };
                e.Use();
            }
            if (e.isKey && e.control && e.keyCode == KeyCode.D && Tree.FocusedGraph != null)
            {            
                Tree.PostGUI += () => { Tree.FocusedNode = Tree.FocusedGraph.Duplicate(); };
                e.Use();
            }
        }

        void HandNodeWindowContextMenu(Event e)
        {
            if (e.button == 1 && e.type == EventType.MouseDown)
            {
                UnityEditor.GenericMenu menu = new UnityEditor.GenericMenu();
                if (Tree.First != this && CanAsFirst)
                {
                    menu.AddItem(new GUIContent("Make First"), false, ContextMakeFirst);
                }
                if (this.Parent != null)
                {
                    menu.AddItem(new GUIContent("Make UnParent"), false, ContextMakeUnParent);
                }

                menu.AddItem(new GUIContent("Duplicate"), false, ContextDuplicate);
                menu.AddItem(new GUIContent("Delete"), false, ContextDelete);
                menu.ShowAsContext();
                e.Use();
            }
        }

        void ContextMakeFirst()
        {
            Tree.First = this;
        }

        void ContextDuplicate()
        {
            Tree.PostGUI += () => { Duplicate(); };
        }

        void ContextDelete()
        {       
            Tree.PostGUI += () => { Tree.DelGraph(this); };
        }

        void ContextMakeUnParent()
        {
            Tree.PostGUI += () => { this.Parent.DelChild(this); };
        }

        void OnValidate()
        {
            this.hideFlags = HideFlags.HideInHierarchy;
        }

        public virtual void DrawNodeWindowContents()
        {
            GUILayout.Label(string.Empty, GUILayout.Height(1));
        }

        public virtual void DrawNodeInspectorGUI()
        {
            NodeComment = UnityEditor.EditorGUILayout.TextField("NodeComment", NodeComment);
            BVTEditorHelper.DrawSeparator();
            if (!AutoDrawNodeInspector)
            {
                return;
            }
            System.Reflection.FieldInfo[] fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                System.Reflection.FieldInfo field = fields[i];
                if (field.IsDefined(typeof(NodeVariable), false))
                {
                    object v = field.GetValue(this);
                    Type t   = field.FieldType;

                    if (t.BaseType == typeof(Enum))
                    {
                        v = UnityEditor.EditorGUILayout.EnumPopup(field.Name, (Enum)v);
                    }
                    else if (t == typeof(int))
                    {
                        v= UnityEditor.EditorGUILayout.IntField(field.Name, (int)v);
                    }
                    else if (t == typeof(bool))
                    {
                        v = UnityEditor.EditorGUILayout.Toggle(field.Name, (bool)v);
                    }
                    else if (t == typeof(float))
                    {
                        v = UnityEditor.EditorGUILayout.FloatField(field.Name, (float)v);
                    }
                    else if (t == typeof(string))
                    {
                        v = UnityEditor.EditorGUILayout.TextField(field.Name, (string)v);
                    }
                    else if (t == typeof(UnityEngine.Object))
                    {
                        v = UnityEditor.EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)v, t, false);
                    }
                    else if (t == typeof(double))
                    {
                        v = UnityEditor.EditorGUILayout.DoubleField(field.Name, (double)v);
                    }
                    else if (t == typeof(Vector3))
                    {
                        v = UnityEditor.EditorGUILayout.Vector3Field(field.Name, (Vector3)v);
                    }
                    else if (t == typeof(Vector2))
                    {
                        v = UnityEditor.EditorGUILayout.Vector2Field(field.Name, (Vector2)v);
                    }
                    else if (t == typeof(AnimationCurve))
                    {
                        v = UnityEditor.EditorGUILayout.CurveField(field.Name, (AnimationCurve)v);
                    }
                    else if (t == typeof(Color))
                    {
                        v = UnityEditor.EditorGUILayout.ColorField(field.Name, (Color)v);
                    }
                    try
                    {
                        field.SetValue(this, v);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
            }
        }

        public void PanNode(Vector2 delta)
        {
            float newX = NodeRect.center.x;
            float newY = NodeRect.center.y;
            newX += delta.x;
            newY += delta.y;
            NodeRect.center = new Vector2(newX, newY);
        }

        public Node Duplicate()
        {
            var child = GameObject.Instantiate(gameObject).GetComponent<Node>();
            child.name = this.GetType().ToString();
            child.Children.Clear();
            child.Parent = null;
            Tree.AddGraphToTree(child);
            child.NodeRect.center += new Vector2(50, 50);
            return child;
        }
#endif
    }
}