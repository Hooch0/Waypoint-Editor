using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;


namespace Hooch.Waypoint
{
    /// <summary>
    /// Waypoint path handler. This is used by traversal agents to handle getting waypoints from the system.
    /// </summary>
    [Serializable]
    public abstract class WaypointPathHandler
    {
        /// <summary>
        /// Event triggered when the traversing agent has reached the end of the path.
        /// </summary>
        public event Action WaypointPathFinished;

        /// <summary>
        /// Event triggered when traversing agent has the reached the next waypoint in its path.
        /// </summary>
        public event Action<IReadOnlyWaypoint> WaypointReached;

        /// <summary>
        /// The active waypoint scene controller this handler uses.
        /// </summary>
        public WaypointSceneController SceneController { get; private set; }

        /// <summary>
        /// Is the current simulation running? Not having a current waypoint or not having an active path status will return false.
        /// </summary>
        public bool SimulationRunning => CurrentWaypoint != null && CurrentPathStatus == PathStatus.Active;

        /// <summary>
        /// The current status of the path handler.
        /// </summary>
        public PathStatus CurrentPathStatus { get; private set; } = PathStatus.Inactive;

        /// <summary>
        /// The current waypoint we are traversing towards.
        /// </summary>
        public IReadOnlyWaypoint CurrentWaypoint { get; private set; }

        /// <summary>
        /// Should detection care about height difference between the waypoint and the traversing agent?
        /// </summary>
        [field: SerializeField] public bool IgnoreHeightDetection { get; set; }

        private Transform _agentTransform;

        public WaypointPathHandler(WaypointSceneController controller, Transform agentTransform)
        {
            SceneController = controller;
            _agentTransform = agentTransform;
        }

        /// <summary>
        /// Set the path.
        /// </summary>
        /// <param name="waypointID"></param>
        public virtual void SetPath(uint waypointID)
        {
            if (SceneController.RuntimeWaypointMap.ContainsKey(waypointID) == false)
            {
                Debug.LogError($"WaypointPathHandler -- Unable to set path. Provided waypoint ID {waypointID} does not exist in runtime map.");
                return;
            }

            CurrentPathStatus = PathStatus.Active;
            CurrentWaypoint = SceneController.RuntimeWaypointMap[waypointID];
        }

        /// <summary>
        /// cancel the path and clears the path.
        /// </summary>
        public virtual void Cancelpath()
        {
            CurrentWaypoint = null;
            CurrentPathStatus = PathStatus.Inactive;
        }

        /// <summary>
        /// Suspends the simulation of the path.
        /// </summary>
        public virtual void SuspendPath()
        {
            CurrentPathStatus = PathStatus.Suspended;
        }

        /// <summary>
        /// resumes the simulation of the path.
        /// </summary>
        public virtual void ResumePath()
        {
            if (CurrentWaypoint != null)
            {
                CurrentPathStatus = PathStatus.Active;
            }
        }

        /// <summary>
        /// Simulates the detection of the next waypoint and handles switching waypoints.
        /// </summary>
        public void UpdateSimulation()
        {
            if (SimulationRunning == false) return;

            Vector3 positon = _agentTransform.position;

            if (IgnoreHeightDetection == true)
            {
                positon.y = CurrentWaypoint.Position.y;
            }

            if (Vector3.Distance(positon, CurrentWaypoint.Position) < CurrentWaypoint.Radius)
            {
                WaypointReached?.Invoke(CurrentWaypoint);

                IReadOnlyInternalWaypointEvent internalEvent = (IReadOnlyInternalWaypointEvent)CurrentWaypoint;
                if (internalEvent.HasEvents == true)
                {
                    SceneController.Internal_RaiseEventWaypointReached(internalEvent, this);
                }
                
                CurrentWaypoint = GetNextWaypoint();

                if (CurrentWaypoint == null)
                {
                    Cancelpath();
                    WaypointPathFinished?.Invoke();
                }
                else
                {
                    OnCurrentWaypointChanged();
                }
            }
        }

        /// <summary>
        /// Called when the current waypoint has changed. Can be used to set a new destination.
        /// </summary>
        protected abstract void OnCurrentWaypointChanged();

        private IReadOnlyWaypoint GetNextWaypoint()
        {
            IReadOnlyWaypointConnections connections = SceneController.RuntimeConnectionMap[CurrentWaypoint.ID];
            IReadOnlyList<IReadOnlyWaypointTransition> transitions = connections.SortedTransitions(x => x.ID);

            if (transitions.Count == 0) return null;

            IReadOnlyWaypointTransition selectedTransition = null;

            if (connections.TransitionLogic == null)
            {

                selectedTransition = transitions[0];

                if (transitions.Count > 1)
                {
                    float value = URandom.Range(0.0f, 100.0f);
                    foreach (IReadOnlyWaypointTransition trans in transitions)
                    {
                        if (trans.ProbabilityNumber >= value)
                        {
                            selectedTransition = trans;
                            break;
                        }
                    }
                }
            }
            else
            {
                selectedTransition = connections.TransitionLogic.Activate(this);
            }


            return SceneController.RuntimeWaypointMap[selectedTransition.ID]; ;
        }
    }
}

