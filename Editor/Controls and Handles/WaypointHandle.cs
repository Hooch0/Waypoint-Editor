using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public class WaypointHandle
    {
        public event Action<List<Waypoint>> SelectionChanged;
        public event Action SelectionValuesChanged;

        public bool IsEditing { get; private set; }
        public bool IsAutolink { get; private set; }

        public WaypointIDHandler IDHandler { get; private set; }

        public WaypointGroup CurrentGroup { get; private set; }


        public IReadOnlyList<Waypoint> Waypoints => _waypoints;
        public IReadOnlyDictionary<uint, Waypoint> SelectedWaypoints => _selectedWaypoints;
        public IReadOnlyList<WaypointConnections> Connections => _connections;
        public IReadOnlyDictionary<uint, Waypoint> WaypointMap => _waypointMap;


        private Waypoint _firstSelectedWaypoint;
        private List<Waypoint> _waypoints = new List<Waypoint>();
        private Dictionary<uint, Waypoint> _selectedWaypoints = new Dictionary<uint, Waypoint>();
        private List<WaypointConnections> _connections = new List<WaypointConnections>();
        private Dictionary<uint, Waypoint> _waypointMap = new Dictionary<uint, Waypoint>();
        private List<Waypoint> _removingWaypoints = new List<Waypoint>();
        private bool _hasAnySelectedWaypoints = false;
        private WaypointSelectionMove _selectionMove;
        private WaypointDrawer _drawer;
        private WaypointSelectionBox _selectionBox;

        private WaypointEditorWindow _editor;

        private bool _isDirty;

        public WaypointHandle(WaypointEditorWindow controller)
        {
            _editor = controller;
            IDHandler = new WaypointIDHandler();
            _selectionMove = new WaypointSelectionMove(this);
            _selectionMove.SelectionPositionChanged += OnSelectionPositionChanged;

            _drawer = new WaypointDrawer(this);
            _selectionBox = new WaypointSelectionBox(this, _selectionMove);
            _selectionBox.SelectionBoxUpdate += OnSelectionBoxUpdated;
        }

        public void SetDirty()
        {
            _isDirty = true;

        }

        public void HandleWaypoints(bool isEditing, bool isAutolink)
        {
            IsEditing = isEditing;
            IsAutolink = isAutolink;
            if (_waypoints != null)
            {
                OnSceneGUI();
                EndHandle();
            }

            if (_isDirty == true)
            {
                ApplyDirty();
            }
        }

        public List<Waypoint> GetFilteredSelectedWaypoints()
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            if (_selectedWaypoints.Count > 0)
            {
                //If we have 2 or less, always add the first sleected waypoint first.
                waypoints.Add(_firstSelectedWaypoint);

                foreach (KeyValuePair<uint, Waypoint> waypointPair in _selectedWaypoints)
                {
                    if (waypointPair.Key == _firstSelectedWaypoint.ID) continue;
                    waypoints.Add(waypointPair.Value);
                }

                //if we have more the 2, sort the list based on the waypoints ID
                if (_selectedWaypoints.Count > 2)
                {
                    waypoints = waypoints.OrderBy(x => x.ID).ToList();
                }
            }

            return waypoints;
        }

        public List<Waypoint> GetSelectedWaypoints()
        {
            List<Waypoint> waypoints = new List<Waypoint>();
            if (_selectedWaypoints.Count > 0)
            {
                foreach (KeyValuePair<uint, Waypoint> waypointPair in _selectedWaypoints)
                {
                    waypoints.Add(waypointPair.Value);
                }
            }

            return waypoints;
        }

        public void ResetSelectedWaypoints()
        {
            ClearSelection();
        }

        public void SetSelectedGroup(WaypointGroup group, bool hardRefresh = false)
        {
            if (group == CurrentGroup && hardRefresh == false) return;

            CurrentGroup = group;

            if (CurrentGroup == null ) return;

            UpdateData(CurrentGroup.Waypoints, CurrentGroup.Connections);

            if (hardRefresh == true)
            {
                Dictionary<uint, Waypoint> newSelection = new Dictionary<uint, Waypoint>();

                foreach(KeyValuePair<uint, Waypoint> pairs in _selectedWaypoints)
                {
                    if(_waypointMap.ContainsKey(pairs.Key) == true)
                    {
                        newSelection.Add(pairs.Key, pairs.Value);
                    }
                }

                _selectedWaypoints = newSelection;
                SelectionChanged?.Invoke(_selectedWaypoints.Values.ToList());
            }
        }

        public void WaypointGroupCleanup(WaypointGroup group)
        {
            foreach(Waypoint waypoint in group.Waypoints)
            {
                IDHandler.AddReuseID(waypoint.ID);
            }

            if (group.Waypoints.Count > 0)
            {
                _editor.MarkDirty();
            }
        }

        public void LinkSelectedWaypoints()
        {
            RegisterUndo("Linking Waypoint(s)");

            List<Waypoint> selectedWaypoints = GetFilteredSelectedWaypoints();

            for (int i = 0; i < selectedWaypoints.Count; i++)
            {
                Waypoint currentWaypoint = selectedWaypoints[i];
                Waypoint nextWaypoint = null;

                if (i + 1 < selectedWaypoints.Count)
                {
                    nextWaypoint = selectedWaypoints[i + 1];
                }

                if (currentWaypoint == null || nextWaypoint == null || CurrentGroup.IsConnected(currentWaypoint, nextWaypoint) == true) continue;

                LinkWaypoints(currentWaypoint, nextWaypoint);
            }
        }

        public void UnlinkSelectedWaypoints()
        {
            RegisterUndo("Ulinking Waypoint(s)");

            List<Waypoint> selectedWaypoints = GetFilteredSelectedWaypoints();

            for (int i = 0; i < selectedWaypoints.Count; i++)
            {
                Waypoint currentWaypoint = selectedWaypoints[i];

                if (currentWaypoint == null) continue; 

                for (int j = 0; j < selectedWaypoints.Count; j++)
                {
                    //Skip
                    if (i == j) continue;

                    Waypoint nextWaypoint = selectedWaypoints[j];

                    if (CurrentGroup.IsConnected(currentWaypoint, nextWaypoint) == true)
                    {
                        UnlinkWaypoints(currentWaypoint, nextWaypoint);
                    }
                }
            }
        }

        public void RegisterUndo(string msg)
        {
            Undo.RegisterCompleteObjectUndo(_editor.SceneAsset, msg);
        }

        public void ClearSelection()
        {
            _firstSelectedWaypoint = null;
            _selectedWaypoints.Clear();
            SelectionChanged?.Invoke(_selectedWaypoints.Values.ToList());
        }

        private void OnSceneGUI()
        {
            _drawer.SetupGUI();

            HandleWaypointIterationEvents();

            if (IsEditing == true)
            {
                HandleWaypointPlacement();
                _selectionBox.HandleSelectionBox();
                _selectionMove.HandleFreeMove();
            }

            _drawer.DrawConnections();

        }

        private void EndHandle()
        {
            foreach (Waypoint waypoint in _removingWaypoints)
            {
                RemoveWaypoint(waypoint);
            }

            _removingWaypoints.Clear();
        }

        private void ApplyDirty()
        {
            _editor.SerializedWaypointEditor.SetIsDifferentCacheDirty();
            _editor.SerializedWaypointEditor.Update();
            _editor.SerializedSceneAsset.SetIsDifferentCacheDirty();
            _editor.SerializedSceneAsset.Update();
            _isDirty = false;
        }

        private void UpdateData(List<Waypoint> waypoints, List<WaypointConnections> connections)
        {
            _waypoints = waypoints;
            _connections = connections;

            _waypointMap.Clear();
            foreach(Waypoint waypoint in waypoints)
            {
                _waypointMap.Add(waypoint.ID, waypoint);
            }

            List<uint> selectedWaypointsToRemove = new List<uint>();

            foreach(uint id in _selectedWaypoints.Keys)
            {
                if (_waypointMap.ContainsKey(id) == false)
                {
                    selectedWaypointsToRemove.Add(id);
                }
            }

            if (selectedWaypointsToRemove.Count > 0)
            {
                foreach (uint id in selectedWaypointsToRemove)
                {
                    _selectedWaypoints.Remove(id);
                }
            }
        }

        private void HandleWaypointIterationEvents()
        {

            Event current = Event.current;

            //Gets a control ID for the waypoint iteration
            int waypointsIteraionControlID = GUIUtility.GetControlID(FocusType.Passive);
            
            //Handles delete key event
            switch (Event.current.GetTypeForControl(waypointsIteraionControlID))
            {
                case EventType.KeyDown:
                    //Delete Waypoint
                    if (WaypointInput.GetDeletionInput(current) && _selectedWaypoints.Count > 0 && current.control == false)
                    {
                        foreach (KeyValuePair<uint, Waypoint> waypointPair in _selectedWaypoints)
                        {
                            _removingWaypoints.Add(waypointPair.Value);
                        }
                        ClearSelection();
                        current.Use();

                    }
                    break;
            }

            //Handle drawing waypoint and detecting input on them.
            foreach (Waypoint waypoint in _waypoints)
            {
                _drawer.DrawWaypoints(waypoint);

                //Conflicts with placement
                if (current.control == true || IsEditing == false || _selectionBox.IsSelectionBoxDragging == true) continue;

                //Get a control ID for a waypoint
                int controlID = GUIUtility.GetControlID(waypoint.GetHashCode(), FocusType.Passive);

                //Get the position of the waypoint convereted to handles space.
                Vector3 position = Handles.matrix.MultiplyPoint(waypoint.Position);

                //Switch state based on the current event being processed by the IMGUI system.
                switch (Event.current.GetTypeForControl(controlID))
                {
                    //This adds the ability for our Handle to be detected based on mouse distance to the Control.
                    case EventType.Layout:
                        HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, 1));
                        break;
                    case EventType.MouseDown:
                        if (_selectionMove.DetectSelection(waypoint, controlID) == true)
                        {
                            _hasAnySelectedWaypoints = true;
                            return;
                        }
                        break;

                    case EventType.MouseUp:
                        if (_selectionMove.FreeMoveDrag == false && (WaypointInput.GetSelectionInput(current) == true || WaypointInput.GetDeselectionInput(current) == true))
                        {
                            _hasAnySelectedWaypoints = HandleSelection(waypoint, controlID);
                            GUIUtility.hotControl = 0;
                            break;
                        }
                        _hasAnySelectedWaypoints = false;
                        break;
                }
            }

            switch (Event.current.GetTypeForControl(waypointsIteraionControlID))
            {
                case EventType.MouseUp:
                    if (WaypointInput.GetDeselectionInput(current) == true && _selectedWaypoints.Count > 0 && _hasAnySelectedWaypoints == false && _selectionMove.FreeMoveDrag == false)
                    {
                        _selectionMove.ResetFreeMove();
                        ClearSelection();
                        _hasAnySelectedWaypoints = false;
                    }
                    break;
            }
        }

        private void HandleWaypointPlacement()
        {
            Event current = Event.current;

            if (IsEditing == false) return;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (current.type)
            {
                case EventType.MouseDown:
                    PlaceWaypoint(current);
                    break;
            }
        }

        private bool HandleSelection(Waypoint waypoint, int controlID)
        {
            Event current = Event.current;
            bool usedMouseDown = false;
            bool clickedOnAnyWaypoints = false;

            //Left click near a waypoint
            if (WaypointInput.GetSelectionInput(current) && HandleUtility.nearestControl == controlID && _selectedWaypoints.ContainsKey(waypoint.ID) == false)
            {
                if (current.shift == false)
                {
                    ClearSelection();
                }

                AddSelectedWaypoint(waypoint);
                usedMouseDown = true;
                clickedOnAnyWaypoints = true;
            }
            else if (WaypointInput.GetSelectionInput(current) && WaypointInput.GetSelectionModifier(current) == true && HandleUtility.nearestControl == controlID && _selectedWaypoints.ContainsKey(waypoint.ID) == true)
            {
                RemoveSelectedWaypoint(waypoint);
                
                if (_selectionMove.FreeMoveControlID == controlID)
                {
                    _selectionMove.ResetFreeMove();
                }

                clickedOnAnyWaypoints = true;

                usedMouseDown = true;
            }

            if (usedMouseDown == true)
            {
                current.Use();
            }

            return clickedOnAnyWaypoints;
        }

        private void AddSelectedWaypoint(Waypoint waypoint)
        {
            if (_selectedWaypoints.ContainsKey(waypoint.ID) == true) return;

            _selectedWaypoints.Add(waypoint.ID, waypoint);

            if (_firstSelectedWaypoint == null)
            {
                _firstSelectedWaypoint = waypoint;
            }

            SelectionChanged?.Invoke(_selectedWaypoints.Values.ToList());
        }

        private void AddMultipleWaypoints(List<Waypoint> waypoints, bool autolink = false)
        {
            foreach (Waypoint waypoint in waypoints)
            {
                if (_selectedWaypoints.ContainsKey(waypoint.ID) == true) return;

                _selectedWaypoints.Add(waypoint.ID, waypoint);

                if (autolink == true && _firstSelectedWaypoint != null)
                {
                    LinkWaypoints(_firstSelectedWaypoint, waypoint);
                }

                if (_firstSelectedWaypoint == null)
                {
                    _firstSelectedWaypoint = waypoint;
                }
            }

            SelectionChanged?.Invoke(_selectedWaypoints.Values.ToList());
        }

        private void RemoveSelectedWaypoint(Waypoint waypoint)
        {
            if (_selectedWaypoints.ContainsKey(waypoint.ID) == false) return;

            if (_firstSelectedWaypoint == waypoint)
            {
                _firstSelectedWaypoint = null;
            }

            _selectedWaypoints.Remove(waypoint.ID);

            if (_selectedWaypoints.Count > 0)
            {
                _firstSelectedWaypoint = _selectedWaypoints.First().Value;
            }

            SelectionChanged?.Invoke(_selectedWaypoints.Values.ToList());
        }

        private void PlaceWaypoint(Event current)
        {
            if (WaypointInput.GetPlacementInput(current) == true)
            {
                // Setting the hotControl tells the Scene View that this mouse down/up event cannot be considered
                // a picking action because the event is in use.
                Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                RaycastHit hit;
                if (WaypointUtility.Raycast(ray, out hit))
                {
                    Waypoint waypoint = CreateWaypoint(hit.point);



                    if (IsAutolink == true)
                    {
                        foreach(KeyValuePair<uint, Waypoint> map in _selectedWaypoints)
                        {
                            LinkWaypoints(map.Value, waypoint);
                        }
                    }

                    //Need to apply the changes BEFORE adding the selected waypoint
                    ApplyDirty();
                    ClearSelection();
                    AddSelectedWaypoint(waypoint);

                    //When a new wapoint is made, set it as the first selected waypoint
                    _firstSelectedWaypoint = waypoint;


                    current.Use();
                }
            }
        }

        private Waypoint CreateWaypoint(Vector3 position)
        {
            RegisterUndo("Created Waypoint");

            Waypoint waypoint = new Waypoint(IDHandler.GetUniqueID(), position, WaypointEditorSettingsAsset.Instance.DefaultRadius);
            
            CurrentGroup.AddWaypoint(waypoint);
            _waypointMap.Add(waypoint.ID, waypoint);
            _isDirty = true;
            _editor.MarkDirty();

            return waypoint;
        }

        private void RemoveWaypoint(Waypoint waypoint)
        {
            RegisterUndo("Removed Waypoint");

            CurrentGroup.RemoveWaypoint(waypoint);
            _waypointMap.Remove(waypoint.ID);
            IDHandler.AddReuseID(waypoint.ID);
            _isDirty = true;
            _editor.MarkDirty();
        }

        private void LinkWaypoints(Waypoint from, Waypoint to)
        {
            CurrentGroup.AddConnection(from, to);
            _isDirty = true;
            _editor.MarkDirty();
        }

        private void UnlinkWaypoints(Waypoint from, Waypoint to)
        {
            CurrentGroup.RemoveConnection(from, to);
            _isDirty = true;
            _editor.MarkDirty();
        }

        private void OnSelectionPositionChanged()
        {
            SelectionValuesChanged?.Invoke();
        }

        private void OnSelectionBoxUpdated(List<Waypoint> waypoints, bool clearFlag)
        {
            if (clearFlag == true)
            {
                _selectedWaypoints.Clear();
            }

            AddMultipleWaypoints(waypoints);
        }
    }
}