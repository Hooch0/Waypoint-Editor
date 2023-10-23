using System;
using UnityEngine;

namespace Hooch.Waypoint
{
    [Serializable]
    public class Waypoint : IReadOnlyWaypoint
    {
        public bool HasTag => string.IsNullOrEmpty(Tag) == false;
        public uint ID { get => _iD; }
        public Vector3 Position { get => _position; set => _position = value; }
        public float Radius { get => _radius; set => _radius = value; }
        public float Height { get => _height; set => _height = value; }
        public string Tag { get => _tag; set => _tag = value; }

        [SerializeField] private uint _iD;
        [SerializeField] private Vector3 _position;
        [SerializeField] private float _radius;
        [SerializeField] private float _height;
        [SerializeField] private string _tag;

        public Waypoint(uint id, Vector3 position, float detectionRadius) 
        {
            _iD = id;
            _position = position;
            _radius = detectionRadius;
            _height = 0.0f;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + (int)ID;
        }
    }
}


