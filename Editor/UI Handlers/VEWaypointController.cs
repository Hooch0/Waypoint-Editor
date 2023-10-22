using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
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
        private ObjectField _waypointSceneDataField;
        private Button _createSceneData;
        private Button _generateRuntimeMapButton;
        private Toggle _autoGenerateToggle;

        private SerializedObject _serializedSceneData;
        private SerializedProperty _autoGenProp; 
        private const string _AUTO_GENERATE_KEY = "Varadia.WaypointSettings.autoGenerate";


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
            _createSceneData.clicked += OnCreateSceenData;

            _generateRuntimeMapButton = _root.Q<Button>(WaypointConstants.WaypointEditor.GenerateRuntimeMapButton);
            _generateRuntimeMapButton.clicked += OnGenerateRuntimeMap;

            _autoGenerateToggle = _root.Q<Toggle>(WaypointConstants.WaypointEditor.AutoGenerateToggle);
            _autoGenProp = editor.SerializedObject.FindProperty(WaypointConstants.WaypointEditor.AutogenerateBinding);
            _autoGenProp.boolValue = EditorPrefs.GetBool(_AUTO_GENERATE_KEY, false);
            editor.SerializedObject.ApplyModifiedProperties();

            _autoGenerateToggle.BindProperty(_autoGenProp);
            _autoGenerateToggle.RegisterValueChangedCallback(OnAutoGenerateToggleChanged);


            _waypointSceneDataField = _root.Q<ObjectField>(WaypointConstants.WaypointEditor.WaypointSceneDataField);
            _waypointSceneDataField.RegisterValueChangedCallback(OnWaypointSceneDataFieldValueChanged);
            _waypointSceneDataField.BindProperty(editor.SerializedObject.FindProperty(WaypointConstants.WaypointEditor.SceneControllerBinding));
        }

        private void OnAutoGenerateToggleChanged(ChangeEvent<bool> evt)
        {
            EditorPrefs.SetBool(_AUTO_GENERATE_KEY, evt.newValue);
        }

        private void OnWaypointSceneDataFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            _editor.UpdateSceneData();
            ChangeSceneData((WaypointSceneController)evt.newValue);
        }

        private void ChangeSceneData(WaypointSceneController sceneData)
        {
            if (sceneData == null)
            {
                _serializedSceneData = null;

                _groupHandler.UpdateSceneData(null);
                _propertiesHandler.UpdateSceneData(null);

                _coreInspector.SetEnabled(false);
                _createSceneData.SetEnabled(true);
            }
            else
            {
                _serializedSceneData = _editor.SerializedSceneController;

                _coreInspector.SetEnabled(true);
                _createSceneData.SetEnabled(false);

                _groupHandler.UpdateSceneData(_serializedSceneData);
                _propertiesHandler.UpdateSceneData(_serializedSceneData);
            }
        }

        private void OnCreateSceenData()
        {
            WaypointUtility.CreateSceenData();
        }


        private void OnGenerateRuntimeMap()
        {
            _editor.GenerateRuntimeMap();
        }
    }
}
