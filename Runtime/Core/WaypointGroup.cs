using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Hooch.Waypoint
{
    [Serializable]
    public class WaypointGroup
    {
        public string GroupName => _groupName;
        public List<Waypoint> Waypoints { get => _waypoints; set => _waypoints = value; }
        public List<WaypointConnections> Connections => _connections;

        [SerializeField] private List<Waypoint> _waypoints = new List<Waypoint>();
        [SerializeReference] private List<WaypointConnections> _connections = new List<WaypointConnections>();

        [SerializeField] private string _groupName;
        
        public WaypointGroup(string groupName)
        {
            _groupName = groupName;
        }

        public void AddWaypoint(Waypoint waypoint, params uint[] connectionIDs)
        {
            _waypoints.Add(waypoint);
            WaypointConnections connections = new WaypointConnections(waypoint.ID);
            
            foreach(uint id in connectionIDs)
            {
                connections.Add(id);
            }
            _connections.Add(connections);
        }

        public void RemoveWaypoint(uint id)
        {
            Waypoint waypoint = _waypoints.FirstOrDefault(x => x.ID == id);
            
            if (waypoint != null)
            {
                RemoveWaypoint(waypoint);
            }
        }

        public void RemoveWaypoint(Waypoint waypoint)
        {
            if (_waypoints.Contains(waypoint) == false) return;

            //Remove every connection to our waypoint
            foreach(WaypointConnections connection in _connections)
            {
                if (connection.Contains(waypoint.ID))
                {
                    connection.Remove(waypoint.ID);
                }
            }

            //Remove our connection to any waypoints.
            WaypointConnections ourConnections = _connections.FirstOrDefault(x => x.ID == waypoint.ID);
            _connections.Remove(ourConnections);
            _waypoints.Remove(waypoint);
        }

        public void AddConnection(Waypoint from, Waypoint to)
        {
            WaypointConnections fromConnection = _connections.FirstOrDefault(x => x.ID == from.ID);

            if (fromConnection == null || to == null)
            {
                Debug.LogError($"Unable to add connection from ID:{from?.ID} to ID:{to?.ID}");
                return;
            }

            if (fromConnection.Contains(to.ID)) return;

            fromConnection.Add(to.ID);
        }

        public void RemoveConnection(Waypoint from, Waypoint to)
        {
            WaypointConnections fromConnection = _connections.FirstOrDefault(x => x.ID == from.ID);

            if (fromConnection == null || to == null)
            {
                Debug.LogError($"Unable to remove connection from ID:{from?.ID} to ID:{to?.ID}");
                return;
            }

            fromConnection.Remove(to.ID);
        }

        public bool IsConnected(Waypoint from, Waypoint to)
        {
            WaypointConnections fromConnection = _connections.FirstOrDefault(x => x.ID == from.ID);

            if (fromConnection == null || to == null)
            {
                Debug.LogError($"Unable to add connection from ID:{from?.ID} to ID:{to?.ID}");
                return false;
            }

            return fromConnection.Contains(to.ID);
        }
    }
}
