namespace Hooch.Waypoint.Editor
{
    public static class WaypointConstants
    {
        public static class WaypointEditor
        {
            public const string DEFAULT_GROUP_NAME = "Default";

            //Overall
            public const string CoreInspector = "CoreInspector";
            public const string WaypointSceneDataField = "WaypointSceneDataField";
            public const string CreateSceneController = "CreateSceneController";
            public const string GenerateRuntimeMapButton = "GenerateRuntimeMapButton";
            public const string AutoGenerateToggle = "AutoGenerateToggle";
            public const string SceneControllerBinding = "_sceneController";
            public const string AutogenerateBinding = "_autoGenerate";

            //Waypoint Group
            public const string GroupListView = "GroupListView";
            public const string GroupButton = "GroupButton";
            public const string GroupTextField = "GroupTextField";
            public const string WaypointGroupsBinding = "_waypointGroups";
            public const string WaypointGroupNameBinding = "_groupName";

            //Waypoint Options
            public const string EditingToggle = "EditingToggle";
            public const string AutolinkToggle = "AutolinkToggle";
            public const string LinkButton = "LinkButton";
            public const string UnlinkButton = "UnlinkButton";
            public const string SelectedIDsTextField = "SelectedIDsTextField";
            public const string EditingToggleBindingPath = "_editingToggle";
            public const string AutolinkBindingPath = "_autolinkToggle";


            //Waypoint Properties
            public const string PropertiesContainer = "PropertiesContainer";
            public const string IDTextField = "IDTextField";
            public const string PositionVector3Field = "PositionVector3Field";
            public const string RadiusFloatField = "RadiusFloatField";
            public const string HeightFloatField = "HeightFloatField";
            public const string TagTextField = "TagTextField";
            public const string ConnectionListView = "ConnectionListView";
            public const string ConnectionsContainer = "ConnectionsContainer";
            public const string ProbabilityFloatField = "ProbabilityFloatField";
            public const string ProbabilityBinding = "_probability";
            public const string WaypointTransitionBinding = "_transitions";
            public const string WaypointConnectionsBinding = "_connections";
        }
    
        public static class WaypointSettings
        {
            public const string GroundDetectionLayerMask = "GroundDetectionLayerMask";
            public const string GroundDetectionlayerMaskBindingPath = "_groundDetectionLayer";
        }
    }
}
