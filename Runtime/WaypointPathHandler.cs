using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using URandom = UnityEngine.Random;


namespace Hooch.Waypoint
{
    public class WaypointPathHandler
    {
        public event Action<string> WaypointWithTagReached;
        public bool IsActive { get; private set; }

        public IReadOnlyWaypoint CurrentWaypoint { get; private set; }

        private WaypointSceneController _controller;
        private NavMeshAgent _agent;

        
        private bool _pathSuspended;

        public void Initialize(WaypointSceneController controller, NavMeshAgent agent)
        {
            _controller = controller;
            _agent = agent;
        }

        public void SetPath(uint waypointID)
        {
            if (_controller.RuntimeWaypointMap.ContainsKey(waypointID) == false)
            {
                Debug.LogError($"WaypointPathHandler -- Unable to set path. Provided waypoint ID {waypointID} does not exist in runtime map.");
                return;
            }

            _agent.ResetPath();
            _pathSuspended = false;
            CurrentWaypoint = _controller.RuntimeWaypointMap[waypointID];
            IsActive = true;
        }

        public void Cancelpath()
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            CurrentWaypoint = null;
            IsActive = false;
        }

        public void SuspendPath()
        {
            _pathSuspended = true;
            _agent.isStopped = true;
            IsActive = false;
        }

        public void ResumePath()
        {
            if (CurrentWaypoint != null)
            {
                _pathSuspended = false;
                _agent.isStopped = false;
                _agent.SetDestination(CurrentWaypoint.Position);
                IsActive = true;
            }
        }

        public void UpdateSimulation()
        {
            if (CurrentWaypoint == null || _pathSuspended == true)

            if (_agent.hasPath == false)
            {
                _agent.SetDestination(CurrentWaypoint.Position);
            }

            if (Vector3.Distance(_agent.transform.position, CurrentWaypoint.Position) < CurrentWaypoint.Radius)
            {

                if (CurrentWaypoint.HasTag == true)
                {
                    WaypointWithTagReached?.Invoke(CurrentWaypoint.Tag);
                }
                
                CurrentWaypoint = GetNextWaypoint();

                if (CurrentWaypoint != null)
                {
                    _agent.SetDestination(CurrentWaypoint.Position);
                }
            }

        }

        private IReadOnlyWaypoint GetNextWaypoint()
        {
            IReadOnlyList<IReadOnlyWaypointTransition> transitions = _controller.RuntimeConnectionMap[CurrentWaypoint.ID].SortedTransitions(x => x.Probability);
            
            if(transitions.Count == 0) return null;

            int index = 0;

            if (transitions.Count > 1)
            {
                float value = URandom.Range(0.0f, 1.0f);
                
                for (int i = 0; i < transitions.Count; i++)
                {
                    if (value <= transitions[i].Probability)
                    {
                        index = i;
                        break;
                    }
                }
            }

            return _controller.RuntimeWaypointMap[transitions[index].ID];
            
        }
    }
}

