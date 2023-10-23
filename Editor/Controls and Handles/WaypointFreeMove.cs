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

        private RaycastHit[] _hits = new RaycastHit[100];
        private Plane _plane = new Plane(Vector3.up, Vector3.zero);

        public Vector3 Do(int id, Vector3 position)
        {
            Event current = Event.current;

            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == id && current.button == 0)
                    {
                        GUIUtility.hotControl = id;

                        if (PlaceObject(current.mousePosition, out Vector3 newPos, out Vector3 normal))
                        {
                            position = newPos;
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

                        if (PlaceObject(current.mousePosition, out Vector3 newPos, out Vector3 normal))
                        {
                            position = newPos;
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

        private bool PlaceObject(Vector2 guiPosition, out Vector3 position, out Vector3 normal)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            RaycastHit resultHit;
            bool valid = TryRaySnap(ray, out resultHit) || TryPlaceOnPlane(ray, out resultHit);
            position = valid ? ray.GetPoint(resultHit.distance) : Vector3.zero;
            normal = valid ? resultHit.normal : Vector3.up;

            return valid;
        }

        private bool TryRaySnap(Ray ray, out RaycastHit resultHit)
        {
            RaycastHit? obj = RaySnap(ray);
            if (obj == null)
            {
                resultHit = default(RaycastHit);
                resultHit.distance = float.PositiveInfinity;
                return false;
            }
            resultHit = obj.Value;
            return true;
        }

        private RaycastHit? RaySnap(Ray ray)
        {
            Camera current = Camera.current;
            if (current == null)
            {
                return null;
            }
            int cullingMask = current.cullingMask;
            RaycastHit raycastHit = default;
            raycastHit.distance = float.PositiveInfinity;
            if (GetNearestHitFromPhysicsScene(ray, Physics.defaultPhysicsScene, cullingMask, ignorePrefabInstance: false, ref raycastHit))
            {
                return raycastHit;
            }
            return null;
        }

        private bool GetNearestHitFromPhysicsScene(Ray ray, PhysicsScene physicsScene, int cullingMask, bool ignorePrefabInstance, ref RaycastHit raycastHit)
        {
            float distance = raycastHit.distance;
            int hitsCount = physicsScene.Raycast(ray.origin, ray.direction, _hits, distance, cullingMask, QueryTriggerInteraction.Ignore);
            float currentDistance = distance;
            int closeestIndex = -1;
            for (int k = 0; k < hitsCount; k++)
            {
                if (_hits[k].distance < currentDistance)
                {
                    currentDistance = _hits[k].distance;
                    closeestIndex = k;
                }
            }
            if (closeestIndex >= 0)
            {
                raycastHit = _hits[closeestIndex];
                return true;
            }
            return false;
        }

        private bool TryPlaceOnPlane(Ray ray, out RaycastHit resultHit)
        {
            resultHit = default(RaycastHit);
            resultHit.distance = float.PositiveInfinity;
            resultHit.normal = Vector3.up;
            if (_plane.Raycast(ray, out float enter))
            {
                resultHit.distance = enter;
                resultHit.normal = _plane.normal;
                return true;
            }
            return false;
        }
    }
}