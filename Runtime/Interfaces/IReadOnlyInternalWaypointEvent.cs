using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    internal interface IReadOnlyInternalWaypointEvent
    {
        bool HasEvents { get; }

        List<WaypointEvent> Events { get; }
    }
}
