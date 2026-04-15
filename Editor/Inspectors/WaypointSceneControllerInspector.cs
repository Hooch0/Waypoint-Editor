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
            WaypointSceneController controller = (WaypointSceneController)target;

            controller.SceneAsset = (WaypointSceneAsset)EditorGUILayout.ObjectField(new GUIContent("Waypoint Scene Asset"), controller.SceneAsset, typeof(WaypointSceneAsset), false);

            GUI.enabled = controller.SceneAsset != null;
            if (GUILayout.Button("Edit Waypoints"))
            {

                WaypointEditorWindow.ShowWindow(controller.SceneAsset);
            }

            GUI.enabled = controller.SceneAsset == null;
            if (GUILayout.Button("Get or Create Scene Asset"))
            {
                controller.SceneAsset = WaypointUtility.GetOrCreateSceneAsset();

                if (EditorWindow.HasOpenInstances<WaypointEditorWindow>())
                {
                    WaypointEditorWindow window = EditorWindow.GetWindow<WaypointEditorWindow>(null, false);
                    window.SetSceneData(controller.SceneAsset);
                }
            }

            GUI.enabled = true;
        }
    }
}
