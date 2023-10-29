using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    public class VEWaypointEventPropertiesHandler
    {
        public VisualElement Container { get; private set; }
        private VisualElement _root;
        private Button _addEventButton;
        private ListView _eventListView;
        private VEWaypointPropertiesHandler _propertyHandler;

        private WaypointEventsDropdown _dropdown;

        private SerializedProperty _currentSerializedWaypoint;
        private SerializedProperty _serializedEvents;

        public VEWaypointEventPropertiesHandler(VisualElement root, VEWaypointPropertiesHandler propertiesHandler)
        {
            _root = root;
            _propertyHandler = propertiesHandler;

            Container = root.Q<VisualElement>(WaypointConstants.WaypointEditor.EventsContainer);
            _addEventButton = _root.Q<Button>(WaypointConstants.WaypointEditor.AddEventButton);
            _eventListView = _root.Q<ListView>(WaypointConstants.WaypointEditor.EventListView);

            _addEventButton.clicked += OnAddEventButtonClicked;

            _eventListView.makeItem += OnMakeItemListView;

            _eventListView.bindItem += OnBindItemListView;

            _dropdown = new WaypointEventsDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState());
            _dropdown.ItemPicked += OnItemPicked;
        }

        public void Bind(SerializedProperty serializedProperty)
        {
            _currentSerializedWaypoint = serializedProperty;
            _serializedEvents = _currentSerializedWaypoint.FindPropertyRelative(WaypointConstants.WaypointEditor.WaypointEventsBinding);

            List<int> indexesToRemove = new List<int>();
            for (int i = 0; i < _serializedEvents.arraySize; i++)
            {
                SerializedProperty prop = _serializedEvents.GetArrayElementAtIndex(i);

                if (prop.managedReferenceValue == null)
                {
                    indexesToRemove.Add(i);
                }
            }

            if (indexesToRemove.Count > 0)
            {
                foreach (int index in indexesToRemove)
                {
                    _serializedEvents.DeleteArrayElementAtIndex(index);
                }

                _propertyHandler.SetChangesDirty();
            }

            _eventListView.BindProperty(_serializedEvents);
        }

        public void Unbind()
        {
            _eventListView.Unbind();
            _eventListView.itemsSource = new List<object>(); ;
        }

        private void OnAddEventButtonClicked()
        {
            _dropdown.Show(new Rect(_addEventButton.worldBound.position, _addEventButton.worldBound.size));
        }

        private void OnItemPicked(WaypointEvent evt)
        {
            _propertyHandler.CurrentSelectedWaypoints[0].AddEvent(evt);
            _propertyHandler.SetChangesDirty();
        }

        private VisualElement OnMakeItemListView()
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            VisualElement indent = new VisualElement();
            indent.style.width = 30;

            PropertyField propertyField = new PropertyField();
            propertyField.style.flexGrow = 1.0f;


            VisualElement middle = new VisualElement();
            middle.style.flexGrow = 1;

            Button deleteButton = new Button();
            deleteButton.styleSheets.Add(WaypointResourceAsset.Instance.EventDeleteButtonStyle);
            deleteButton.AddToClassList("delete-button");
            deleteButton.text = "X";

            deleteButton.clickable.clickedWithEventInfo += OnDeleteClicked;


            container.Add(indent);
            container.Add(propertyField);
            container.Add(middle);
            container.Add(deleteButton);

            return container;
        }

        private void OnBindItemListView(VisualElement element, int index)
        {
            //Setup property field
            PropertyField propertyField = element.Q<PropertyField>();
            SerializedProperty serilizedEvent = _serializedEvents.GetArrayElementAtIndex(index);
            propertyField.BindProperty(serilizedEvent);


            Label label = propertyField.Q<Label>();
            WaypointEvent evt = (WaypointEvent)serilizedEvent.managedReferenceValue;
            label.RegisterValueChangedCallback(OnPropertyFieldLabelValueChanged);
            label.userData = index;
            label.text = evt.GetType().Name;

            //Setup button
            Button deleteButton = element.Q<Button>();
            deleteButton.userData = index;
        }

        private void OnDeleteClicked(EventBase evt)
        {
            int index = (int)((Button)evt.currentTarget).userData;
            _serializedEvents.DeleteArrayElementAtIndex(index);

            _propertyHandler.SetChangesDirty();
        }

        private void OnPropertyFieldLabelValueChanged(ChangeEvent<string> evt)
        {
            Label label = (Label)evt.currentTarget;

            SerializedProperty serilizedEvent = _serializedEvents.GetArrayElementAtIndex((int)label.userData);
            WaypointEvent wevt = (WaypointEvent)serilizedEvent.managedReferenceValue;

            string name = wevt.GetType().Name;

            if (label.text == name) return;
            label.text = name;
        }

    }
}
