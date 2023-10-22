using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Hooch.Waypoint.Editor
{

    public class VEWaypointOptionsHandler
    {
        private WaypointEditorWindow _editor;
        private VisualElement _root;
        private Toggle _autolinkToggle;
        private Button _linkButton;
        private Button _unlinkButton;
        private TextField _selectedIdsTextField;
        private WaypointHandle _waypointHandleInstance;

        public VEWaypointOptionsHandler(VisualElement root, WaypointEditorWindow editor)
        {
            _root = root;
            _editor = editor;
            _editor.WaypointHandler.SelectionChanged += OnSelectionChanged;


            _autolinkToggle = _root.Q<Toggle>(WaypointConstants.WaypointEditor.AutolinkToggle);
            _linkButton = _root.Q<Button>(WaypointConstants.WaypointEditor.LinkButton);
            _unlinkButton = _root.Q<Button>(WaypointConstants.WaypointEditor.UnlinkButton);
            _selectedIdsTextField = _root.Q<TextField>(WaypointConstants.WaypointEditor.SelectedIDsTextField);

            _linkButton.clicked += OnLinkButtonClicked;
            _unlinkButton.clicked += OnUnlinkButtonClicked;

            SerializedProperty autolinkProp = _editor.SerializedObject.FindProperty(WaypointConstants.WaypointEditor.AutolinkBindingPath);

            _autolinkToggle.BindProperty(autolinkProp);
            

            _selectedIdsTextField.SetEnabled(false);
        }

        private void OnLinkButtonClicked()
        {
            _editor.LinkSelectedWaypoints();
        }

        private void OnUnlinkButtonClicked()
        {
            _editor.UnlinkSelectedWaypoints();
        }

        private void OnSelectionChanged(List<Waypoint> selection)
        {
            string txt = "";
            for (int i = 0; i < selection.Count; i++)
            {
                txt += selection[i].ID.ToString();
                if (i+1 < selection.Count)
                {
                    txt += ", ";
                }
            }

            _selectedIdsTextField.value = txt;
        }
    }
}
