using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Hooch.Waypoint.Editor
{   
    [CustomEditor(typeof(WaypointSceneController))]
    public class WaypointSceneControllerInspector : UEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Edit Waypoints"))
            {
                WaypointEditorWindow.ShowWindow((WaypointSceneController)target);
            }
        }
    }
}
