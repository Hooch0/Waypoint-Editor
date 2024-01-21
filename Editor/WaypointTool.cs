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

            if (controller == null)
            {
                controller = WaypointUtility.CreateSceenController();
            }

            if (controller.SceneAsset == null)
            {
                controller.SceneAsset = WaypointUtility.GetOrCreateSceneAsset();
            }

            if (EditorWindow.HasOpenInstances<WaypointEditorWindow>() == true)
            {
                EditorWindow.GetWindow<WaypointEditorWindow>().SetSceneData(controller.SceneAsset);
            }

            if (Selection.GetFiltered<WaypointSceneController>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<WaypointTool>();
            }
            else
            {
                Selection.selectionChanged += OnSelectionChanged;

                Selection.objects = new Object[1] { controller.gameObject };
            }
        }

        private static void OnSelectionChanged()
        {
            Selection.selectionChanged -= Selection.selectionChanged;
            ToolManager.SetActiveTool<WaypointTool>();
        }

        private void OnEnable()
        {
            _toolbarIcon = new GUIContent(WaypointResourceAsset.Instance.ToolbarIcon);
            ToolManager.activeToolChanged += OnActiveToolChanged;
        }


        private void OnDisable()
        {
            ToolManager.activeToolChanged -= OnActiveToolChanged;
        }


        private void OnActiveToolChanged()
        {
            if (ToolManager.IsActiveTool(this) == false && _window != null && _window.EditingToggle == true)
            {
                _window.DisableEditing();
            }
        }

        public override void OnActivated()
        {
            WaypointSceneController controller = FindObjectOfType<WaypointSceneController>();


            _window = WaypointEditorWindow.ShowWindow();
            _window.EnableEditing();
            Selection.SetActiveObjectWithContext(target, target);
            EditorWindow.GetWindow<WaypointEditorWindow>().SetSceneData(controller.SceneAsset);
        }

    }
}