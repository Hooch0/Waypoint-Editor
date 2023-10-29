using System;

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
        /// Use to check if this event can be triggerd.
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="pathHandler"></param>
        /// <returns></returns>
        public abstract bool CanActivate(IReadOnlyWaypoint waypoint, WaypointPathHandler pathHandler);

        /// <summary>
        /// Called when an event is aborted. Used for cleaning up and stopping active events.
        /// </summary>
        public abstract void Abort();

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
