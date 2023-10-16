using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public class WaypointSelectionBox
    {
        public event Action<List<Waypoint>, bool> SelectionBoxUpdate;
        public bool IsSelectionBoxDragging { get; private set;}

        private WaypointHandle _handler;
        private WaypointSelectionMove _selectionMove;
        private Rect _selectionBox;

        

        public WaypointSelectionBox(WaypointHandle handler, WaypointSelectionMove selectionMove)
        {
            _handler = handler;
            _selectionMove = selectionMove;
            _selectionBox = new Rect();
        }

        public void HandleSelectionBox()
        {
            
            Event current = Event.current;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (_selectionMove.FreeMoveDrag == true) return;

            if (IsSelectionBoxDragging == true)
            {
                Handles.BeginGUI();
                GUI.Box(_selectionBox, "", EditorStyles.selectionRect);
                Handles.EndGUI();
            }

            //Handles Selection box events
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (WaypointInput.GetSelectionBoxInput(current) == true)
                    {
                        GUIUtility.hotControl = controlID;
                        StartDrag(current);
                    }
                    break;
                case EventType.MouseDrag:
                    if (WaypointInput.GetSelectionBoxInput(current) == true)
                    {
                        UpdateDrag(current);
                    }
                    
                    break;
                case EventType.MouseUp:
                    if (WaypointInput.GetSelectionBoxInput(current) == true && IsSelectionBoxDragging == true )
                    {
                        GUIUtility.hotControl = 0;
                        EndDrag(current);
                    }
                    break;
            }

        }
        
        private void StartDrag(Event current)
        {
            _selectionBox.x = current.mousePosition.x;
            _selectionBox.y = current.mousePosition.y;
            _selectionBox.width = 0;
            _selectionBox.height = 0;   
        }

        private void UpdateDrag(Event current)
        {
            IsSelectionBoxDragging = true;
            _selectionBox.width = current.mousePosition.x - _selectionBox.x;
            _selectionBox.height = current.mousePosition.y - _selectionBox.y;
        }

        private void EndDrag(Event current)
        {
            _selectionBox.width = current.mousePosition.x - _selectionBox.x;
            _selectionBox.height = current.mousePosition.y - _selectionBox.y;

            IsSelectionBoxDragging = false;

            bool clearFlag = current.shift == false;

            List<Waypoint> collection = new List<Waypoint>();

            foreach(Waypoint waypoint in _handler.Waypoints)
            {
                Vector3 screenPoint = Camera.current.WorldToScreenPoint(waypoint.Position);
                Vector2 guiSpace  = new Vector2(screenPoint.x, Camera.current.pixelHeight - screenPoint.y);

                if (_selectionBox.Contains(guiSpace, true) == false) continue;

                int controlID = GUIUtility.GetControlID(waypoint.GetHashCode(), FocusType.Passive);

                collection.Add(waypoint);
            }

            SelectionBoxUpdate?.Invoke(collection, clearFlag);
        }

    }
}
