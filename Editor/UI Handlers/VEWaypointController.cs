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

        private SerializedObject _serializedSceneData;
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

            ObjectChangeEvents.changesPublished += OnChangesPublished;

            _createSceneData = _root.Q<Button>(WaypointConstants.WaypointEditor.CreateSceneController);
            _createSceneData.clicked += OnCreateSceenData;

            editor.SerializedObject.ApplyModifiedProperties();

            _waypointSceneDataField = _root.Q<ObjectField>(WaypointConstants.WaypointEditor.WaypointSceneDataField);
            _waypointSceneDataField.RegisterValueChangedCallback(OnWaypointSceneDataFieldValueChanged);
            _waypointSceneDataField.BindProperty(editor.SerializedObject.FindProperty(WaypointConstants.WaypointEditor.SceneControllerBinding));
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
            _editor.SetSceneData(WaypointUtility.CreateSceenData());
        }

        private void OnChangesPublished(ref ObjectChangeEventStream stream)
        {
            for (int i = 0; i < stream.length; i++)
            {
                if (stream.GetEventType(i) == ObjectChangeKind.DestroyGameObjectHierarchy)
                {
                    stream.GetDestroyGameObjectHierarchyEvent(i, out DestroyGameObjectHierarchyEventArgs data);

                    if (data.instanceId == _editor.CurrentSceneControllerInstanceID)
                    {
                        _editor.SetSceneData(null);
                    }
                }
            }
        }
    }
}
