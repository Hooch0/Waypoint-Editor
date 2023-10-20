using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    public class VEWaypointSettingsController
    {
        private VisualElement _root;
        private LayerMaskField _layerMaskField; 


        public VEWaypointSettingsController(VisualElement root)
        {
            _root = root;
            _layerMaskField = _root.Q<LayerMaskField>(WaypointConstants.WaypointSettings.GroundDetectionLayerMask);

            _layerMaskField.bindingPath = WaypointConstants.WaypointSettings.GroundDetectionlayerMaskBindingPath;

            WaypointEditorSettingsScriptableObject settings = WaypointEditorSettingsScriptableObject.Instance;

            SerializedObject serializedObject = new SerializedObject(settings);



            _root.Bind(serializedObject);
        }
    }
}
