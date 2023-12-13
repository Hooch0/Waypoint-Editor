using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;

namespace Hooch.Waypoint.Editor
{
    //TODO: Cleanup how we generate the asset. Maybe adda new button?

    public class WaypointEditorWindow : EditorWindow
    {
        public event Action<WaypointGroup> CurrentGroupChanged;
        public event Action<bool> IsDirtyChanged;
        public SerializedObject SerializedWaypointEditor
        {
            get
            {
                if (_serializedWaypointEditor == null)
                {
                    _serializedWaypointEditor = new SerializedObject(this);
                }

                return _serializedWaypointEditor;
            }
        }
        public SerializedObject SerializedSceneAsset { get; private set; }
        public SerializedProperty SerializedWaypointGroups { get; private set; }

        public WaypointSceneAsset SceneAsset => _sceneAsset;
        public WaypointHandle WaypointHandler { get; private set; }
        public bool AutolinkToggle => _autolinkToggle;
        public bool EditingToggle => _editingToggle;


        private VEWaypointController _veController;
        [SerializeField] private WaypointSceneAsset _sceneAsset;
        private bool _editingToggle;
        [SerializeField] private bool _autolinkToggle;
        [SerializeField] private bool _autoGenerate;

        private bool _isDirty = false;


        private Vector2 _windowSize = new Vector2(600, 800);

        private SceneView _currentView;
        private SerializedObject _serializedWaypointEditor;


        [MenuItem("Tools/Waypoint System/Waypoint Editor", priority = 20)]
        public static WaypointEditorWindow ShowWindow()
        {
            WaypointEditorWindow wnd = GetWindow<WaypointEditorWindow>();
            wnd.titleContent = new GUIContent("Waypoint Editor");
            wnd.minSize = wnd._windowSize;
            return wnd;
        }

        public static void ShowWindow(WaypointSceneAsset controller)
        {
            WaypointEditorWindow wnd = ShowWindow();
            wnd.SetSceneData(controller);

            if (wnd._veController != null && wnd._sceneAsset == null)
            {
                wnd.LoadWaypointSceneAsset();
            }
        }

        private void OnEnable() 
        {
            WaypointHandler = new WaypointHandle(this);
            
            SceneView.duringSceneGui += OnDuringSceneGUI;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            if (ToolManager.activeToolType == typeof(WaypointTool))
            {
                // Try to activate previously used tool
                ToolManager.RestorePreviousPersistentTool();
            }
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (_currentView == null || _editingToggle == false) return;

            _currentView.Repaint();
        }

        public void CreateGUI()
        {
            if (_sceneAsset == null)
            {
                LoadWaypointSceneAsset();
            }
            VisualTreeAsset tree = WaypointResourceAsset.Instance.EditorUI;
            tree.CloneTree(rootVisualElement);
            _veController = new VEWaypointController(rootVisualElement, this);
        }

        public void EnableEditing()
        {   
            _editingToggle = true;
        }

        public void DisableEditing()
        {
            _editingToggle = false;
            WaypointHandler.ClearSelection();
        }

        public void SetSceneData(WaypointSceneAsset controller)
        {
            UpdateSceneDataIntenral(controller);
            _sceneAsset = controller;
            SerializedWaypointEditor.UpdateIfRequiredOrScript();
            SerializedWaypointEditor.ApplyModifiedProperties();
        }

        public void UpdateSceneData()
        {
            UpdateSceneDataIntenral(_sceneAsset);
        }

        public void AddWaypointGroup(WaypointGroup group)
        {
            SerializedWaypointGroups.arraySize++;
            SerializedWaypointGroups.GetArrayElementAtIndex(SerializedWaypointGroups.arraySize-1).managedReferenceValue = group;
            EditorUtility.SetDirty(SerializedSceneAsset.targetObject);
            SerializedSceneAsset.ApplyModifiedProperties();

            SetSelectedGroup((WaypointGroup)SerializedWaypointGroups.GetArrayElementAtIndex(SerializedWaypointGroups.arraySize - 1).managedReferenceValue);
        }

        public void RemoveWaypointGroup(WaypointGroup group)
        {
            int index = -1;

            for (int i = 0; i < SerializedWaypointGroups.arraySize; i++)
            {
                WaypointGroup groupProp = (WaypointGroup)SerializedWaypointGroups.GetArrayElementAtIndex(i).managedReferenceValue;
                
                if (groupProp == group)
                {
                    index = i;
                    break;
                }
            }

            if (IsSelectedGroup((WaypointGroup)SerializedWaypointGroups.GetArrayElementAtIndex(index).managedReferenceValue))
            {
                SetSelectedGroup((WaypointGroup)SerializedWaypointGroups.GetArrayElementAtIndex(index-1).managedReferenceValue);
            }

            WaypointHandler.WaypointGroupCleanup(group);


            SerializedWaypointGroups.DeleteArrayElementAtIndex(index);
            EditorUtility.SetDirty(SerializedSceneAsset.targetObject);
            SerializedSceneAsset.ApplyModifiedProperties();
        }

        public void SetSelectedGroup(WaypointGroup group)
        {
            WaypointHandler.ResetSelectedWaypoints();
            WaypointHandler.SetSelectedGroup(group);
            CurrentGroupChanged?.Invoke(group);
        }

        public bool IsSelectedGroup(WaypointGroup group)
        {
            return WaypointHandler.CurrentGroup == group;
        }

        public void LinkSelectedWaypoints()
        {
            WaypointHandler.LinkSelectedWaypoints();
        }

        public void UnlinkSelectedWaypoints()
        {
            WaypointHandler.UnlinkSelectedWaypoints();
        }

        public WaypointGroup GetGroupByName(string name)
        {
            if (SerializedSceneAsset != null)
            {
                SerializedProperty groupsProp = SerializedSceneAsset.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);

                for (int i = 0; i < groupsProp.arraySize; i++)
                {
                    WaypointGroup group = (WaypointGroup)groupsProp.GetArrayElementAtIndex(i).managedReferenceValue;
                    if (group.GroupName == name)
                    {
                        return group;
                    }
                }
            }
            return null;
        }

        public void GenerateRuntimeMap()
        {
            if (SceneAsset == null) return;
            SceneAsset.Internal_GenerateRuntimeMap();

            _isDirty = false;
            IsDirtyChanged?.Invoke(_isDirty);
        }

        public SerializedProperty GetCurrentSerializedGroup()
        {
            if (SerializedWaypointGroups != null)
            {
                for (int i = 0; i < SerializedWaypointGroups.arraySize; i++)
                {
                    SerializedProperty groupProp = SerializedWaypointGroups.GetArrayElementAtIndex(i);
                    WaypointGroup group = (WaypointGroup)groupProp.managedReferenceValue;

                    if (WaypointHandler.CurrentGroup == group)
                    {
                        
                        return groupProp;
                    }
                }
            }

            return null;
        }

        public void MarkDirty()
        {
            _isDirty = true;
            IsDirtyChanged?.Invoke(_isDirty);

            if (_autoGenerate == true)
            {
                GenerateRuntimeMap();
            }
        }

        private void ResetEditor()
        {
            _editingToggle = false;
            _autolinkToggle = false;
            WaypointHandler.ClearSelection();
            WaypointHandler.SetSelectedGroup(null);
            _sceneAsset = null;
            SerializedSceneAsset = null;
            SerializedWaypointGroups = null;

            SerializedWaypointEditor.UpdateIfRequiredOrScript();
        }

        private void UpdateSceneDataIntenral(WaypointSceneAsset sceneAsset)
        {
            if (WaypointHandler.CurrentGroup != null)
            {
                SetSelectedGroup(null);
            }

            if (sceneAsset != null)
            {
                SerializedSceneAsset = new SerializedObject(sceneAsset);
                WaypointHandler.IDHandler.SetupUniqueID(GetCurrentWaypointGroupList());
                SerializedWaypointGroups = SerializedSceneAsset.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);
                //If default is not available, add it.
                if (SerializedWaypointGroups.arraySize == 0)
                {
                    AddWaypointGroup(new WaypointGroup(WaypointConstants.WaypointEditor.DEFAULT_GROUP_NAME));
                }
                else
                {
                    SetSelectedGroup((WaypointGroup)SerializedWaypointGroups.GetArrayElementAtIndex(0).managedReferenceValue);
                }
            }
            else
            {
                SerializedSceneAsset = null;
                SerializedWaypointGroups = null;
                ResetEditor();
            }
        }

        private List<WaypointGroup> GetCurrentWaypointGroupList()
        {

            List<WaypointGroup> groups = new List<WaypointGroup>();

            if (SerializedSceneAsset != null)
            {
                SerializedProperty groupsProp = SerializedSceneAsset.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);


                for (int i = 0; i < groupsProp.arraySize; i++)
                {
                    groups.Add((WaypointGroup)groupsProp.GetArrayElementAtIndex(i).managedReferenceValue);
                }
            }

            return groups;
        }

        private void LoadWaypointSceneAsset()
        {
            /*WaypointSceneController controller = FindObjectOfType<WaypointSceneController>();

            if (controller != null)
            {
                SetSceneData(controller);
            }*/
        }

        private void OnDuringSceneGUI(SceneView view)
        {
            _currentView = view;
            if (_sceneAsset == null) return;
            WaypointHandler.HandleWaypoints(_editingToggle, AutolinkToggle);

        }

        private void OnUndoRedoPerformed()
        {
            if (_sceneAsset == null) return;
            WaypointHandler.SetSelectedGroup(WaypointHandler.CurrentGroup, true);
            WaypointHandler.IDHandler.SetupUniqueID(GetCurrentWaypointGroupList());
            MarkDirty();
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Single)
            {
                ResetEditor();

                if (_sceneAsset == null)
                {
                    LoadWaypointSceneAsset();
                }
            }
        }
    }
}
