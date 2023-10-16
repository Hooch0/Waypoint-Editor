using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public class WaypointDrawer
    {
        private WaypointHandle _handler;
        private Texture _waypointTexture;
        private readonly float _waypointLabelOffset = 350;
        private readonly Vector2 _waypointLabelSize = new Vector2(100, 20); 
        private readonly Vector2 _waypointTexturScaleMinMax = new Vector2(5, 100);
        private readonly float _waypointConstantSize = 350;
        private GUIStyle _idLabelStyle = null;


        public WaypointDrawer(WaypointHandle handler)
        {
            _handler = handler;
        }

        public void LoadTexture()
        {
            _waypointTexture = Resources.Load<Texture>("Varadia_Waypoint");
        }

        public void SetupGUI()
        {
            _idLabelStyle = GUI.skin.label;
            _idLabelStyle.alignment = TextAnchor.MiddleCenter;
        }

        public void DrawWaypoints(Waypoint waypoint)
        {
            Vector3 position = waypoint.Position;
            WaypointEditorSettingsHandler.WaypointEditorSettings settings = WaypointEditorSettingsHandler.GetEditorSettings();
            //Only for selected.
            if (_handler.SelectedWaypoints.ContainsKey(waypoint.ID) && _handler.IsEditing == true)
            {
                Handles.color = settings.RadiusColor;
                Handles.DrawWireDisc(position, Vector3.up, waypoint.Radius);
                Handles.color = Color.white;
                Handles.DrawWireDisc(position, Vector3.up, 0.05f, 2);
            }
            

            if (HandleUtility.WorldToGUIPointWithDepth(position).z >= 0f)
            {
                Handles.BeginGUI();

                float distance = (Camera.current.transform.position - position).magnitude;
                float textureSize = _waypointConstantSize / distance;
                float labelOffset = _waypointLabelOffset / distance;

                const float offset = 7.5f;

                Vector2 guiPoint = HandleUtility.WorldToGUIPoint(position);

                textureSize = Mathf.Clamp(textureSize, _waypointTexturScaleMinMax.x, _waypointTexturScaleMinMax.y);
                labelOffset = Mathf.Clamp(labelOffset, _waypointTexturScaleMinMax.x, _waypointTexturScaleMinMax.y);

                Rect labelRect = new Rect(guiPoint.x - (_waypointLabelSize.x / 2), (guiPoint.y - labelOffset - offset) - (_waypointLabelSize.y), _waypointLabelSize.x, _waypointLabelSize.y);
                Rect textureRect = new Rect(guiPoint.x - (textureSize / 2), (guiPoint.y - offset) - (textureSize), textureSize, textureSize);
                    
                GUI.color = settings.IDColor;
                GUI.Label(labelRect, waypoint.ID.ToString(), _idLabelStyle);
                GUI.color = Color.white;
                
                GUI.DrawTexture(textureRect, _waypointTexture);
                Handles.EndGUI();
            }

            Handles.color = Color.white;

           
        }

        public void DrawConnections()
        {
            Color lineColor;
            WaypointEditorSettingsHandler.WaypointEditorSettings settings = WaypointEditorSettingsHandler.GetEditorSettings();
            foreach(WaypointConnections connection in _handler.Connections)
            {
                if (_handler.SelectedWaypoints.ContainsKey(connection.ID) == true)
                {
                    lineColor = settings.SelectedLineColor;
                }
                else
                {
                    lineColor = settings.LineColor;
                }

                if (connection.Transitions == null)
                {
                    Debug.Log($"{connection.ID} has null transitions");
                    continue;
                }
                foreach(WaypointTransition transition in connection.Transitions)
                {
                    

                    Vector3 start = _handler.WaypointMap[connection.ID].Position;
                    Vector3 end = _handler.WaypointMap[transition.ID].Position;
                    WaypointUtility.DrawLineArrow(start, end, lineColor, settings.ArrowHeadColor, 0.5f, 25, 3.5f);
                }
            }
        }
    }
}
