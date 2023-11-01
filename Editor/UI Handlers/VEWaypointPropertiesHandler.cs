using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{

    public class VEWaypointPropertiesHandler
    {
        public IReadOnlyList<Waypoint> CurrentSelectedWaypoints { get; private set; }


        private VisualElement _root;

        private VisualElement _propertiesContainer;
        private TextField _idTextField;
        private Vector3Field _positionVector3Field;
        private FloatField _positionVector3XField;
        private FloatField _positionVector3YField;
        private FloatField _positionVector3ZField;
        private FloatField _radiusFloatField;
        private FloatField _heightFloatField;
        private TextField _tagTextField;

        private VisualElement _connectionsContainer;
        private Label _transitionLogicLabel;
        private PropertyField _transitionLogicPropertyField;
        private Button _setTransitionLogicButton;
        private ListView _connectionsListView;
        private IntegerField _weightIntField;

        private SerializedObject _serializedSceneData;
        private SerializedProperty _currentSerializedConnections;
        private SerializedProperty _currentSerializedTransitions;

        private WaypointEditorWindow _editor;

        private VEWaypointEventPropertiesHandler _eventProperties;

        private WaypointTypeDropdown<WaypointTransitionLogic> _dropdown;

        private const string SetTransitionButtonLabel = "Set Transition Logic";
        private const string UnsetTransitionButtonLabel = "Unset";

        public VEWaypointPropertiesHandler(VisualElement root, WaypointEditorWindow editor)
        {
            _root = root;
            _editor = editor;

            _eventProperties = new VEWaypointEventPropertiesHandler(root, this);

            _propertiesContainer = _root.Q<VisualElement>(WaypointConstants.WaypointEditor.PropertiesContainer);
            _idTextField = _root.Q<TextField>(WaypointConstants.WaypointEditor.IDTextField);
            _positionVector3Field = _root.Q<Vector3Field>(WaypointConstants.WaypointEditor.PositionVector3Field);
            _radiusFloatField = _root.Q<FloatField>(WaypointConstants.WaypointEditor.RadiusFloatField);
            _heightFloatField = _root.Q<FloatField>(WaypointConstants.WaypointEditor.HeightFloatField);
            _tagTextField = _root.Q<TextField>(WaypointConstants.WaypointEditor.TagTextField);

            _connectionsContainer = _root.Q<VisualElement>(WaypointConstants.WaypointEditor.ConnectionsContainer);
            _transitionLogicLabel = _root.Q<Label>(WaypointConstants.WaypointEditor.TransitionLogicLabel);
            _transitionLogicPropertyField = _root.Q<PropertyField>(WaypointConstants.WaypointEditor.TransitionLogicPropertyField);
            _setTransitionLogicButton = _root.Q<Button>(WaypointConstants.WaypointEditor.SetTransitionLogicButton);
            _connectionsListView = _root.Q<ListView>(WaypointConstants.WaypointEditor.ConnectionListView);
            _weightIntField = _root.Q<IntegerField>(WaypointConstants.WaypointEditor.WeightIntField);


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
            _tagTextField.RegisterValueChangedCallback(OnTagValueChanged);
            _weightIntField.RegisterValueChangedCallback(OnWeightValueChanged);

            _setTransitionLogicButton.text = SetTransitionButtonLabel;

            _setTransitionLogicButton.clicked += OnSetTransitionButtonClicked;


            _connectionsListView.makeItem += OnMakeItemListView;

            _connectionsListView.bindItem += OnBindItemListView;

            _connectionsListView.onSelectionChange += OnListViewSelectionChanged;

            SetContainerEnabledStatus(false);

            editor.WaypointHandler.SelectionChanged += OnWaypointSelectionChanged;
            editor.WaypointHandler.SelectionValuesChanged += OnSelectionValuesChanged;

            _dropdown = new WaypointTypeDropdown<WaypointTransitionLogic>("Transition Logic Overrides", new UnityEditor.IMGUI.Controls.AdvancedDropdownState());
            _dropdown.ItemPicked += OnItemPicked;

        }


        public void UpdateSceneData(SerializedObject serializedSceneData)
        {
            _serializedSceneData = serializedSceneData;
        }

        public void SetChangesDirty()
        {
            EditorUtility.SetDirty(_serializedSceneData.targetObject);
            _serializedSceneData.ApplyModifiedProperties();
        }

        private void SetContainerEnabledStatus(bool status)
        {
            if (status == false)
            {
                _idTextField.value = "";
                _positionVector3Field.SetValueWithoutNotify(Vector3.zero);
                _radiusFloatField.SetValueWithoutNotify(0);
                _heightFloatField.SetValueWithoutNotify(0);
                _tagTextField.SetValueWithoutNotify("");

                _connectionsListView.Unbind();
                _connectionsListView.itemsSource = new List<object>();

                _weightIntField.Unbind();
                _eventProperties.Unbind();
            }

            _propertiesContainer.SetEnabled(status);
            _connectionsContainer.SetEnabled(status);
        }

        private void SetPropertiesData()
        {
            //No select
            if (CurrentSelectedWaypoints.Count == 0)
            {
                ResetPropertiesPanel();
            }

            //Single Select
            if (CurrentSelectedWaypoints.Count == 1)
            {
                _eventProperties.Container.SetEnabled(true);
                _eventProperties.Unbind();
                DisabledAllMixedValues();
                Waypoint waypoint = CurrentSelectedWaypoints[0];
                _idTextField.value = waypoint.ID.ToString();
                SetVector3Field(waypoint.Position);
                _radiusFloatField.SetValueWithoutNotify(waypoint.Radius);
                _heightFloatField.SetValueWithoutNotify(waypoint.Height);
                _tagTextField.SetValueWithoutNotify(waypoint.Tag);

                //Set the list view only if we have 1 waypoint selected
                _currentSerializedConnections = GetSerializedConnection(waypoint.ID);
                SerializedProperty currentSerializedWaypoint = GetSerializedWaypoint(waypoint);


                if (_currentSerializedConnections != null)
                {


                    //Set first connection as the selected
                    _connectionsListView.RegisterCallback<GeometryChangedEvent>((x) =>
                    {
                        _connectionsListView.SetSelection(0);
                    });
                    _currentSerializedTransitions = _currentSerializedConnections.FindPropertyRelative(WaypointConstants.WaypointEditor.WaypointTransitionBinding);
                    _connectionsListView.BindProperty(_currentSerializedTransitions);

                    WaypointConnections connections = (WaypointConnections)_currentSerializedConnections.managedReferenceValue;
                    if (connections.TransitionLogic != null)
                    {
                        _setTransitionLogicButton.text = $"{UnsetTransitionButtonLabel} {ObjectNames.NicifyVariableName(connections.TransitionLogic.GetType().Name)}";
                        _transitionLogicLabel.style.display = DisplayStyle.None;
                        _transitionLogicPropertyField.style.display = DisplayStyle.Flex;
                        _transitionLogicPropertyField.BindProperty(_currentSerializedConnections.FindPropertyRelative(WaypointConstants.WaypointEditor.TransitionLogicBinding));
                    }
                    else
                    {
                        _setTransitionLogicButton.text = SetTransitionButtonLabel;
                        _transitionLogicLabel.style.display = DisplayStyle.Flex;
                        _transitionLogicPropertyField.style.display = DisplayStyle.None;
                    }
                }

                if (currentSerializedWaypoint != null)
                {
                    _eventProperties.Bind(currentSerializedWaypoint);
                }

                _weightIntField.SetValueWithoutNotify(0);
                _connectionsContainer.SetEnabled(true);
            }

            //Multi Select
            if (CurrentSelectedWaypoints.Count > 1)
            {
                _eventProperties.Container.SetEnabled(false);
                _idTextField.showMixedValue = true;

                Waypoint startWaypoint = CurrentSelectedWaypoints[0];

                CheckPositionMulti(startWaypoint);
                CheckRadiusMulti(startWaypoint);
                CheckHeightMulti(startWaypoint);
                CheckTagMulti(startWaypoint);

                ResetSingleSelectProperties();
            }

        }

        private void CheckPositionMulti(Waypoint comparisonWaypoint)
        {
            bool mixedValue = false;
            bool xMixedValue = false;
            bool yMixedValue = false;
            bool zMixedValue = false;
            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
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
            CheckMixedValue(comparisonWaypoint.Radius, x => x.Radius, _radiusFloatField);
        }

        private void CheckHeightMulti(Waypoint comparisonWaypoint)
        {
            CheckMixedValue(comparisonWaypoint.Height, x => x.Height, _heightFloatField);
        }

        private void CheckTagMulti(Waypoint comparisonWaypoint)
        {
            CheckMixedValue(comparisonWaypoint.Tag, x => x.Tag, _tagTextField);
        }

        private void CheckMixedValue<T>(T checkvalue, Func<Waypoint, T> comparison, BaseField<T> valueField)
        {
            bool mixedValue = false;
            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
            {
                if (EqualityComparer<T>.Default.Equals(comparison.Invoke(waypoint), checkvalue) == false)
                {
                    mixedValue = true;
                    break;
                }
            }

            valueField.SetValueWithoutNotify(checkvalue);
            valueField.showMixedValue = mixedValue;
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
            _tagTextField.showMixedValue = false;
        }

        private SerializedProperty GetSerializedConnection(uint id)
        {

            SerializedProperty serializedConnectionList = _editor.GetCurrentSerializedGroup().FindPropertyRelative(WaypointConstants.WaypointEditor.WaypointGroupConnectionsBinding);

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

        private SerializedProperty GetSerializedWaypoint(Waypoint waypoint)
        {
            SerializedProperty serializedWaypointGroup = _editor.GetCurrentSerializedGroup();
            WaypointGroup group = (WaypointGroup)serializedWaypointGroup.managedReferenceValue;

            int index = group.Waypoints.IndexOf(waypoint);

            SerializedProperty serializedWaypoints = serializedWaypointGroup.FindPropertyRelative(WaypointConstants.WaypointEditor.WaypointGroupWaypointListBinding);
            if (serializedWaypoints.arraySize - 1 < index) return null;

            return serializedWaypoints.GetArrayElementAtIndex(index);
        }

        private void ResetPropertiesPanel()
        {
            DisabledAllMixedValues();
            _idTextField.value = "";
            SetVector3Field(Vector3.zero);
            _radiusFloatField.SetValueWithoutNotify(0);
            _heightFloatField.SetValueWithoutNotify(0);
            _tagTextField.SetValueWithoutNotify("");

            ResetSingleSelectProperties();
        }

        private void ResetSingleSelectProperties()
        {
            _setTransitionLogicButton.Unbind();

            _transitionLogicLabel.style.display = DisplayStyle.Flex;
            _transitionLogicPropertyField.style.display = DisplayStyle.None;

            //This clears the listview
            _connectionsListView.Unbind();
            _connectionsListView.itemsSource = new List<object>();

            _eventProperties.Unbind();

            _weightIntField.Unbind();
            _connectionsContainer.SetEnabled(false);
        }

        private void OnPositionXValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint X Position");

            Waypoint first = CurrentSelectedWaypoints[0];
            float newValue = evt.newValue;
            if (_positionVector3XField.showMixedValue == true)
            {
                newValue = first.Position.x;
                _positionVector3XField.SetValueWithoutNotify(newValue);
                _positionVector3XField.showMixedValue = false;
            }

            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
            {
                waypoint.Position = new Vector3(newValue, waypoint.Position.y, waypoint.Position.z);
            }

            SetChangesDirty();
        }

        private void OnPositionZValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint Z Position");

            Waypoint first = CurrentSelectedWaypoints[0];
            float newValue = evt.newValue;
            if (_positionVector3ZField.showMixedValue == true)
            {
                newValue = first.Position.z;
                _positionVector3ZField.SetValueWithoutNotify(newValue);
                _positionVector3ZField.showMixedValue = false;
            }

            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
            {
                waypoint.Position = new Vector3(waypoint.Position.x, waypoint.Position.y, newValue);
            }

            SetChangesDirty();
        }

        private void OnRadiusValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint Radius");

            Waypoint first = CurrentSelectedWaypoints[0];
            if (_radiusFloatField.showMixedValue == true)
            {
                _radiusFloatField.SetValueWithoutNotify(Mathf.Clamp(first.Radius, 0, Mathf.Infinity));
                _radiusFloatField.showMixedValue = false;
            }

            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
            {
                waypoint.Radius = Mathf.Clamp(evt.newValue, 0, Mathf.Infinity);
            }

            SetChangesDirty();
        }

        private void OnHeightValueChanged(ChangeEvent<float> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Height Position");

            Waypoint first = CurrentSelectedWaypoints[0];
            if (_heightFloatField.showMixedValue == true)
            {
                _heightFloatField.SetValueWithoutNotify(first.Height);
                _heightFloatField.showMixedValue = false;
            }

            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
            {
                waypoint.Height = evt.newValue;
            }

            SetChangesDirty();
        }

        private void OnTagValueChanged(ChangeEvent<string> evt)
        {
            Undo.RegisterCompleteObjectUndo(_serializedSceneData.targetObject, "Changed Waypoint Tag");

            Waypoint first = CurrentSelectedWaypoints[0];
            if (_tagTextField.showMixedValue == true)
            {
                _tagTextField.SetValueWithoutNotify(first.Tag);
                _tagTextField.showMixedValue = false;
            }

            foreach (Waypoint waypoint in CurrentSelectedWaypoints)
            {
                waypoint.Tag = evt.newValue;
            }

            SetChangesDirty();
        }

        private void OnWeightValueChanged(ChangeEvent<int> evt)
        {
            if (evt.newValue < 0)
            {
                _weightIntField.SetValueWithoutNotify(0);
            }
        }

        private void OnSelectionValuesChanged()
        {
            SetPropertiesData();
        }

        private void OnWaypointSelectionChanged(List<Waypoint> selection)
        {
            ResetPropertiesPanel();
            CurrentSelectedWaypoints = selection;
            SetContainerEnabledStatus(CurrentSelectedWaypoints.Count > 0);
            SetPropertiesData();
        }

        private void OnListViewSelectionChanged(IEnumerable<object> enumerable)
        {
            if (_connectionsListView.selectedItem == null) return;

            SerializedProperty currentSerializedTransitions = (SerializedProperty)_connectionsListView.selectedItem;
            _weightIntField.BindProperty(currentSerializedTransitions.FindPropertyRelative(WaypointConstants.WaypointEditor.WeightBinding));
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


        private void OnSetTransitionButtonClicked()
        {
            WaypointConnections connections = (WaypointConnections)_currentSerializedConnections.managedReferenceValue;

            if (connections.TransitionLogic == null)
            {
                _dropdown.Show(new Rect(_setTransitionLogicButton.worldBound.position, _setTransitionLogicButton.worldBound.size));
            }
            else
            {
                connections.SetTransitionLogic(null);

                _transitionLogicLabel.style.display = DisplayStyle.Flex;
                _transitionLogicPropertyField.style.display = DisplayStyle.None;

                _setTransitionLogicButton.text = SetTransitionButtonLabel;
            }

            SetChangesDirty();
        }

        private void OnItemPicked(WaypointTransitionLogic logic)
        {
            WaypointConnections connections = (WaypointConnections)_currentSerializedConnections.managedReferenceValue;

            connections.SetTransitionLogic(logic);

            _transitionLogicLabel.style.display = DisplayStyle.None;
            _transitionLogicPropertyField.style.display = DisplayStyle.Flex;

            _setTransitionLogicButton.text = $"{UnsetTransitionButtonLabel} {ObjectNames.NicifyVariableName(logic.GetType().Name)}";
        }

    }
}
