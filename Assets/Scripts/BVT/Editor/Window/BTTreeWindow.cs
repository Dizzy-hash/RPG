using BVT.Core;
using UnityEditor;
using UnityEngine;

namespace BVT.EDT
{
    public class BTTreeWindow : EditorWindow
    {
        private GUIStyle     mToolbarStyle;
        private GUIStyle     mToolbarTextFieldStyle;
        private GUIStyle     mToolbarButtonStyle;
        private GUIStyle     mTextCenterStyle;
        private GUISkin      mBVTSkin;
        private Rect         mGraphRect           = new Rect(0, 0, 2000, 2000);
        private Rect         mBlackboardRect      = new Rect(15, 55, 0, 0);
        private Rect         mInspectorRect       = new Rect(15, 55, 0, 0);
        private NodeTree     mTree                = null;
        private int          mTreeInstanceID      = 0;
        private Vector2      mScrollPos           = Vector2.zero;

        public GUIStyle      ToolbarStyle
        {
            get
            {
                if(mToolbarStyle==null)
                {
                    this.mToolbarStyle = new GUIStyle("toolbar");
                    this.mToolbarStyle.fixedHeight = 30;
                    this.mToolbarStyle.alignment = TextAnchor.MiddleCenter;
                    this.mToolbarStyle.fontSize = 20;
                }
                return mToolbarStyle;
            }
        }

        public GUIStyle      ToolbarTextFieldStyle
        {
            get
            {
                if (mToolbarTextFieldStyle == null)
                {
                    this.mToolbarTextFieldStyle = new GUIStyle("toolbarTextField");
                    this.mToolbarTextFieldStyle.fixedHeight = 30;
                    this.mToolbarTextFieldStyle.alignment = TextAnchor.MiddleCenter;
                    this.mToolbarTextFieldStyle.fontSize = 20;
                }
                return mToolbarTextFieldStyle;
            }
        }

        public GUIStyle      ToolbarButtonStyle
        {
            get
            {
                if (mToolbarButtonStyle == null)
                {
                    this.mToolbarButtonStyle = new GUIStyle("toolbarButton");
                    this.mToolbarButtonStyle.fixedHeight = 30;
                    this.mToolbarButtonStyle.alignment = TextAnchor.MiddleCenter;
                    this.mToolbarButtonStyle.fontSize = 20;
                }
                return mToolbarButtonStyle;
            }
        }

        public GUIStyle      TextCenterStyle
        {
            get
            {
                if (mTextCenterStyle == null)
                {
                    this.mTextCenterStyle = new GUIStyle("label");
                    this.mTextCenterStyle.alignment = TextAnchor.UpperCenter;
                }
                return mTextCenterStyle;
            }
        }

        public GUISkin       BVTSkin
        {
            get
            {
                if(mBVTSkin==null)
                {
                    mBVTSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Scripts/BVT/Editor/Resource/Skin/BVTSkin.guiskin");
                }
                return mBVTSkin;
            }
        }

        public NodeTree       GraphTree
        {
            get { return mTree; }
            set { mTree = value; }
        }

        public System.Type   FocuseType
        {
            get { return this.mTree.FocusedGraph.GetType(); }
        }      

        void OnEnable()
        {
            this.titleContent = new GUIContent("BVT Editor");
        }

        void OnGUI()
        {
            if(EditorApplication.isCompiling)
            {
                return;
            }
            if (mTree == null)
            {
                return;
            }
            if(PrefabUtility.GetPrefabType(mTree)==PrefabType.Prefab)
            {
                return;
            }
            DrawToorBar();
            GUI.skin = BVTSkin;

            BeginWindows();
            DrawAllNodes();
            EndWindows();

            DrawTreeInfo();
            DrawBlackboardInfo();
            DrawSelectNodeInfo();
            DrawNodeGraphControls();

            Repaint();
            GUI.skin = null;
        }

        void OnSelectionChange()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            var lastWindow = EditorWindow.focusedWindow;
            var tree = Selection.activeGameObject.GetComponent<NodeTree>();
            if (tree != null)
            {
                OpenWindow(tree);
            }
            if(lastWindow!=null)
            {
                EditorWindow.GetWindow(lastWindow.GetType());
            }
        }

        void OnInspectorUpdate()
        {
            if (EditorUtility.InstanceIDToObject(mTreeInstanceID) is NodeTree)
            {
                mTree = EditorUtility.InstanceIDToObject(mTreeInstanceID) as NodeTree;
                Repaint();
            }
        }

        private void DrawToorBar()
        {
            GUILayout.BeginHorizontal(ToolbarStyle, GUILayout.ExpandWidth(true));
            bool showNodeIcon     = BVTSettings.ShowNodeIcon;
            bool showBlackboard   = BVTSettings.ShowBlackboard;
            bool showNodeDesc     = BVTSettings.ShowNodeComment;

            GUI.backgroundColor = new Color(0.8f, 0.8f, 1);
            if (GUILayout.Button("类型", ToolbarButtonStyle, GUILayout.Width(120)))
            {

            }

            GUI.backgroundColor = Color.green;
            BVTSettings.ShowNodeComment  = GUILayout.Toggle(showNodeDesc,     showNodeDesc ?      "隐藏备注":"显示备注", ToolbarButtonStyle, GUILayout.Width(120));
            BVTSettings.ShowNodeIcon     = GUILayout.Toggle(showNodeIcon,     showNodeIcon ?      "文字模式":"图片模式", ToolbarButtonStyle, GUILayout.Width(120));
            BVTSettings.ShowBlackboard   = GUILayout.Toggle(showBlackboard,   showBlackboard ?    "隐藏黑板":"显示黑板", ToolbarButtonStyle, GUILayout.Width(120));


            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("加载Tree", ToolbarButtonStyle, GUILayout.Width(120)))
            {

            }
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("保存Tree", ToolbarButtonStyle, GUILayout.Width(120)))
            {

            }

            GUILayout.Space(100);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("清除所有节点", ToolbarButtonStyle, GUILayout.Width(200)))
            {

            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        private void DrawBackground()
        {
            Texture2D bgTex = null;
            Texture2D bgLineTex = null;
            if (bgTex == null || bgLineTex == null)
            {
                return;
            }
            float fixHeight = ToolbarStyle.fixedHeight;
            float width = 15;
            {
                Vector2 pos = new Vector2(0, fixHeight);
                mGraphRect.position = pos;
                GUI.DrawTexture(mGraphRect, bgTex);
            }
            for (int i = 0; i < mGraphRect.width / width; i++)
            {
                Rect rectPos = new Rect(width * i, 0, 1f, mGraphRect.height);
                rectPos.position = new Vector2(rectPos.x, fixHeight);
                GUI.DrawTexture(rectPos, bgLineTex);
            }

            int j = Mathf.RoundToInt(ToolbarStyle.fixedHeight / width);
            for (; j < mGraphRect.height / width; j++)
            {
                Rect rectPos = new Rect(0, width * j, mGraphRect.width, 1f);
                rectPos.position = new Vector2(rectPos.x, rectPos.y - 1);
                GUI.DrawTexture(rectPos, bgLineTex);
            }
        }

        private void DrawTreeInfo()
        {
            GUILayout.BeginVertical();
            GUI.color = Color.yellow;
            TextCenterStyle.fontSize = 20;
            GUILayout.Label(mTree.Name, TextCenterStyle);
            TextCenterStyle.fontSize = 12;
            GUILayout.Label(string.Format("{0}  {1}", mTree, mTree.ID), TextCenterStyle);
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }

        private void DrawBlackboardInfo()
        {
            if (!BVTSettings.ShowBlackboard)
            {
                return;
            }
            mBlackboardRect.width = 300;
            mBlackboardRect.x = Screen.width - 310;
            mBlackboardRect.y = 50;
            mBlackboardRect.height = mTree.Blackboard.Data.Count * 20 + 110;
            GUISkin lastSkin = GUI.skin;
            GUI.Box(mBlackboardRect, "", BVTGUIStyle.BVT_WindowShadow);
            GUILayout.BeginArea(mBlackboardRect, "Variables", new GUIStyle(BVTGUIStyle.BVT_Panel));
            GUILayout.Space(10);
            GUI.skin = null;
            mTree.Blackboard.SetRoot(mTree.transform);
            mTree.Blackboard.DrawGUI();
            GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(mBlackboardRect.width - 10));
            GUI.skin = lastSkin;
            GUILayout.EndArea();
        }

        private void DrawSelectNodeInfo()
        {
            if (mTree.FocusedGraph == null)
            {
                mInspectorRect.height = 0;
                GUILayout.BeginArea(Rect.zero);
                GUILayout.EndArea();
                return;
            }

            mInspectorRect.width = 320;
            mInspectorRect.x = 10;
            mInspectorRect.y = 50;
            GUISkin lastSkin = GUI.skin;
            GUI.Box(mInspectorRect, "", BVTGUIStyle.BVT_WindowShadow);

            NodeNameAttribute nameAttr = BVTEditorHelper.GetAttribute<NodeNameAttribute>(FocuseType);
            GUILayout.BeginArea(mInspectorRect, nameAttr != null ? nameAttr.Name : FocuseType.Name, new GUIStyle(BVTGUIStyle.BVT_Panel));
            GUILayout.Space(5);
            GUI.skin = null;

            if (BVTSettings.ShowNodeInfo)
            {
                GUI.backgroundColor = new Color(0.8f, 0.8f, 1);
                NodeDescAttribute descAttr = BVTEditorHelper.GetAttribute<NodeDescAttribute>(FocuseType);
                EditorGUILayout.HelpBox(descAttr != null ? descAttr.Desc : string.Empty, MessageType.None, true);
                GUI.backgroundColor = Color.white;
                mTree.FocusedGraph.DrawNodeInspectorGUI();
            }

            GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(mInspectorRect.width - 1));
            GUI.skin = lastSkin;
            if (Event.current.type == EventType.Repaint)
            {
                mInspectorRect.height = GUILayoutUtility.GetLastRect().yMax + 5;
            }
            GUILayout.EndArea();
        }

        private void DrawNodeGraphControls()
        {
            Event e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown && mInspectorRect.Contains(e.mousePosition))
            {
                e.Use();
            }
            if (!mInspectorRect.Contains(e.mousePosition) &&
                !mBlackboardRect.Contains(e.mousePosition))
            {
                if (e.button == 0 && e.isMouse && e.type == EventType.MouseDown)
                {
                    mTree.FocusedNode = null;
                    return;
                }
                if (e.button == 1 && e.type == EventType.MouseDown)
                {
                    var pos = e.mousePosition + mScrollPos;
                    BVTEditorHelper.ShowComponentsSelectionMenu(typeof(Node), (selectType) =>
                    {
                        Node graph = mTree.AddGraph(selectType);
                        graph.NodeRect.center = pos;
                        mTree.FocusedNode = graph;
                    });
                    e.Use();
                }
            }

            if (e.isKey && e.alt && e.keyCode == KeyCode.Q)
            {
                foreach(Node graph in mTree.AllGraphs)
                {
                    graph.NodeRect.width  = Node.MinSize.x;
                    graph.NodeRect.height = Node.MinSize.y;
                }
                e.Use();
            }

            if (mTree.PostGUI != null)
            {
                mTree.PostGUI.Invoke();
                mTree.PostGUI = null;
            }
        }

        private void DrawAllNodes()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            foreach (var current in mTree.AllGraphs)
            {
                if (current != null)
                {
                    current.DrawGUI();
                }
            }
            EditorUtility.SetDirty(this);
        }

        public static BTTreeWindow OpenWindow(NodeTree tree)
        {
            var window = EditorWindow.GetWindow<BTTreeWindow>();
            window.mTreeInstanceID = tree.GetInstanceID();
            window.mTree = tree;
            window.mTree.FocusedNode = null;
            window.mTree.UpdateGraphIDs();
            return window;
        }

        public static BTTreeWindow ShutWindow()
        {
            var window = EditorWindow.GetWindow<BTTreeWindow>();
            window.Close();
            return null;
        }
    }
}