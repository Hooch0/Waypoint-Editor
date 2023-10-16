using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{

    public class VEWaypointPropertiesHandler
    {
        private VisualElement _root;

        private VisualElement _propertiesContainer;
        private TextField _idTextField;
        private Vector3Field _positionVector3Field;
        private FloatField _positionVector3XField;
        private FloatField _positionVector3YField;
        private FloatField _positionVector3ZField;
        private FloatField _radiusFloatField;
        private FloatField _heightFloatField;

        private VisualElement _connectionsContainer;
        private ListView _connectionsListView;
        private FloatField _probabilityFloatField;
        
        private SerializedObject _serializedSceneData;
        private SerializedProperty _currentSerializedConnections;
        private SerializedProperty _currentSerializedTransitions;

        private List<Waypoint> _currentSelectedWaypoints;
        private WaypointEditorWindow _editor;
        

        public VEWaypointPropertiesHandler(VisualElement root, WaypointEditorWindow editor)
        {
            _root = root;
            _editor = editor;
            
            _propertiesContainer = _root.Q<VisualElement>(WaypointConstants.PropertiesContainer);
            _idTextField = _root.Q<TextField>(WaypointConstants.IDTextField);
            _positionVector3Field = _root.Q<Vector3Field>(WaypointConstants.PositionVector3Field);
            _radiusFloatField = _root.Q<FloatField>(WaypointConstants.RadiusFloatField);
            _heightFloatField = _root.Q<FloatField>(WaypointConstants.HeightFloatField);

            _connectionsContainer = _root.Q<VisualElement>(WaypointConstants.ConnectionsContainer);
            _connectionsListView = _root.Q<ListView>(WaypointConstants.ConnectionListView);            
            _probabilityFloatField = _root.Q<FloatField>(WaypointConstants.ProbabilityFloatField);


            _idTextField.SetEnabled(false);
            _positionVector3XField = _positionVector3Field.Q<FloatField>("unity-x-input");
            _positionVector3YField = _positionVector3Field.Q<FloatField>("unity-y-input");
            _positionVector3ZField = _positionVector3Field.Q<FloatField>("unity-z-input");
            
            //Disabled the Y input
            _positionVector3YField.SetEnabled(false);

            _positionVector3XField.RegisterValueChangedCallback(OnPositionXValueChanged);
            _positionVector3ZField.RegisterValueChangedCallback(OnPositionZValueChanged);
            _radiusFloatField.RegisterValueChangedCallback(OnRadiusValueChanged);
            _heightFloatField.RegisterValueChangedCallback(OnHeightValueChanged);

            _connectionsListView.makeItem += OnMakeItemListView;

            _connectionsListView.bindItem += OnBindItemListView;
            
            _connectionsListView.onSelectionChange += OnListViewSelectionChanged;

            SetContainerEnabledStatus(false);

            editor.WaypointHandler.SelectionChanged += OnWaypointSelectionChanged;
            editor.WaypointHandler.SelectionValuesChanged += OnSelectionValuesChanged;
        }

        

        public void UpdateSceneData(SerializedObject serializedSceneData)
        {
            _serializedSceneData = serializedSceneData;
        }

        private void SetContainerEnabledStatus(bool status)
        {
            if(status == false)
            {
                _idTextField.value = "";
                _positionVector3Field.SetValueWithoutNotify(Vector3.zero);
                _radiusFloatField.SetValueWithoutNotify(0);
                _heightFloatField.SetValueWithoutNotify(0);

                _connectionsListView.Unbind();
                _connectionsListView.itemsSource = new List<object>();

                _probabilityFloatField.Unbind();
            }

            _propertiesContainer.SetEnabled(status);
            _connectionsContainer.SetEnabled(status);
        }
    
        private void SetPropertiesData()
        {
            //No select
            if (_currentSelectedWaypoints.Count == 0)
            {
                ResetPropertiesPanel();
            }

            //Single Select
            if (_currentSelectedWaypoints.Count == 1)
            {
                DisabledAllMixedValues();
                Waypoint waypoint = _currentSelectedWaypoints[0];
                _idTextField.value = waypoint.ID.ToString();
                SetVector3Field(waypoint.Position);
                _radiusFloatField.SetValueWithoutNotify(waypoint.Radius);
                _heightFloatField.SetValueWithoutNotify(waypoint.Height);

                //Set the list view only if we have 1 waypoint selected
                _currentSerializedConnections = GetSerializedConnection(waypoint.ID);

                if (_currentSerializedConnections != null)
                {
                    //Set first connection as the selected
                    _connectionsListView.RegisterCallback<GeometryChangedEvent>((x) =>
                    {
                        _connectionsListView.SetSelection(0);
                    });
                    _currentSerializedTransitions = _currentSerializedConnections.FindPropertyRelative(WaypointConstants.WaypointTransitionBinding);
                    _connectionsListView.BindProperty(_currentSerializedTransitions);
                }

                _probabilityFloatField.SetValueWithoutNotify(0);
                _connectionsContainer.SetEnabled(true);
            }

            //Multi Select
            if (_currentSelectedWaypoints.Count > 1)
            {
                _idTextField.showMixedValue = true;

                Waypoint startWaypoint = _currentSelectedWaypoints[0];

                CheckPositionMulti(startWaypoint);
                CheckRadiusMulti(startWaypoint);
                CheckHeightMulti(startWaypoint);

                //This clears the listview
                _connectionsListView.Unbind();
                _connectionsListView.itemsSource = new List<object>();

                _probabilityFloatField.Unbind();
                _connectionsContainer.SetEnabled(false);
            }
            
        }

        private void CheckPositionMulti(Waypoint comparisonWaypoint)
        {
            bool mixedValue = false;
            bool xMixedValue = false;
            bool yMixedValue = false;
            bool zMixedValue = false;
            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                if (EqualityComparer<Vector3>.Default.Equals(waypoint.Position, comparisonWaypoint.Position) == false)
                {
                    mixedValue = true;

                    if (EqualityComparer<float>.Default.Equals(waypoint.Position.x, comparisonWaypoint.Position.x) == false)
                    {
                        xMixedValue = true;
                    }

                    if (EqualityComparer<float>.Default.Equals(waypoint.Position.y, comparisonWaypoint.Position.y) == false)
                    {
                        yMixedValue = true;
                    }

                    if (EqualityComparer<float>.Default.Equals(waypoint.Position.z, comparisonWaypoint.Position.z) == false)
                    {
                        zMixedValue = true;
                    }
                }

                if (xMixedValue == true && yMixedValue == true && zMixedValue == true)
                {
                    break;
                }

            }


            if (mixedValue == true)
            {
                _positionVector3XField.showMixedValue = xMixedValue;
                _positionVector3YField.showMixedValue = yMixedValue;
                _positionVector3ZField.showMixedValue = zMixedValue;
            }
            else
            {
                _positionVector3XField.showMixedValue = false;
                _positionVector3YField.showMixedValue = false;
                _positionVector3ZField.showMixedValue = false;
            }
              
        }

        private void CheckRadiusMulti(Waypoint comparisonWaypoint)
        {
            bool mixedValue = false;
            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                if (EqualityComparer<float>.Default.Equals(waypoint.Radius, comparisonWaypoint.Radius) == false)
                {
                    mixedValue = true;
                    break;
                }
            }
            
            _radiusFloatField.SetValueWithoutNotify(comparisonWaypoint.Radius);
            _radiusFloatField.showMixedValue = mixedValue;
        }

        private void CheckHeightMulti(Waypoint comparisonWaypoint)
        {
            bool mixedValue = false;
            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                if (EqualityComparer<float>.Default.Equals(waypoint.Height, comparisonWaypoint.Height) == false)
                {
                    mixedValue = true;
                    break;
                }
            }

            _heightFloatField.SetValueWithoutNotify(comparisonWaypoint.Height);
            _heightFloatField.showMixedValue = mixedValue;
        }

        private void SetVector3Field(Vector3 newValue)
        {
            _positionVector3XField.SetValueWithoutNotify(newValue.x);
            _positionVector3YField.SetValueWithoutNotify(newValue.y);
            _positionVector3ZField.SetValueWithoutNotify(newValue.z);
        }

        private void DisabledAllMixedValues()
        {
            _idTextField.showMixedValue = false;
            _positionVector3Field.showMixedValue = false;
            _positionVector3XField.showMixedValue = false;
            _positionVector3YField.showMixedValue = false;
            _positionVector3ZField.showMixedValue = false;
            _radiusFloatField.showMixedValue = false;
            _heightFloatField.showMixedValue = false;
        }

        private SerializedProperty GetSerializedConnection(uint id)
        {
            
            SerializedProperty serializedConnectionList = _editor.GetCurrentSerializedGroup().FindPropertyRelative(WaypointConstants.WaypointConnectionsBinding);

            for (int i = 0; i < serializedConnectionList.arraySize; i++)
            {
                SerializedProperty serializedConnection = serializedConnectionList.GetArrayElementAtIndex(i);

                if (serializedConnection == null || serializedConnection.managedReferenceValue == null) continue;
                
                WaypointConnections connections = (WaypointConnections)serializedConnection.managedReferenceValue;
                if (connections.ID == id)
                {
                    return serializedConnection;
                }
            }
            return null;
        }

        private void ResetPropertiesPanel()
        {
            DisabledAllMixedValues();
            _idTextField.value = "";
            SetVector3Field(Vector3.zero);
            _radiusFloatField.SetValueWithoutNotify(0);
            _heightFloatField.SetValueWithoutNotify(0);

            
            //This clears the listview
            _connectionsListView.Unbind();
            _connectionsListView.itemsSource = new List<object>();

            _probabilityFloatField.Unbind();
        }

        private void OnPositionXValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint X Position");
           
            Waypoint first = _currentSelectedWaypoints[0];
            float newValue = evt.newValue;
            if (_positionVector3XField.showMixedValue == true)
            {
                newValue = first.Position.x;
                _positionVector3XField.SetValueWithoutNotify(newValue);
                _positionVector3XField.showMixedValue = false;
            }

            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                waypoint.Position = new Vector3(newValue, waypoint.Position.y, waypoint.Position.z);
            }

            EditorUtility.SetDirty(_serializedSceneData.targetObject);
            _serializedSceneData.ApplyModifiedProperties();
        }

        private void OnPositionZValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint Z Position");

            Waypoint first = _currentSelectedWaypoints[0];
            float newValue = evt.newValue;
            if (_positionVector3ZField.showMixedValue == true)
            {
                newValue = first.Position.z;
                _positionVector3ZField.SetValueWithoutNotify(newValue);
                _positionVector3ZField.showMixedValue = false;
            }

            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                waypoint.Position = new Vector3(waypoint.Position.x, waypoint.Position.y, newValue);
            }

            EditorUtility.SetDirty(_serializedSceneData.targetObject);
            _serializedSceneData.ApplyModifiedProperties();
        }

        private void OnRadiusValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint Radius");

            Waypoint first = _currentSelectedWaypoints[0];
            if ( _radiusFloatField.showMixedValue == true)
            {
                _radiusFloatField.SetValueWithoutNotify(Mathf.Clamp(first.Radius,0 , Mathf.Infinity));
                _radiusFloatField.showMixedValue = false;
            }

            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                waypoint.Radius = Mathf.Clamp(evt.newValue, 0 , Mathf.Infinity);
            }

            EditorUtility.SetDirty(_serializedSceneData.targetObject);
            _serializedSceneData.ApplyModifiedProperties();
        }

        private void OnHeightValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Height Position");

            Waypoint first = _currentSelectedWaypoints[0];
            if ( _heightFloatField.showMixedValue == true)
            {
                _heightFloatField.SetValueWithoutNotify(first.Height);
                _heightFloatField.showMixedValue = false;
            }

            foreach(Waypoint waypoint in _currentSelectedWaypoints)
            {
                waypoint.Height = evt.newValue;
            }

            EditorUtility.SetDirty(_serializedSceneData.targetObject);
            _serializedSceneData.ApplyModifiedProperties();
        }
        
        private void OnSelectionValuesChanged()
        {
            SetPropertiesData();
        }   

        private void OnWaypointSelectionChanged(List<Waypoint> selection)
        {
            ResetPropertiesPanel();
            _currentSelectedWaypoints = selection;
            SetContainerEnabledStatus(_currentSelectedWaypoints.Count > 0);
            SetPropertiesData();
        }

        private void OnListViewSelectionChanged(IEnumerable<object> enumerable)
        {
            if (_connectionsListView.selectedItem == null) return;

            SerializedProperty currentSerializedTransitions = (SerializedProperty)_connectionsListView.selectedItem;
            _probabilityFloatField.BindProperty(currentSerializedTransitions.FindPropertyRelative(WaypointConstants.ProbabilityBinding));
        }

        private void OnBindItemListView(VisualElement element, int arg2)
        {
            Label label = (Label)element;
            WaypointConnections connections = (WaypointConnections)_currentSerializedConnections.managedReferenceValue;
            WaypointTransition transition = (WaypointTransition)_currentSerializedTransitions.GetArrayElementAtIndex(arg2).managedReferenceValue;
            //This can happen with undo/redo.
            if (transition == null) return;
            label.text = $"{connections.ID} --> {transition.ID}";
        }

        private VisualElement OnMakeItemListView()
        {
            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            return label;
        }
    
    }
}
