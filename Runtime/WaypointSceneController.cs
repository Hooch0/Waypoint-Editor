using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hooch.Waypoint
{
    public sealed class WaypointSceneController : MonoBehaviour
    {
        [SerializeReference] private List<WaypointGroup> _waypointGroups = new List<WaypointGroup>();

        [SerializeField] private Waypoint[] _runtimeWaypointMap;
        [SerializeField] private WaypointConnections[] _runtimeConnectionMap;
        [SerializeField] private Waypoint[] _tagCacheMap;

        private Dictionary<string, List<IReadOnlyWaypoint>> _runtimeTagMap = new Dictionary<string, List<IReadOnlyWaypoint>>();
        private Dictionary<WaypointPathHandler, HandlerEventPair> _activeEvents = new Dictionary<WaypointPathHandler, HandlerEventPair>();

        private List<WaypointPathHandler> _handlersToRemove = new List<WaypointPathHandler>();


        private class HandlerEventPair
        {
            public WaypointPathHandler PathHandler { get; private set; }
            public List<WaypointEvent> Events { get; private set; } = new List<WaypointEvent>();

            public HandlerEventPair(WaypointPathHandler pathHandler, List<WaypointEvent> events)
            {
                PathHandler = pathHandler;
                Events = events;
            }

            public bool Update()
            {
                for (int i = Events.Count - 1; i >= 0; i--)
                {
                    if (Events[i].Update() == WaypointEventStatus.Finished)
                    {
                        Events.RemoveAt(i);
                    }
                }

                return Events.Count == 0;
            }

        }

        private void Awake()
        {
            foreach (Waypoint waypoint in _tagCacheMap)
            {
                if (_runtimeTagMap.TryAdd(waypoint.Tag, new List<IReadOnlyWaypoint>() { waypoint }) == true) continue;

                _runtimeTagMap[waypoint.Tag].Add(waypoint);
            }
        }

        private void Update()
        {
            CheckForEvents();
        }

        public void GenerateRuntimeMap()
        {
            _runtimeTagMap.Clear();

            List<Waypoint> waypoints = GetAllWaypoints();
            List<WaypointConnections> connections = GetAllConnections();


            uint maxID = waypoints.Max(x => x.ID);

            _runtimeWaypointMap = new Waypoint[maxID + 1];
            _runtimeConnectionMap = new WaypointConnections[maxID + 1];

            List<Waypoint> tagcache = new List<Waypoint>();


            foreach (Waypoint waypoint in waypoints)
            {
                _runtimeWaypointMap[waypoint.ID] = waypoint;

                if (waypoint.HasTag == true)
                {
                    tagcache.Add(waypoint);
                }
            }

            foreach (WaypointConnections connection in connections)
            {
                connection?.Setup();

                _runtimeConnectionMap[connection.ID] = connection;
            }

            _tagCacheMap = tagcache.ToArray();
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
                if (TryGetWaypoint(request.ID, out waypoint) == false)
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
            if (_runtimeWaypointMap == null) return false;

            bool invalidID = _runtimeWaypointMap.Length < id;
            waypoint = invalidID == false ? _runtimeWaypointMap[id] : null;

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

        public bool TryGetConnetion(uint id, out IReadOnlyWaypointConnections waypoint)
        {
            waypoint = null;
            if (_runtimeWaypointMap == null) return false;

            bool invalidID = _runtimeWaypointMap.Length < id;
            waypoint = invalidID == false ? _runtimeConnectionMap[id] : null;

            if (invalidID == true || waypoint == null) return false;

            return true;
        }

        public void ClearEvents(WaypointPathHandler handler)
        {
            if (_activeEvents.TryGetValue(handler, out HandlerEventPair handlerEventPair))
            {
                for (int i = handlerEventPair.Events.Count - 1; i >= 0; i--)
                {
                    WaypointEvent wEvent = handlerEventPair.Events[i];
                    wEvent.Abort();
                }
            }
            _activeEvents.Remove(handler);
        }

        public void RemoveEvent<T>(WaypointPathHandler handler) where T : WaypointEvent
        {
            if (_activeEvents.TryGetValue(handler, out HandlerEventPair handlerEventPair))
            {
                Type removeType = typeof(T);
                for (int i = handlerEventPair.Events.Count - 1; i >= 0; i--)
                {
                    WaypointEvent wEvent = handlerEventPair.Events[i];
                    if (wEvent.GetType() == removeType)
                    {
                        wEvent.Abort();
                        handlerEventPair.Events.RemoveAt(i);

                    }
                }
            }
        }

        internal void Internal_RaiseEventWaypointReached(IReadOnlyInternalWaypointEvent waypoint, WaypointPathHandler pathHandler)
        {
            IReadOnlyWaypoint readOnlyWaypoint = (IReadOnlyWaypoint)waypoint;

            foreach (WaypointEvent evt in waypoint.Events)
            {
                if (evt.CanActivate(readOnlyWaypoint, pathHandler) == false) continue;

                WaypointEvent clone = (WaypointEvent)evt.Clone();

                bool tryAddMap = _activeEvents.TryAdd(pathHandler, new HandlerEventPair(pathHandler, new List<WaypointEvent>() { clone }));
                if (tryAddMap == false)
                {
                    _activeEvents[pathHandler].Events.Add(clone);
                }

                clone.Activate((IReadOnlyWaypoint)waypoint, pathHandler);
            }
        }

        private void CheckForEvents()
        {
            foreach (WaypointPathHandler handler in _activeEvents.Keys)
            {
                bool updateComplete = _activeEvents[handler].Update();
                if (updateComplete == true)
                {
                    _handlersToRemove.Add(handler);
                }
            }

            if (_handlersToRemove.Count > 0)
            {
                foreach (WaypointPathHandler handler in _handlersToRemove)
                {
                    _activeEvents.Remove(handler);
                }

                _handlersToRemove.Clear();
            }
        }

        private List<Waypoint> GetAllWaypoints()
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            foreach (WaypointGroup group in _waypointGroups)
            {
                waypoints.AddRange(group.Waypoints);
            }

            return waypoints.OrderBy(x => x.ID).ToList();
        }

        private List<WaypointConnections> GetAllConnections()
        {
            List<WaypointConnections> connections = new List<WaypointConnections>();

            foreach (WaypointGroup group in _waypointGroups)
            {
                connections.AddRange(group.Connections);
            }

            return connections.OrderBy(x => x.ID).ToList();
        }
    }
}