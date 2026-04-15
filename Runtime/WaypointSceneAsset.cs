using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


[assembly: InternalsVisibleTo("WaypointEditor.Editor")]
namespace Hooch.Waypoint
{
    public class WaypointSceneAsset : ScriptableObject, ISerializationCallbackReceiver
    {

        [field: SerializeReference] public Waypoint[] RuntimeWaypointMap { get; private set; }
        [field: SerializeReference] public WaypointConnections[] RuntimeConnectionMap { get; private set; }
        [field: SerializeReference] public Waypoint[] TagCacheMap { get; private set; }

        [SerializeReference] private List<WaypointGroup> _waypointGroups = new List<WaypointGroup>();

        internal void Internal_RebuildRuntimeIndex()
        {
            List<Waypoint> waypoints = GetAllWaypoints();
            List<WaypointConnections> connections = GetAllConnections();

            if (waypoints.Count == 0)
            {
                RuntimeWaypointMap = null;
                RuntimeConnectionMap = null;
                TagCacheMap = null;
                return;
            }

            uint maxID = waypoints.Max(x => x.ID) + 1;

            RuntimeWaypointMap = new Waypoint[maxID];
            RuntimeConnectionMap = new WaypointConnections[maxID];

            List<Waypoint> tagcache = new List<Waypoint>();


            foreach (Waypoint waypoint in waypoints)
            {
                RuntimeWaypointMap[waypoint.ID] = waypoint;

                if (waypoint.HasTag == true)
                {
                    tagcache.Add(waypoint);
                }
            }

            foreach (WaypointConnections connection in connections)
            {
                connection?.Setup();

                RuntimeConnectionMap[connection.ID] = connection;
            }

            TagCacheMap = tagcache.ToArray();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            Internal_RebuildRuntimeIndex();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        public IReadOnlyWaypoint GetWaypoint(uint id)
        {
            if (TryGetWaypoint(id, out IReadOnlyWaypoint waypoint) == false)
            {
                Debug.LogError("Waypoint Editor -- Unable to Get Waypoint with ID \"{id}\". Either the runtime map is empty/null or the provided ID does not exist.");
            }

            return waypoint;
        }

        public bool TryGetWaypoint(uint id, out IReadOnlyWaypoint waypoint)
        {
            waypoint = null;
            if (RuntimeWaypointMap == null) return false;

            bool invalidID = RuntimeWaypointMap.Length < id;
            waypoint = invalidID == false ? RuntimeWaypointMap[id] : null;

            if (invalidID == true || waypoint == null) return false;

            return true;
        }

        public IReadOnlyWaypointConnections GetConnection(uint id)
        {
            if (TryGetConnetion(id, out IReadOnlyWaypointConnections connection) == false)
            {
                Debug.LogError("Waypoint Editor -- Unable to Get Waypoint connection with ID \"{id}\". Either the runtime map is empty/null or the provided ID does not exist.");
            }

            return connection;
        }

        public bool TryGetConnetion(uint id, out IReadOnlyWaypointConnections connection)
        {
            connection = null;
            if (RuntimeWaypointMap == null) return false;

            bool invalidID = RuntimeWaypointMap.Length < id;
            connection = invalidID == false ? RuntimeConnectionMap[id] : null;

            if (invalidID == true || connection == null) return false;

            return true;
        }

        private List<Waypoint> GetAllWaypoints()
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            foreach (WaypointGroup group in _waypointGroups)
            {
                if (group == null) continue;
                foreach (Waypoint waypoint in group.Waypoints)
                {
                    if (waypoint == null) continue;
                    waypoints.Add(waypoint);
                }
            }

            return waypoints.OrderBy(x => x.ID).ToList();
        }

        private List<WaypointConnections> GetAllConnections()
        {
            List<WaypointConnections> connections = new List<WaypointConnections>();

            foreach (WaypointGroup group in _waypointGroups)
            {
                if (group == null) continue;
                foreach (WaypointConnections connection in group.Connections)
                {
                    if (connection == null) continue;
                    connections.Add(connection);
                }
            }

            return connections.OrderBy(x => x.ID).ToList();
        }
    }
}
