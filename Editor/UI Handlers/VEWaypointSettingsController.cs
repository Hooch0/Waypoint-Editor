using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    public class VEWaypointSettingsController
    {
        private VisualElement _root;
        private FloatField _defualtRadiusFloatField;
        private LayerMaskField _layerMaskField; 


        public VEWaypointSettingsController(VisualElement root)
        {
            _root = root;
            _defualtRadiusFloatField = _root.Q<FloatField>(WaypointConstants.WaypointSettings.DefaultRadiusFloatField);
            _layerMaskField = _root.Q<LayerMaskField>(WaypointConstants.WaypointSettings.GroundDetectionLayerMask);

            _defualtRadiusFloatField.bindingPath = WaypointConstants.WaypointSettings.DefaultRadiusFloatFieldBindingPath;
            _layerMaskField.bindingPath = WaypointConstants.WaypointSettings.GroundDetectionlayerMaskBindingPath;

            WaypointEditorSettingsAsset settings = WaypointEditorSettingsAsset.Instance;

            SerializedObject serializedObject = new SerializedObject(settings);


            _root.Bind(serializedObject);
        }
    }
}
