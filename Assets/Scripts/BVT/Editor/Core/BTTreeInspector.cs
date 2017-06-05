using BVT.Core;
using UnityEditor;
using UnityEngine;

namespace BVT.EDT
{
    [CustomEditor(typeof(NodeTree), true)]
    public class BVTGraphTreeInspector : Editor
    {
        public NodeTree          Tree { get { return target as NodeTree; } }

        public override void OnInspectorGUI()
        {
            if(IsPrefab())
            {
                return;
            }
            if(Tree.IsRunning)
            {
                return;
            }
            GUILayout.Space(0);
            Tree.ID   = EditorGUILayout.IntField ("Tree ID",   Tree.ID);
            Tree.Name = EditorGUILayout.TextField("Tree Name", Tree.Name);
            Tree.Loop = EditorGUILayout.Toggle(   "Tree Loop", Tree.Loop);
            EditorGUILayout.LabelField("Tree Desc");
            Tree.Desc = EditorGUILayout.TextArea ( Tree.Desc, GUILayout.Height(50));
     

            GUI.backgroundColor = new Color(0.8f, 0.8f, 1);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Editor", GUILayout.Width(100), GUILayout.Height(30)))
            {
                BTTreeWindow.OpenWindow(Tree);
            }
            if (GUILayout.Button("Shut", GUILayout.MaxWidth(60), GUILayout.Height(30)))
            {
                BTTreeWindow.ShutWindow();
            }

            if (GUILayout.Button("Save", GUILayout.MaxWidth(60), GUILayout.Height(30)))
            {

            }
            if (GUILayout.Button("Load", GUILayout.MaxWidth(60), GUILayout.Height(30)))
            {

            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.black;
            GUILayout.Space(10);

            Tree.Blackboard.SetRoot(Tree.transform);
            Tree.Blackboard.DrawGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        private bool IsPrefab()
        {
            return PrefabUtility.GetPrefabType(Tree) == PrefabType.Prefab;
        }
    }
}