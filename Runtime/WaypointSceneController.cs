using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    public sealed class WaypointSceneController : MonoBehaviour
    {
        public IReadOnlyDictionary<uint, IReadOnlyWaypoint> RuntimeWaypointMap => _runtimeWaypointMap;
        public IReadOnlyDictionary<uint, IReadOnlyWaypointConnections> RuntimeConnectionMap => _runtimeConnectionMap;

        [SerializeReference] private List<WaypointGroup> _waypointGroups = new List<WaypointGroup>();

        private Dictionary<uint, IReadOnlyWaypoint> _runtimeWaypointMap = new Dictionary<uint, IReadOnlyWaypoint>();
        private Dictionary<uint, IReadOnlyWaypointConnections> _runtimeConnectionMap = new Dictionary<uint, IReadOnlyWaypointConnections>();
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
            GenerateRuntimeMap();
        }

        private void Update()
        {
            CheckForEvents();
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
                    connections.Setup();
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
    }
}