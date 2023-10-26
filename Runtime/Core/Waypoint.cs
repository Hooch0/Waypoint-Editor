using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public class Waypoint : IReadOnlyWaypoint, IReadOnlyInternalWaypointEvent
    {
        public bool HasTag => string.IsNullOrEmpty(Tag) == false;
        public uint ID { get => _iD; }
        public Vector3 Position { get => _position; set => _position = value; }
        public float Radius { get => _radius; set => _radius = value; }
        public float Height { get => _height; set => _height = value; }
        public string Tag { get => _tag; set => _tag = value; }
        public bool HasEvents => _events.Count > 0;

        public List<WaypointEvent> Events => _events;

        [SerializeField] private uint _iD;
        [SerializeField] private Vector3 _position;
        [SerializeField] private float _radius;
        [SerializeField] private float _height;
        [SerializeField] private string _tag;
        [SerializeReference] private List<WaypointEvent> _events = new List<WaypointEvent>();

        public Waypoint(uint id, Vector3 position, float detectionRadius) 
        {
            _iD = id;
            _position = position;
            _radius = detectionRadius;
            _height = 0.0f;
        }

        public void AddEvent(WaypointEvent evt)
        {
            _events.Add(evt);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + (int)ID;
        }
    }
}


