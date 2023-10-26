using System;
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
    [Serializable]
    public class WaypointNavMeshAgentPathHandler : WaypointPathHandler
    {
        public NavMeshAgent Agent { get; private set; }

        public WaypointNavMeshAgentPathHandler(WaypointSceneController controller, NavMeshAgent agent) : base(controller, agent.transform)
        {
            Agent = agent;
            IgnoreHeightDetection = true;
        }

        public override void SetPath(uint waypointID)
        {
            base.SetPath(waypointID);
            Agent.ResetPath();
            Agent.SetDestination(CurrentWaypoint.Position);
        }

        public override void Cancelpath()
        {
            base.Cancelpath();
            Agent.isStopped = true;
            Agent.ResetPath();
        }

        public override void SuspendPath()
        {
            base.SuspendPath();
            Agent.isStopped = true;
        }

        public override void ResumePath()
        {
            if (CurrentWaypoint != null)
            {
                base.ResumePath();
                Agent.isStopped = false;
                Agent.SetDestination(CurrentWaypoint.Position);
            }
        }

        protected override void OnCurrentWaypointChanged()
        {
            Agent.SetDestination(CurrentWaypoint.Position);
        }
    }
}
