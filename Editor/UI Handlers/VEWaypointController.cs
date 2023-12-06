using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;

namespace Hooch.Waypoint.Editor
{
    public class VEWaypointController
    {
        private VEWaypointGroupHandler _groupHandler;
        private VEWaypointOptionsHandler _optionsHandler;
        private VEWaypointPropertiesHandler _propertiesHandler;

        private WaypointEditorWindow _editor;

        private VisualElement _root;
        private VisualElement _coreInspector;
        private ObjectField _waypointSceneAssetField;
        private Button _createSceneData;
        private ToolbarButton _generateToolbarButton;

        private SerializedObject _serializedSceneAsset;


        public VEWaypointController(VisualElement root, WaypointEditorWindow editor)
        {
            _root = root;
            _editor = editor;

            _groupHandler = new VEWaypointGroupHandler(_root, _editor);
            _optionsHandler = new VEWaypointOptionsHandler(_root, _editor);
            _propertiesHandler = new VEWaypointPropertiesHandler(_root, _editor);

            _coreInspector = _root.Q<VisualElement>(WaypointConstants.WaypointEditor.CoreInspector);
            _coreInspector.SetEnabled(false);

            _createSceneData = _root.Q<Button>(WaypointConstants.WaypointEditor.CreateSceneController);
            _createSceneData.clicked += OnCreateSceenController;

            _waypointSceneAssetField = _root.Q<ObjectField>(WaypointConstants.WaypointEditor.WaypointSceneDataField);
            _waypointSceneAssetField.RegisterValueChangedCallback(OnWaypointSceneAssetFieldValueChanged);
            _waypointSceneAssetField.BindProperty(editor.SerializedWaypointEditor.FindProperty(WaypointConstants.WaypointEditor.SceneAssetBinding));

            _generateToolbarButton = _root.Q<ToolbarButton>(WaypointConstants.WaypointEditor.GenerateToolbarButton);
            _generateToolbarButton.clicked += OnGenerateToolbarButtonClicked;
        }



        private void OnWaypointSceneAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            _editor.UpdateSceneData();
            ChangeSceneData((WaypointSceneAsset)evt.newValue);
        }

        private void ChangeSceneData(WaypointSceneAsset sceneAsset)
        {
            if (sceneAsset == null)
            {
                _serializedSceneAsset = null;

                _groupHandler.UpdateSceneData(null);
                _propertiesHandler.UpdateSceneData(null);

                _coreInspector.SetEnabled(false);
                _createSceneData.SetEnabled(true);
            }
            else
            {
                _serializedSceneAsset = _editor.SerializedSceneAsset;

                _coreInspector.SetEnabled(true);
                _createSceneData.SetEnabled(false);

                _groupHandler.UpdateSceneData(_serializedSceneAsset);
                _propertiesHandler.UpdateSceneData(_serializedSceneAsset);
            }
        }

        private void OnCreateSceenController()
        {
            WaypointSceneController controller = WaypointUtility.CreateSceenController();
            controller.SceneAsset = WaypointUtility.GetAndCreateSceneAsset();
            _editor.SetSceneData(controller.SceneAsset);
        }

        private void OnGenerateToolbarButtonClicked()
        {
            _editor.GenerateRuntimeMap();
        }
    }
}
