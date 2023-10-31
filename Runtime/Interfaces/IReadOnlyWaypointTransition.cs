namespace Hooch.Waypoint
{
    public interface IReadOnlyWaypointTransition
    {
        uint ID { get; }
        int ProbabilityNumber { get; }
    }
}