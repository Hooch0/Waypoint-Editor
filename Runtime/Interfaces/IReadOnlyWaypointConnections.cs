using System;
using System.Collections.Generic;

namespace Hooch.Waypoint
{
    public interface IReadOnlyWaypointConnections
    {
        uint ID { get; }
        WaypointTransitionLogic TransitionLogic { get; }
        IReadOnlyList<IReadOnlyWaypointTransition> Transitions { get; }

        IReadOnlyList<IReadOnlyWaypointTransition> SortedTransitions<TKey>(Func<IReadOnlyWaypointTransition, TKey> comparison);
    }
}
