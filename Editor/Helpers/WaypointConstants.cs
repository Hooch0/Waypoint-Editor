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
            public const string SceneAssetBinding = "_sceneAsset";
            public const string GenerateToolbarButton = "GenerateToolbarButton";
            public const string AutoGenerateToolbarToggle = "AutoGenerateToolbarToggle";
            public const string AutoGenerateBinding = "_autoGenerate";


            //Waypoint Group
            public const string GroupListView = "GroupListView";
            public const string GroupButton = "GroupButton";
            public const string GroupTextField = "GroupTextField";
            public const string WaypointGroupsBinding = "_waypointGroups";
            public const string WaypointGroupNameBinding = "_groupName";

            //Waypoint Options
            public const string AutolinkToggle = "AutolinkToggle";
            public const string LinkButton = "LinkButton";
            public const string UnlinkButton = "UnlinkButton";
            public const string SelectedIDsTextField = "SelectedIDsTextField";
            public const string AutolinkBindingPath = "_autolinkToggle";


            //Waypoint Properties
            public const string PropertiesContainer = "PropertiesContainer";
            public const string IDTextField = "IDTextField";
            public const string PositionVector3Field = "PositionVector3Field";
            public const string RadiusFloatField = "RadiusFloatField";
            public const string HeightFloatField = "HeightFloatField";
            public const string TagTextField = "TagTextField";

            //Waypoint Connections
            public const string TransitionLogicLabel = "TransitionLogicLabel";
            public const string TransitionLogicBinding = "_transitionLogic";
            public const string TransitionLogicPropertyField = "TransitionLogicPropertyField";
            public const string SetTransitionLogicButton = "SetTransitionLogicButton";
            public const string ConnectionListView = "ConnectionListView";
            public const string ConnectionsContainer = "ConnectionsContainer";
            public const string WeightIntField = "WeightIntField";

            public const string WeightBinding = "_weight";
            public const string WaypointTransitionBinding = "_transitions";

            //Waypoint Event Properties
            public const string EventsContainer = "EventsContainer";
            public const string EventListView = "EventListView";
            public const string AddEventButton = "AddEventButton";


            //Waypoint Group bindings
            public const string WaypointGroupConnectionsBinding = "_connections";
            public const string WaypointGroupWaypointListBinding = "_waypoints";

            //Waypoint Bindings
            public const string WaypointEventsBinding = "_events";
        }

        public static class WaypointSettings
        {
            public const string DefaultRadiusFloatField = "DefaultRadiusFloatField";
            public const string DefaultRadiusFloatFieldBindingPath = "_defualtRadius";
            public const string GroundDetectionLayerMask = "GroundDetectionLayerMask";
            public const string GroundDetectionlayerMaskBindingPath = "_groundDetectionLayer";
        }
    }
}
