using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    public sealed class WaypointSceneController : MonoBehaviour
    {
        [field: SerializeField] public WaypointSceneAsset SceneAsset { get; set; }

        private Dictionary<string, List<IReadOnlyWaypoint>> _runtimeTagMap = new Dictionary<string, List<IReadOnlyWaypoint>>();
        private Dictionary<WaypointPathHandler, HandlerEventPair> _activeEvents = new Dictionary<WaypointPathHandler, HandlerEventPair>();

        private List<WaypointPathHandler> _handlersToRemove = new List<WaypointPathHandler>();

        private bool _isValid => SceneAsset != null && SceneAsset.RuntimeWaypointMap != null && SceneAsset.RuntimeWaypointMap != null && _runtimeTagMap != null;


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
            GenerateTagMap();
        }

        private void Update()
        {
            CheckForEvents();
        }

        private void OnDestroy()
        {
            foreach (HandlerEventPair eventPair in _activeEvents.Values)
            {
                foreach (WaypointEvent evt in eventPair.Events)
                {
                    evt.Dispose();
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

        public IList<IReadOnlyWaypoint> GetWaypoint(WaypointRequest request)
        {
            if (_isValid == false)
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
            if (_isValid == false) return null;

            return SceneAsset.GetWaypoint(id);
        }

        public bool TryGetWaypoint(uint id, out IReadOnlyWaypoint waypoint)
        {
            waypoint = null;
            if (_isValid == false) return false;

            return SceneAsset.TryGetWaypoint(id, out waypoint);
        }

        public IReadOnlyWaypointConnections GetConnection(uint id)
        {
            if (_isValid == false) return null;

            return SceneAsset.GetConnection(id);
        }

        public bool TryGetConnetion(uint id, out IReadOnlyWaypointConnections connections)
        {
            connections = null;
            if (_isValid == false) return false;

            return SceneAsset.TryGetConnetion(id, out connections);
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

        private void GenerateTagMap()
        {
            if (_isValid == false) return;

            _runtimeTagMap.Clear();

            foreach (Waypoint waypoint in SceneAsset.TagCacheMap)
            {
                if (_runtimeTagMap.TryAdd(waypoint.Tag, new List<IReadOnlyWaypoint>() { waypoint }) == true) continue;

                _runtimeTagMap[waypoint.Tag].Add(waypoint);
            }
        }
    }
}