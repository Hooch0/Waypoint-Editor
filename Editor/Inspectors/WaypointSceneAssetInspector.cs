using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Hooch.Waypoint.Editor
{
    [CustomEditor(typeof(WaypointSceneAsset))]
    public class WaypointSceneAssetInspector : UEditor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = true;
        }
    }
}
