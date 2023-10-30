using System;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public class WaypointRequest
    {
        [field: SerializeField] public string Tag { get; private set; }
        [field: SerializeField] public uint ID { get; private set; }

        public WaypointRequest(string tag)
        {
            Tag = tag;
        }

        public WaypointRequest(uint id)
        {
            ID = id;
        }
    }
}
