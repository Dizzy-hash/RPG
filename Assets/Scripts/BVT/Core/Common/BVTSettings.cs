using UnityEngine;
using System.Collections;

namespace BVT.Core
{
#if UNITY_EDITOR
    public class BVTSettings
    {
        public static bool          ShowNodeIcon
        {
            get { return UnityEditor.EditorPrefs.GetBool("BVT.ShowNodeIcon", true); }
            set { UnityEditor.EditorPrefs.SetBool("BVT.ShowNodeIcon", value); }
        }

        public static bool          ShowBlackboard
        {
            get { return UnityEditor.EditorPrefs.GetBool("BVT.ShowBlackboard", true); }
            set { UnityEditor.EditorPrefs.SetBool("BVT.ShowBlackboard", value); }
        }

        public static bool          ShowNodeComment
        {
            get { return UnityEditor.EditorPrefs.GetBool("BVT.ShowNodeComment", true); }
            set { UnityEditor.EditorPrefs.SetBool("BVT.ShowNodeComment", value); }
        }

        public static bool          ShowNodeInfo
        {
            get { return UnityEditor.EditorPrefs.GetBool("BVT.ShowNodeInfo", true); }
            set { UnityEditor.EditorPrefs.SetBool("BVT.ShowNodeInfo", value); }
        }
    }
#endif
}

