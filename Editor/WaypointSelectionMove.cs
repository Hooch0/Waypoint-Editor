using System;
using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public class WaypointSelectionMove
    {
        public event Action SelectionPositionChanged;

        public Waypoint FreeMoveWaypoint { get; set; }
        public bool FreeMoveDrag { get; private set; }
        public int FreeMoveControlID { get; private set; }


        private WaypointFreeMove _freeMove;
        private WaypointHandle _handler;

        public WaypointSelectionMove( WaypointHandle waypointHandle)
        {
            _handler = waypointHandle;
            _freeMove = new WaypointFreeMove();
            _freeMove.EndMove += OnEndFreeMove;
        }

        public void HandleFreeMove()
        {

            if (FreeMoveDrag == false) return;

            Vector3 GetWaypointPosition(Vector3 position)
            {
                Vector3 newPositon = position;
                RaycastHit hit;
                Ray ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                if (WaypointUtility.Raycast(ray, out hit) == true)
                {
                    newPositon = hit.point;
                }

                return newPositon;
            }

            Waypoint startWaypoint = FreeMoveWaypoint;

            Vector3 startPosition = startWaypoint.Position;
            
            Vector3 position = _freeMove.Do(FreeMoveControlID, startWaypoint.Position);
            if (FreeMoveDrag == false) return;

            Vector3 diff = position - startPosition;

            bool isDirty = diff.magnitude > 0;
            
            if (isDirty == true)
            {
                _handler.RegisterUndo("Moved Waypoint('s)");
            }

            startWaypoint.Position = GetWaypointPosition(position);
            
            foreach (uint id in _handler.SelectedWaypoints.Keys)
            {
                //Skip the waypoint being moved
                if (id == FreeMoveWaypoint.ID) continue;
                Waypoint waypoint = _handler.SelectedWaypoints[id];

                position = waypoint.Position + diff;
                
                waypoint.Position = GetWaypointPosition(position);
            }

            if (isDirty == true)
            {
                SelectionPositionChanged?.Invoke();
                _handler.ApplyDirty();
            }
        }

        public bool DetectSelection(Waypoint waypoint, int controlID)
        {
            Event current = Event.current;

            if (current.shift == false && _handler.SelectedWaypoints.ContainsKey(waypoint.ID) == true && HandleUtility.nearestControl == controlID)
            {
                FreeMoveDrag = true;
                FreeMoveControlID = controlID;
                FreeMoveWaypoint = waypoint;
                return true;
            }

            return false;
        }
        
        public void ResetFreeMove()
        {
            FreeMoveDrag = false;
            FreeMoveControlID = 0;
            FreeMoveWaypoint = null;
        }

        private void OnEndFreeMove(int controlID)
        {
            ResetFreeMove();
        }
    }
}
