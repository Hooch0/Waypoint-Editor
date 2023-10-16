using System;
using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    //This was copied and modified from Unity's Handles.FreeMoveHandle 
    internal class WaypointFreeMove
    {
        public event Action<int> StartMove;
        public event Action<int> EndMove;

        private Plane _plane = new Plane(Vector3.up, Vector3.zero);

        public Vector3 Do(int id, Vector3 position)
        {
            Event current = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);

            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == id && current.button == 0)
                    {
                        GUIUtility.hotControl = id;

                        if (_plane.Raycast(ray, out float enter))
                        {
                            position = ray.GetPoint(enter);
                        }
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        StartMove?.Invoke(id);
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != id)
                        {
                            break;
                        }

                        if (_plane.Raycast(ray, out float enter))
                        {
                            position = ray.GetPoint(enter);
                        }
                        GUI.changed = true;
                        current.Use();
                        break;
                    }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        EndMove?.Invoke(id);
                    }
                    break;
            }


            return position;
        }
    }
}

