using System;
using System.Collections.Generic;

namespace Hooch.Waypoint
{
    public interface IReadOnlyWaypointConnections
    {
        uint ID { get; }
        IReadOnlyList<WaypointTransition> Transitions { get; }

        IReadOnlyList<WaypointTransition> SortedTransitions<TKey>(Func<WaypointTransition, TKey> comparison);
    }
}
