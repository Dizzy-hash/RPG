using UnityEngine;
using System.Collections;
using UnityEditor;

namespace EDT
{
    [CustomEditor(typeof(CharacterView), true)]
    public class EditorCharacter : Editor
    {
        public SerializedProperty characterAttrProperty;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CharacterView view = target as CharacterView;
            Character cc = view.Owner;
            if (cc == null)
            {
                return;
            }
            EditorGUILayout.TextField("ID", cc.ID.ToString());
            EditorGUILayout.TextField("Type", cc.Type.ToString());
            EditorGUILayout.TextField("GUID", cc.GUID.ToString());
            EditorGUILayout.TextField("Camp", cc.Camp.ToString());
            EditorGUILayout.TextField("FSM", cc.FSM.ToString());

            if (cc.Target != null) EditorGUILayout.ObjectField("Target", cc.Target.View, typeof(CharacterView), true);
            if (cc.Obj != null) EditorGUILayout.ObjectField("Obj", cc.Obj, typeof(CharacterView), true);
            if (cc.ObjTrans != null) EditorGUILayout.ObjectField("ObjTrans", cc.Obj, typeof(CharacterView), true);
            if (cc.CacheTransform != null) EditorGUILayout.ObjectField("CacheTransform", cc.CacheTransform, typeof(CharacterView), true);
            if (cc.CurrAttr != null) view.Attr = cc.CurrAttr;
        }
    }
}

