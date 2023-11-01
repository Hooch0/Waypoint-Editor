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
        public WaypointTransitionLogic TransitionLogic => _transitionLogic;
        public IReadOnlyList<IReadOnlyWaypointTransition> Transitions => _transitions;

        [SerializeReference] private List<WaypointTransition> _transitions = new List<WaypointTransition>();
        [SerializeReference] private WaypointTransitionLogic _transitionLogic;

        public WaypointConnections(uint id)
        {
            ID = id;
        }

        public void Setup()
        {
            GenerateWeight();

            if (_transitionLogic != null)
            {
                _transitionLogic.Intialize(this, _transitions);
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

        public void SetTransitionLogic(WaypointTransitionLogic transitionLogic)
        {
            _transitionLogic = transitionLogic;
        }

        private void GenerateWeight()
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

    }
}
