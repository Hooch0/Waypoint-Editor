using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    [EditorTool("Waypoint Tool", typeof(WaypointSceneController))]
    public class WaypointTool : EditorTool
    {
        public override GUIContent toolbarIcon => _toolbarIcon;

        private WaypointEditorWindow _window;

        private GUIContent _toolbarIcon;

        [Shortcut("Waypoint Tool", null, KeyCode.W, ShortcutModifiers.Control)]
        private static void ToolShortcut()
        {
            WaypointSceneController controller = FindObjectOfType<WaypointSceneController>();


            //Check if we have a controller, if not make one.
            if (controller == null)
            {
                controller = WaypointUtility.CreateSceenController();
            }

            //Check if we have a scene asset, if not etiher get or make one.
            if (controller.SceneAsset == null)
            {
                controller.SceneAsset = WaypointUtility.GetOrCreateSceneAsset();
            }

            //Check if we have a waypoint editor instance opne, if not open one and set the scene data.
            if (EditorWindow.HasOpenInstances<WaypointEditorWindow>() == true)
            {
                WaypointEditorWindow window = EditorWindow.GetWindow<WaypointEditorWindow>();
                window.SetSceneData(controller.SceneAsset);
            }

            //Check if have a waypoint scene controller selected, if not then
            if (Selection.GetFiltered<WaypointSceneController>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<WaypointTool>();
            }
            else
            {
                Selection.selectionChanged += OnSelectionChanged;
                Selection.SetActiveObjectWithContext(controller.gameObject, controller.gameObject);
            }
        }

        private static void OnSelectionChanged()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            ToolManager.SetActiveTool<WaypointTool>();
        }

        private void OnActiveToolChanged()
        {
            if (ToolManager.IsActiveTool(this) == false && _window != null && _window.EditingToggle == true)
            {
                SceneVisibilityManager.instance.EnableAllPicking();
                _window.DisableEditing();
            }
        }

        private void OnEnable()
        {
            ToolManager.activeToolChanged += OnActiveToolChanged;

            if (_toolbarIcon != null) return;

            _toolbarIcon = new GUIContent(WaypointResourceAsset.Instance.ToolbarIcon);
        }

        private void OnDisable()
        {
            ToolManager.activeToolChanged -= OnActiveToolChanged;
        }

        public override void OnActivated()
        {

            WaypointSceneController controller = FindObjectOfType<WaypointSceneController>();


            _window = WaypointEditorWindow.ShowWindow();

            if (_window.EditingToggle == true) return;

            _window.EnableEditing();
            _window.SetSceneData(controller.SceneAsset);
            Selection.SetActiveObjectWithContext(target, target);
            SceneVisibilityManager.instance.DisableAllPicking();
        }
    }
}