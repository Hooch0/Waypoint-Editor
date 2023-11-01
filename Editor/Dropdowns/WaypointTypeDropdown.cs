using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Hooch.Waypoint.Editor
{
    public class WaypointTypeDropdown<T> : AdvancedDropdown where T : class
    {
        public event Action<T> ItemPicked;

        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();

        private List<Type> _types = new List<Type>();

        private string _categoryName;

        public WaypointTypeDropdown(string categoryName, AdvancedDropdownState state) : base(state)
        {
            _categoryName = categoryName;
            GenerateTypeMap();
        }

        public void GenerateTypeMap()
        {
            _types = GetAllTypes();
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem(_categoryName);

            _typeMap.Clear();
            foreach (Type type in _types)
            {
                string name = ObjectNames.NicifyVariableName(type.Name);
                _typeMap.Add(name, type);
                AdvancedDropdownItem newObject = new AdvancedDropdownItem(name);
                root.AddChild(newObject);
            }

            return root;
        }


        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            T evt = (T)Activator.CreateInstance(_typeMap[item.name]);
            ItemPicked?.Invoke(evt);
        }

        private List<Type> GetAllTypes()
        {
            List<Type> objects = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                {
                    objects.Add(type);
                }
            }
            return objects;
        }
    }
}
