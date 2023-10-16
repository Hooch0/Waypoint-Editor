using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    [ExecuteInEditMode]
    public class WaypointSceneController : MonoBehaviour
    {
        public IReadOnlyDictionary<uint, IReadOnlyWaypoint> RuntimeWaypointMap => _runtimeWaypointMap;
        public IReadOnlyDictionary<uint, IReadOnlyWaypointConnections> RuntimeConnectionMap => _runtimeConnectionMap;

        [SerializeReference] private List<WaypointGroup> _waypointGroups = new List<WaypointGroup>();

        private Dictionary<uint, IReadOnlyWaypoint> _runtimeWaypointMap = new Dictionary<uint, IReadOnlyWaypoint>();
        private Dictionary<uint, IReadOnlyWaypointConnections> _runtimeConnectionMap = new Dictionary<uint, IReadOnlyWaypointConnections>();
        
        private void Awake()
        {
            GenerateRuntimeMap();
        }

        private void GenerateRuntimeMap()
        {
            _runtimeWaypointMap.Clear();
            _runtimeConnectionMap.Clear();

            foreach(WaypointGroup group in _waypointGroups)
            {
                foreach (Waypoint waypoint in group.Waypoints)
                {
                    _runtimeWaypointMap.Add(waypoint.ID, waypoint);
                }

                foreach (WaypointConnections connections in group.Connections)
                {
                    _runtimeConnectionMap.Add(connections.ID, connections);
                }
            }
        }
    }
}