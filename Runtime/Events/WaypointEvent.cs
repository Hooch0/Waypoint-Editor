using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public abstract class WaypointEvent : ICloneable
    {
        /// <summary>
        /// Triggered the first time this event is activated.
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="pathHandler"></param>
        public abstract void Activate(IReadOnlyWaypoint waypoint, WaypointPathHandler pathHandler);

        /// <summary>
        /// Triggered each frame like Unity's Update.
        /// </summary>
        /// <returns>Return the current event status. Marking an event as Finished cleans it up.</returns>
        public abstract WaypointEventStatus Update();

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
