using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public static class WaypointInput
    {
        public static bool GetPlacementInput(Event current) => current.button == 0 && GetPlacementModifier(current) == true;

        public static bool GetSelectionInput(Event current) => current.button == 0;

        public static bool GetDeselectionInput(Event current) => GetSelectionModifier(current) == false && (current.button == 0 );

        public static bool GetDeletionInput(Event current) => current.keyCode == KeyCode.Delete;
        public static bool GetSelectionBoxInput(Event current) => current.button == 0 && current.control == false;

        public static bool GetSelectionModifier(Event current) => current.shift;
        public static bool GetPlacementModifier(Event current) => current.control;
    }
}
