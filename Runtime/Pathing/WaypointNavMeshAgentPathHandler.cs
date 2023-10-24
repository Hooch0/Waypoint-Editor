using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;

namespace Hooch.Waypoint
{
    /// <summary>
    /// Prebuilt path handler for Unity's NavMesh Agent.
    /// </summary>
    public class WaypointNavMeshAgentPathHandler : WaypointPathHandler
    {
        private NavMeshAgent _agent;

        public WaypointNavMeshAgentPathHandler(WaypointSceneController controller, NavMeshAgent agent) : base(controller, agent.transform)
        {
            _agent = agent;
        }

        public override void SetPath(uint waypointID)
        {
            base.SetPath(waypointID);
            _agent.ResetPath();
            _agent.SetDestination(CurrentWaypoint.Position);
        }

        public override void Cancelpath()
        {
            base.Cancelpath();
            _agent.isStopped = true;
            _agent.ResetPath();
        }

        public override void SuspendPath()
        {
            base.SuspendPath();
            _agent.isStopped = true;
        }

        public override void ResumePath()
        {
            if (CurrentWaypoint != null)
            {
                base.ResumePath();
                _agent.isStopped = false;
                _agent.SetDestination(CurrentWaypoint.Position);
            }
        }

        protected override void OnCurrentWaypointChanged()
        {
            _agent.SetDestination(CurrentWaypoint.Position);
        }
    }
}
