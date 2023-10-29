using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.IMGUI.Controls;

namespace Hooch.Waypoint.Editor
{
    public class WaypointEventsDropdown : AdvancedDropdown
    {
        public event Action<WaypointEvent> ItemPicked;

        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();

        public WaypointEventsDropdown(AdvancedDropdownState state) : base(state) { }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Events");

            List<Type> eventTypes = GetAllWaypointEventTypes();
            _typeMap.Clear();
            foreach (Type type in eventTypes)
            {
                _typeMap.Add(type.Name, type);
                AdvancedDropdownItem newObject = new AdvancedDropdownItem(type.Name);
                root.AddChild(newObject);
            }

            return root;
        }


        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            WaypointEvent evt = (WaypointEvent)Activator.CreateInstance(_typeMap[item.name]);
            ItemPicked?.Invoke(evt);
        }

        private List<Type> GetAllWaypointEventTypes()
        {
            List<Type> objects = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WaypointEvent))))
                {
                    objects.Add(type);
                }
            }
            return objects;
        }
    }
}
