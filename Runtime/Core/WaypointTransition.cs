using System;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public class WaypointTransition : IReadOnlyWaypointTransition
    {
        [field: SerializeField] public uint ID { get; private set; }

        //Probability number is generated after weight calculation.
        //Is not exact proability, sorta.
        public int ProbabilityNumber { get; set; }
        public int Weight => _weight == 0 ? 1 : _weight;

        [SerializeField] private int _weight;

        public WaypointTransition(uint iD)
        {
            ID = iD;
        }
    }
}