using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    public class WaypointSceneController : MonoBehaviour
    {
        public event Action<IReadOnlyWaypoint, WaypointPathHandler> EventWaypointReached;

        public IReadOnlyDictionary<uint, IReadOnlyWaypoint> RuntimeWaypointMap => _runtimeWaypointMap;
        public IReadOnlyDictionary<uint, IReadOnlyWaypointConnections> RuntimeConnectionMap => _runtimeConnectionMap;

        [SerializeReference] private List<WaypointGroup> _waypointGroups = new List<WaypointGroup>();

        private Dictionary<uint, IReadOnlyWaypoint> _runtimeWaypointMap = new Dictionary<uint, IReadOnlyWaypoint>();
        private Dictionary<uint, IReadOnlyWaypointConnections> _runtimeConnectionMap = new Dictionary<uint, IReadOnlyWaypointConnections>();
        private Dictionary<string, List<IReadOnlyWaypoint>> _runtimeTagMap = new Dictionary<string, List<IReadOnlyWaypoint>>();
        
        private void Awake()
        {
            GenerateRuntimeMap();
        }

        private void GenerateRuntimeMap()
        {
            _runtimeWaypointMap.Clear();
            _runtimeConnectionMap.Clear();
            _runtimeTagMap.Clear();

            foreach(WaypointGroup group in _waypointGroups)
            {
                foreach (Waypoint waypoint in group.Waypoints)
                {
                    _runtimeWaypointMap.Add(waypoint.ID, waypoint);

                    if (waypoint.HasTag == true)
                    {
                        if (_runtimeTagMap.ContainsKey(waypoint.Tag) == false)
                        {
                            _runtimeTagMap.Add(waypoint.Tag, new List<IReadOnlyWaypoint>());
                        }

                        _runtimeTagMap[waypoint.Tag].Add(waypoint);
                    }
                }

                foreach (WaypointConnections connections in group.Connections)
                {
                    _runtimeConnectionMap.Add(connections.ID, connections);
                }
            }
        }
    
        public IList<IReadOnlyWaypoint> GetWaypoint(WaypointRequest request)
        {
            if (_runtimeWaypointMap == null || _runtimeWaypointMap == null || _runtimeTagMap == null)
            {
                Debug.LogError("Waypoint Editor -- Runtime maps have not generated! Unable to get waypoint.");
                return null;
            }

            List<IReadOnlyWaypoint> waypoints = null;

            if (request.Tag != null)
            {
                if (_runtimeTagMap.TryGetValue(request.Tag, out waypoints) == false)
                {
                    Debug.LogError("$Waypoint Editor -- Unable to get waypoint by provided tag \"{request.Tag}\".");
                }
            }
            else
            {
                IReadOnlyWaypoint waypoint;
                if (_runtimeWaypointMap.TryGetValue(request.ID, out waypoint) == false)
                {
                    Debug.LogError($"Waypoint Editor -- Unable to get waypoint by provided ID \"{request.ID}\".");
                }
                else
                {
                    waypoints = new List<IReadOnlyWaypoint>() { waypoint };
                }


            }

            return waypoints;
        }
    
        internal void Interanl_RaiseEventWaypointReached(IReadOnlyWaypoint waypoint, WaypointPathHandler pathHandler)
        {
            EventWaypointReached?.Invoke(waypoint, pathHandler);
        }
    }
}