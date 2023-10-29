namespace Hooch.Waypoint
{
    public class WaypointRequest
    {
        public string Tag { get; private set; }
        public uint ID { get; private set; }

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
