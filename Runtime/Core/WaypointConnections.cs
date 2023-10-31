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

        public void GenerateWeight()
        {

            if (_transitions.Count <= 1) return;

            int totalWeight = 0;

            foreach (WaypointTransition trans in _transitions)
            {
                totalWeight += trans.Weight;
            }

            int holdingNumber = 0;
            foreach (WaypointTransition trans in _transitions)
            {
                float prob = (float)trans.Weight / (float)totalWeight;
                trans.ProbabilityNumber = (int)(prob * 100) + holdingNumber;
                holdingNumber = trans.ProbabilityNumber;
            }
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
