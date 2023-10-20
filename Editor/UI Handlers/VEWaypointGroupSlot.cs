using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{
    
    public class VEWaypointGroupSlot : BindableElement, INotifyValueChanged<string>
    {

        public event Action<VEWaypointGroupSlot> Deleted;

        public virtual string value
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (!EqualityComparer<string>.Default.Equals(_groupName, value) && string.IsNullOrEmpty(value) == false && string.IsNullOrWhiteSpace(value) == false)
                {
                    if (panel != null)
                    {
                        using (ChangeEvent<string> evt = ChangeEvent<string>.GetPooled(_groupName, value))
                        {
                            evt.target = this;
                            SetValueWithoutNotify(value);
                            SendEvent(evt);
                        }
                    }
                    else
                    {
                        SetValueWithoutNotify(value);
                    }
                }
            }
        }

        [SerializeField]
        private string _groupName;

        private VisualElement _groupBackground;
        private Label _label;
        private TextField _textField;
        private ContextualMenuManipulator _contextMenu;


        public VEWaypointGroupSlot()
        {
            name = "WaypointGroupSlot";
            _groupBackground = new VisualElement();
            _groupBackground.name = "GroupBackground";

            _label = new Label();

            _textField = new TextField("");
            _textField.style.display = DisplayStyle.None;
            _textField.isDelayed = true;
            _textField.RegisterCallback<FocusOutEvent>(OnTextFieldLostFocus);

            Add(_groupBackground);
            _groupBackground.Add(_label);
            _groupBackground.Add(_textField);
            _contextMenu = new ContextualMenuManipulator(OnBuildContextMenu);
            Reset();
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if(string.IsNullOrEmpty(_groupName) && newValue == WaypointConstants.WaypointEditor.DEFAULT_GROUP_NAME)
            {
                _textField.UnregisterValueChangedCallback(OnTextFieldChanged);
                _label.UnregisterCallback<MouseDownEvent>(OnLabelMouseDown);
                _groupBackground.tooltip = $"{WaypointConstants.WaypointEditor.DEFAULT_GROUP_NAME} cannot be renamed or deleted.";
                _contextMenu.target = null;
            }
            
            _groupName = newValue;
            _label.text = newValue;
            _textField.value = newValue;
        }

        public void Reset()
        {
            value = "";
            _textField.RegisterValueChangedCallback(OnTextFieldChanged);
            _label.RegisterCallback<MouseDownEvent>(OnLabelMouseDown);
            _groupBackground.tooltip = "";
            _contextMenu.target = this;
            _groupName = "";
        }

        private void OnTextFieldChanged(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue) || string.IsNullOrWhiteSpace(evt.newValue)) return;
            value = evt.newValue;
            
            _textField.style.display = DisplayStyle.None;
            _label.style.display = DisplayStyle.Flex;

        }

        private void OnLabelMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 0 && evt.clickCount == 2)
            {
                _textField.style.display = DisplayStyle.Flex;
                _label.style.display = DisplayStyle.None;
                _textField.Focus();
            }
        }

        private void OnTextFieldLostFocus(FocusOutEvent evt)
        {
            _label.text = _groupName;
            _textField.value = _groupName;

            _textField.style.display = DisplayStyle.None;
            _label.style.display = DisplayStyle.Flex;
        }

        private void OnBuildContextMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", OnDeletePressed);
        }

        private void OnDeletePressed(DropdownMenuAction action)
        {
            Deleted?.Invoke(this);
        }

        
    }
}
