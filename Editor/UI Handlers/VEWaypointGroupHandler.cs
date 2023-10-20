using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    public class VEWaypointGroupHandler
    {
        private VisualElement _root;
        private WaypointEditorWindow _editor;

        private ListView _groupListView;
        private Button _groupButton;
        private TextField _groupTextField;

        

        public VEWaypointGroupHandler(VisualElement root, WaypointEditorWindow editor)
        {
            _root = root;
            _editor = editor;

            _editor.CurrentGroupChanged += OnCurrentGroupChanged;

            _groupListView = _root.Q<ListView>(WaypointConstants.WaypointEditor.GroupListView);
            _groupButton = _root.Q<Button>(WaypointConstants.WaypointEditor.GroupButton);
            _groupTextField = _root.Q<TextField>(WaypointConstants.WaypointEditor.GroupTextField);
            

            _groupButton.clicked += OnGroupButtonClicked;
            _groupTextField.RegisterCallback<KeyDownEvent>(OnGroupTextFieldKeyDown);


            _groupListView.makeItem += OnMakeItemListView;

            _groupListView.bindItem += OnBindItemListView;
            
            _groupListView.onSelectionChange += OnSlotSelectionChanged;
           
            
        }


        public void UpdateSceneData(SerializedObject serializedSceneData)
        {
            if (serializedSceneData == null)
            {
                //This clears the listview
                _groupListView.Unbind();
                _groupListView.itemsSource = new List<object>();
                return;
            }
            _groupListView.BindProperty(_editor.SerializedWaypointGroups);

            _groupListView.RegisterCallback<GeometryChangedEvent>((x) =>
            {
                SetCurrentGroupAsSelectedElement();
            });

        }

        private void MakeNewGroup()
        {
            if (string.IsNullOrEmpty(_groupTextField.value) == true || _editor.GetGroupByName(_groupTextField.value) != null) return;

            WaypointGroup group = new WaypointGroup(_groupTextField.value);

            AddGroup(group);
            _groupTextField.value = "";
        }

        private void AddGroup(WaypointGroup group)
        {
            _editor.AddWaypointGroup(group);
        }

        private void RemoveGroup(WaypointGroup group)
        {
            _editor.RemoveWaypointGroup(group);
        }

        private void OnGroupButtonClicked()
        {
            MakeNewGroup();
        }

        private VisualElement OnMakeItemListView()
        {
            VEWaypointGroupSlot slot = new VEWaypointGroupSlot();
            slot.Deleted += OnSlotDeleted;

            return slot;
        }

        private int FindElementIndex(SerializedProperty groupProp)
        {
            return FindElementIndex((WaypointGroup)groupProp.managedReferenceValue);
        }

        private int FindElementIndex(WaypointGroup groupToFind)
        {
            
            int foundIndex = -1;

            for (int i = 0; i < _editor.SerializedWaypointGroups.arraySize; i++)
            {
                WaypointGroup group = (WaypointGroup)_editor.SerializedWaypointGroups.GetArrayElementAtIndex(i).managedReferenceValue;

                if (group.GroupName == groupToFind.GroupName)
                {
                    foundIndex = i;
                    break;
                }
            }

            return foundIndex;
        }

        private void SetCurrentGroupAsSelectedElement()
        {
            if (_editor.SerializedWaypointGroups == null) return;

            WaypointGroup group = _editor.WaypointHandler.CurrentGroup;
            int index = 0;
            if (group != null)
            {
                index = FindElementIndex(group);
            }

            if (_groupListView.selectedIndex == index) return;

            _groupListView.SetSelection(index);
        }

        private void OnBindItemListView(VisualElement element, int index)
        {
            VEWaypointGroupSlot slot = (VEWaypointGroupSlot)element;
            SerializedProperty group = _editor.SerializedWaypointGroups.GetArrayElementAtIndex(index).FindPropertyRelative(WaypointConstants.WaypointEditor.WaypointGroupNameBinding);
            slot.Unbind();
            slot.Reset();
            slot.BindProperty(group);
        }

        private void OnSlotSelectionChanged(IEnumerable<object> selection)
        {
            if (_groupListView.selectedItem == null) return;

            SerializedProperty groupProp = (SerializedProperty)_groupListView.selectedItem;

            _editor.SetSelectedGroup((WaypointGroup)groupProp.managedReferenceValue);
        }


        private void OnSlotDeleted(VEWaypointGroupSlot slot)
        {
            RemoveGroup(_editor.GetGroupByName(slot.value));
        }

        private void OnGroupTextFieldKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                MakeNewGroup();
            }
        }
    
        private void OnCurrentGroupChanged(WaypointGroup group)
        {
            if (group == null) return;
            int index = FindElementIndex(group);
            if (_groupListView.selectedIndex == index) return;

            _groupListView.SetSelection(index);
        }

    }
}
