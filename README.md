# Waypoint Editor (Package)

The Waypoint Editor provides a system to create, manage, add functionality to waypoints that allow AI agents to follow and perform unique actions upon reaching destinations. This is not intended to replace existing pathfinding solutions but instead compliment them and allow for new mechanics. (This guide is in progress.)

## Setup
### Settings
When the system is used for the first time a folder will be created called "Waypoint System Setttings" and in it will be a settings file. In this file you can change the default radius the path handlers use to detect when they have reached a waypoint (note: this radius can also be changed for each individual waypoint in the waypoint editor window). Next, we have the Ground Detection Layer, this is used to detect objects that are acceptable for waypoints to be placed/created on.
The path for this is Assets -> Waypoint System Settings -> Settings

### Preferences
There is a preference page for modifying colors the system uses. This can range from selected colors, ID colors, arrow head colors, etc. This page also has the keybinds that are currently used (note: these keybinds cannot be changed)
To navaigate to this preference page go to Edit -> Preferences -> Waypoint System Setttings.

![image](https://github.com/user-attachments/assets/6a0f5e81-d86d-4795-9251-ef5a074ba7d9)


## Waypoint Event's
Waypoint Event's allows you add custom logic to a waypoint node. This logic can alter the state of agent traversing it and hook into other system for endless possibilities.

## How to create a Waypoint Event
To create a waypoint event inherit from the class ```WaypointEvent.cs``` and implement the following functions ```Activate```, ```CanActivate```, ```Update```, and ```Abort```. At this point your new Waypoint Event will appear in the inspector. All thats left is to add functionality to this.

## How to add a Waypoint Event to a Waypoint Node
![image](https://github.com/user-attachments/assets/cfe47b45-1023-43de-9211-9a112dbaa384) ![image](https://github.com/user-attachments/assets/8686de7b-9024-4e4f-bf9f-324112bcf668)

## Example Waypoint Event
```
public class WaypointEventWait : WaypointEvent
{
    public float timeToWait;

    //Used to show serialized value appear in inspector.
    [field: SerializeField] public Vector2 RandomTimeToStay { get; private set; }

    private Timer _timer;
    private WaypointPathHandler _pathHandler;
    private WaypointEventStatus _status = WaypointEventStatus.Running;

    //Triggered when the waypoint has been reached and the event is activated.
    public override void Activate(IReadOnlyWaypoint waypoint, WaypointPathHandler pathHandler)
    {
        _pathHandler = pathHandler;

        //Custom Timer not included in this package that runs until x amount of time has passed, then triggers a callback. Time is calculated using Unity's Time.DeltaTime. 
        _timer = new Timer(timeToWait, OnTimerFinished);
        _timer.Start();
        _pathHandler.SuspendPath();

        _status = WaypointEventStatus.Running;
    }

    //Updates the current status of this event.
    public override WaypointEventStatus Update()
    {
        if (_status == WaypointEventStatus.Finished)
        {
            _pathHandler.ResumePath();
        }

        return _status;
    }

    private void OnTimerFinished()
    {
        _status = WaypointEventStatus.Finished;
    }

    //Checks if we can activate this event.
    public override bool CanActivate(IReadOnlyWaypoint waypoint, WaypointPathHandler pathHandler)
    {
        return true;
    }

    //Aborts this event. This allows for code to be cleaned up. This happens if this event is removed from the Agents WaypointHandler.
    public override void Abort()
    {
        _timer.Stop();
        _pathHandler.ResumePath();
    }
}
```

## Prerequisites
The current version of the Waypoint Editor requires at minimum Unity 2021.3 LTS and should work with future releases. 
