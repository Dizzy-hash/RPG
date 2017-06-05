using UnityEngine;
using System.Collections;
using UnityEditor;

namespace EDT
{
    [CustomEditor(typeof(UIFlyword), false)]
    public class EditorFlyword : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIFlyword word = target as UIFlyword;
            word.text         = EditorGUILayout.TextField("Text", word.text);
            word.color        = EditorGUILayout.ColorField("Color", word.color);
            word.outlineColor = EditorGUILayout.ColorField("OutlineColor", word.outlineColor);
            word.GenerateFilter();
        }
    }
}