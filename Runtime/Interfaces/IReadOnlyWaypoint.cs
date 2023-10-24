using UnityEngine;

namespace Hooch.Waypoint
{
    public interface IReadOnlyWaypoint
    {
        bool HasTag { get; }
        uint ID { get; }
        Vector3 Position { get; }
        float Radius { get; }
        float Height { get; }
        string Tag { get; }
        bool IsEvent { get; }
    }
}

