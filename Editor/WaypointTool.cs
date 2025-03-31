using System.CodeDom;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    [EditorTool("Waypoint Tool", typeof(WaypointSceneController))]
    public class WaypointTool : EditorTool
    {
        public override GUIContent toolbarIcon => _toolbarIcon;
        public static WaypointTool Instance { get; private set; }

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
            if (Selection.GetFiltered<WaypointSceneController>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<WaypointTool>();
            }

            WaypointEditorWindow.ShowWindow();

            Selection.SetActiveObjectWithContext(controller.gameObject, controller.gameObject);
        }

        private void OnSelectionChanged()
        {

            //Check if have a waypoint scene controller selected, if not then
            if (Selection.GetFiltered<WaypointSceneController>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<WaypointTool>();
            }
        }

        private void OnActiveToolChanged()
        {
            if (ToolManager.IsActiveTool(this) == false && _window != null && _window.EditingToggle == true)
            {
                _window.DisableEditing();
            }
        }

        private void OnEnable()
        {
            Instance = this;
            ToolManager.activeToolChanged += OnActiveToolChanged;
            Selection.selectionChanged += OnSelectionChanged;

            if (_toolbarIcon != null) return;

            _toolbarIcon = new GUIContent(WaypointResourceAsset.Instance.ToolbarIcon);
        }

        private void OnDisable()
        {
            ToolManager.activeToolChanged -= OnActiveToolChanged;
            Selection.selectionChanged -= OnSelectionChanged;
            _window?.DisableEditing();
        }

        public override void OnActivated()
        {

            WaypointSceneController controller = FindObjectOfType<WaypointSceneController>();

            if (EditorWindow.HasOpenInstances<WaypointEditorWindow>() == true)
            {
                _window = EditorWindow.GetWindow<WaypointEditorWindow>();
            }

            if (_window == null || _window.EditingToggle == true) return;

            _window.EnableEditing();
            _window.SetSceneData(controller.SceneAsset);
            Selection.SetActiveObjectWithContext(target, target);
        }
    }
}