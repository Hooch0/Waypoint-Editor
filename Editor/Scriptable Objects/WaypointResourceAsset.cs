using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    public class WaypointResourceAsset : ScriptableObject
    {
        [field: SerializeField] public VisualTreeAsset EditorUI { get; private set; }
        [field: SerializeField] public VisualTreeAsset SettingsUI { get; private set; }
        [field: SerializeField] public StyleSheet EventDeleteButtonStyle { get; private set; }
        [field: SerializeField] public Texture2D WaypointIcon { get; private set; }
        [field: SerializeField] public Texture2D ToolbarIcon { get; private set; }

        private const string _WAYPOINT_RESOURCE_ASSET_NAME = "Waypoint Resource Asset";

        public static WaypointResourceAsset Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (WaypointResourceAsset)Resources.Load(_WAYPOINT_RESOURCE_ASSET_NAME);
                }

                return _instance;
            }
        }

        private static WaypointResourceAsset _instance;

    }
}
