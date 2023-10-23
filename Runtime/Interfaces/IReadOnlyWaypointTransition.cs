using System;
using UnityEngine;

namespace Hooch.Waypoint
{
    public interface IReadOnlyWaypointTransition
    {
       uint ID { get; }
       float Probability { get; }
    }
}