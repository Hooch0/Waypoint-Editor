using System;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public class WaypointTransition : IReadOnlyWaypointTransition
    {
        [field: SerializeField] public uint ID { get; private set; }
        public float Probability => _probability;

        [SerializeField] private float _probability;

        public WaypointTransition(uint iD)
        {
            ID = iD;
        }
    }
}