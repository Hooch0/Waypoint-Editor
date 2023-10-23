using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    public class WaypointSettingsWindow : EditorWindow
    {
        private VEWaypointSettingsController _veController;

        [MenuItem("Tools/Waypoint System/Settings", priority = 40)]
        public static WaypointSettingsWindow ShowWindow()
        {
            WaypointSettingsWindow wnd = GetWindow<WaypointSettingsWindow>();
            wnd.titleContent = new GUIContent("Waypoint Settings");
            wnd.minSize = new Vector2(400,200);
            WaypointEditorSettingsAsset insntace = WaypointEditorSettingsAsset.Instance;
            return wnd;
        }

        public void CreateGUI()
        {
            VisualTreeAsset tree = WaypointResourceAsset.Instance.SettingsUI;
            tree.CloneTree(rootVisualElement);
            _veController = new VEWaypointSettingsController(rootVisualElement);
        }

        
    }
}
