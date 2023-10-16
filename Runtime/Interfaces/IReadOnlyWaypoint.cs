using UnityEngine;

namespace Hooch.Waypoint
{
    public interface IReadOnlyWaypoint
    {
        uint ID { get; }
        Vector3 Position { get; }
        float Radius { get; }
        float Height { get; }
    }
}

