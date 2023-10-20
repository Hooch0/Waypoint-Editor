using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Hooch.Waypoint.Editor
{
    public class WaypointEditorWindow : EditorWindow
    {
        public event Action<WaypointGroup> CurrentGroupChanged;

        public SerializedObject SerializedObject { get; private set; }
        public SerializedObject SerializedSceneController { get; private set;}
        public SerializedProperty SerializedWaypointGroups { get; private set; }

        public WaypointSceneController SceneController => _sceneController;
        public WaypointHandle WaypointHandler { get; private set; }
        public bool EditingToggle => _editingToggle;
        public bool AutolinkToggle => _autolinkToggle;


        private VEWaypointController _veController;
        [SerializeField] private WaypointSceneController _sceneController;
        [SerializeField] private bool _editingToggle;
        [SerializeField] private bool _autolinkToggle;
        [SerializeField] private bool _autoGenerate;
        
        private Vector2 _windowSize = new Vector2(600, 800);
        private int? _generationProgressID;
        private int _id;

        private SceneView _currentView;
        private const string _EDITOR_UI_DOCUMENT = "UI Toolkit/Documents/WaypointControllerEditor";



        [MenuItem("Tools/Waypoint System/Editor", priority = 20)]
        public static WaypointEditorWindow ShowWindow()
        {
            WaypointEditorWindow wnd = GetWindow<WaypointEditorWindow>();
            wnd.titleContent = new GUIContent("Waypoint Editor");
            wnd.minSize = wnd._windowSize;
            return wnd;
        }

        public static void ShowWindow(WaypointSceneController controller)
        {
            WaypointEditorWindow wnd = ShowWindow();
            wnd.SetSceneData(controller);

            if (wnd._veController != null && wnd._sceneController == null)
            {
                wnd.LoadWaypointSceneController();
            }
        }

        private void OnEnable() 
        {
            WaypointHandler = new WaypointHandle(this);
            
            SceneView.duringSceneGui += OnDuringSceneGUI;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update += Update;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.update -= Update;
        }



        private void Update()
        {
            if (_currentView == null || EditingToggle == false) return;

            _currentView.Repaint();
            /*if (_autoGenerate == false || WaypointHandler.IsDirty == false) return;

            WaypointHandler.SetDirty(false);
            GenerateRuntimeMap();*/
        }

        public void CreateGUI()
        {
            SerializedObject = new SerializedObject(this);
            if (_sceneController == null)
            {
                LoadWaypointSceneController();
            }
            VisualTreeAsset tree = Resources.Load(_EDITOR_UI_DOCUMENT) as VisualTreeAsset;
            tree.CloneTree(rootVisualElement);
            _veController = new VEWaypointController(rootVisualElement, this);
        }

        public void SetSceneData(WaypointSceneController controller)
        {
            UpdateSceneDataIntenral(controller);
            _sceneController = controller;
            SerializedObject.UpdateIfRequiredOrScript();
        }

        public void UpdateSceneData()
        {
            UpdateSceneDataIntenral(_sceneController);
        }

        public void AddWaypointGroup(WaypointGroup group)
        {
            SerializedWaypointGroups.arraySize++;
            SerializedWaypointGroups.GetArrayElementAtIndex(SerializedWaypointGroups.arraySize-1).managedReferenceValue = group;
            EditorUtility.SetDirty(SerializedSceneController.targetObject);
            SerializedSceneController.ApplyModifiedProperties();

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
            EditorUtility.SetDirty(SerializedSceneController.targetObject);
            SerializedSceneController.ApplyModifiedProperties();
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
            if (SerializedSceneController != null)
            {
                SerializedProperty groupsProp = SerializedSceneController.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);

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
            if(_generationProgressID != null)
            {
                Progress.Cancel(_generationProgressID.Value);
            }

            //_sceneController.StartCoroutine(RunGenerateRuntimeMap());
        }

        public void CreateSceenData()
        {
            GameObject go = new GameObject("Waypoint Controller", typeof(WaypointSceneController));

            WaypointSceneController sceneData = go.GetComponent<WaypointSceneController>();
            SetSceneData(sceneData);
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

        private void ResetEditor()
        {
            _editingToggle = false;
            _autolinkToggle = false;
            WaypointHandler.ClearSelection();
            WaypointHandler.SetSelectedGroup(null);
            _sceneController = null;
            SerializedSceneController = null;
            SerializedWaypointGroups = null;

            if (SerializedObject != null && SerializedObject.targetObject != null)
            {
                SerializedObject.UpdateIfRequiredOrScript();
            }
        }

        private void UpdateSceneDataIntenral(WaypointSceneController sceneController)
        {
            if (WaypointHandler.CurrentGroup != null)
            {
                SetSelectedGroup(null);
            }
            
            if (sceneController != null)
            {
                SerializedSceneController = new SerializedObject(sceneController);
                WaypointHandler.IDHandler.SetupUniqueID(GetCurrentWaypointGroupList());
                SerializedWaypointGroups = SerializedSceneController.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);
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
                SerializedSceneController = null;
                SerializedWaypointGroups = null;
                ResetEditor();
            }
        }

        private List<WaypointGroup> GetCurrentWaypointGroupList()
        {

            List<WaypointGroup> groups = new List<WaypointGroup>();

            if (SerializedSceneController != null)
            {
                SerializedProperty groupsProp = SerializedSceneController.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);


                for (int i = 0; i < groupsProp.arraySize; i++)
                {
                    groups.Add((WaypointGroup)groupsProp.GetArrayElementAtIndex(i).managedReferenceValue);
                }
            }

            return groups;
        }

        private void LoadWaypointSceneController()
        {
            WaypointSceneController controller = FindObjectOfType<WaypointSceneController>();

            if (controller != null)
            {
                SetSceneData(controller);
            }
        }

        private void OnDuringSceneGUI(SceneView view)
        {
            _currentView = view;
            if (_sceneController == null) return;
            WaypointHandler.HandleWaypoints(EditingToggle, AutolinkToggle);

            if (EditingToggle == false) return;

            Selection.objects = null;
            UnityEditor.Tools.current = Tool.Custom;
        }

        private void OnUndoRedoPerformed()
        {
            if (_sceneController == null) return;
            WaypointHandler.SetSelectedGroup(WaypointHandler.CurrentGroup, true);
            WaypointHandler.IDHandler.SetupUniqueID(GetCurrentWaypointGroupList());
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Single)
            {
                ResetEditor();

                if (_sceneController == null)
                {
                    LoadWaypointSceneController();
                }
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                _id = SceneController.gameObject.GetInstanceID();
                _sceneController = null;
            }

            if (change == PlayModeStateChange.EnteredEditMode && _sceneController == null)
            {
                WaypointSceneController[] controllers = FindObjectsOfType<WaypointSceneController>();

                foreach(WaypointSceneController controller in controllers)
                {
                    if (controller.gameObject.GetInstanceID() == _id)
                    {
                        SetSceneData(controller);
                        break;
                    }
                }
            }
        }
    
        /*
        Disbaled until a suitable replacement can be found for generating dicitonary/hashmaps in editor without losing performance.
        private System.Collections.IEnumerator RunGenerateRuntimeMap()
        {
            SerializedProperty groupsProp = SerializedSceneController.FindProperty(WaypointConstants.WaypointEditor.WaypointGroupsBinding);

            int groupsCnt = groupsProp.arraySize;
            int waypointsCnt = 0;
            int connectionsCnt = 0;


            for (int i = 0; i < groupsCnt; i++)
            {
                WaypointGroup group = (WaypointGroup)groupsProp.GetArrayElementAtIndex(i).managedReferenceValue;

                waypointsCnt += group.Waypoints.Count;
                connectionsCnt += group.Connections.Count;
            }


            int total = groupsCnt + waypointsCnt + connectionsCnt;
            int current = 0;

            WaypointSerializableDictionary runtimeWaypointMap = new WaypointSerializableDictionary();
            WaypointConnectionSerializableDictionary runtimeConnectionMap = new WaypointConnectionSerializableDictionary();
            _generationProgressID = UnityEditor.Progress.Start("Generating Runtime Map");
            for (int i = 0; i <  groupsProp.arraySize; i++)
            {
                WaypointGroup group = (WaypointGroup)groupsProp.GetArrayElementAtIndex(i).managedReferenceValue;;
                foreach (Waypoint waypoint in group.Waypoints)
                {
                    runtimeWaypointMap.Add(waypoint.ID, waypoint);
                    UnityEditor.Progress.Report(_generationProgressID.Value, ++current / total);
                }

                foreach (WaypointConnections connections in group.Connections)
                {
                    runtimeConnectionMap.Add(connections.ID, connections);
                    UnityEditor.Progress.Report(_generationProgressID.Value, ++current / total);
                }
                UnityEditor.Progress.Report(_generationProgressID.Value, ++current / total, $"Finished Generating ({i + 1}/{groupsCnt})");
                yield return new WaitForSeconds(0.1f);
            }
            UnityEditor.Progress.Report(_generationProgressID.Value, 1.0f, "Finished");
            UnityEditor.Progress.Remove(_generationProgressID.Value);

            //_sceneController.SetRuntimeMaps(runtimeWaypointMap, runtimeConnectionMap);
            _generationProgressID = null;
        }
        */
    }
    
}
