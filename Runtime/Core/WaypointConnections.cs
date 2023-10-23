using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Hooch.Waypoint
{
    [Serializable]
    public class WaypointConnections : IReadOnlyWaypointConnections
    {
        [field: SerializeField] public uint ID { get; private set; }
        public IReadOnlyList<IReadOnlyWaypointTransition> Transitions => _transitions;

        [SerializeReference] private List<WaypointTransition> _transitions = new List<WaypointTransition>();

        public WaypointConnections(uint id)
        {
            ID = id;
        }

        public IReadOnlyList<IReadOnlyWaypointTransition> SortedTransitions<TKey>(Func<IReadOnlyWaypointTransition, TKey> comparison)
        {
            return _transitions.OrderBy(comparison).ToList();
        }

        public void Add(uint id)
        {
            _transitions.Add(new WaypointTransition(id));
        }

        public void Remove(uint id)
        {
            _transitions.Remove(_transitions.First(x => x.ID == id));
        }

        public bool Contains(uint id)
        {
            return _transitions.FirstOrDefault(x => x.ID == id) != null;
        }

    }
}
