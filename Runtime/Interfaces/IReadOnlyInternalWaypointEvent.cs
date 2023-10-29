using System.Collections.Generic;

namespace Hooch.Waypoint
{
    internal interface IReadOnlyInternalWaypointEvent
    {
        bool HasEvents { get; }

        List<WaypointEvent> Events { get; }
    }
}
