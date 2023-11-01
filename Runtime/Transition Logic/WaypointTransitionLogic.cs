using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public abstract class WaypointTransitionLogic
    {
        protected IReadOnlyWaypointConnections Connections { get; private set; }
        protected IReadOnlyList<IReadOnlyWaypointTransition> Transitions { get; private set; }


        internal void Intialize(IReadOnlyWaypointConnections connections, IReadOnlyList<IReadOnlyWaypointTransition> transitions)
        {
            Connections = connections;
            Transitions = transitions;
        }

        public abstract IReadOnlyWaypointTransition Activate(WaypointPathHandler pathHandler);
    }
}
