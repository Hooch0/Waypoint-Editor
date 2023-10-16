using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Hooch.Waypoint
{
    //This is actually used by the AIAgent for movement.
    public class WaypointPathHandler : MonoBehaviour
    {

        [SerializeField] private uint _testWaypointID;
        [SerializeField] private WaypointSceneController _controller;
        [SerializeField] private NavMeshAgent _agent;

        private IReadOnlyWaypoint _currentWaypoint;

        private void Start()
        {
            if (_controller == null || _agent == null) return;

            SetPath(_testWaypointID);
        }

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

            _currentWaypoint = _controller.RuntimeWaypointMap[waypointID];;
            _agent.SetDestination(_currentWaypoint.Position);
        }

        private void Update()
        {
            if (_currentWaypoint == null) return;

            if (Vector3.Distance(_agent.transform.position, _currentWaypoint.Position) < _currentWaypoint.Radius)
            {
                _currentWaypoint = GetNextWaypoint();

                if (_currentWaypoint != null)
                {
                    _agent.SetDestination(_currentWaypoint.Position);
                }
            }

        }

        private IReadOnlyWaypoint GetNextWaypoint()
        {
            IReadOnlyList<WaypointTransition> transitions = _controller.RuntimeConnectionMap[_currentWaypoint.ID].SortedTransitions(x => x.Probability);
            

            if(transitions.Count == 0) return null;

            int index = 0;

            if (transitions.Count > 1)
            {
                float value = Random.Range(0.0f, 1.0f);
                //This is a hardset random range 
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

