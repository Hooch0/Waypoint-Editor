using System.Text;
using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public class WaypointDrawer
    {
        private WaypointHandle _handler;
        private readonly float _waypointLabelOffset = 350;
        private readonly Vector2 _waypointTexturScaleMinMax = new Vector2(5, 100);
        private readonly float _waypointConstantSize = 350;
        private GUIStyle _idLabelStyle = null;
        private readonly Color32 _defualtWaypointColor = new Color32(255, 204, 63, 255);


        public WaypointDrawer(WaypointHandle handler)
        {
            _handler = handler;
        }

        public void SetupGUI()
        {
            _idLabelStyle = GUI.skin.label;
            _idLabelStyle.alignment = TextAnchor.MiddleCenter;
            _idLabelStyle.richText = true;
        }

        public void DrawWaypoints(Waypoint waypoint)
        {
            Vector3 position = waypoint.Position;
            WaypointPreferencesHandler.WaypointPreferences settings = WaypointPreferencesHandler.GetPreferencesSettings();
            bool isSelected = false;
            //Only for selected.
            if (_handler.SelectedWaypoints.ContainsKey(waypoint.ID) && _handler.IsEditing == true)
            {
                Handles.color = settings.RadiusColor;
                Handles.DrawWireDisc(position, Vector3.up, waypoint.Radius);
                Handles.color = Color.white;
                Handles.DrawWireDisc(position, Vector3.up, 0.05f, 2);
                isSelected = true;
            }
            

            if (HandleUtility.WorldToGUIPointWithDepth(position).z >= 0f)
            {
                Handles.BeginGUI();

                float distance = (Camera.current.transform.position - position).magnitude;
                float labelOffset = _waypointLabelOffset / distance;
                float textureSize = _waypointConstantSize / distance;


                const float offset = 7.5f;

                Vector2 guiPoint = HandleUtility.WorldToGUIPoint(position);

                labelOffset = Mathf.Clamp(labelOffset, _waypointTexturScaleMinMax.x, _waypointTexturScaleMinMax.y);
                textureSize = Mathf.Clamp(textureSize, _waypointTexturScaleMinMax.x, _waypointTexturScaleMinMax.y);

                StringBuilder sb = new StringBuilder();

                sb.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(settings.IDColor)}>{waypoint.ID}</color>");


                if (string.IsNullOrEmpty(waypoint.Tag) == false)
                {
                    sb.Append($" - <color=#{ColorUtility.ToHtmlStringRGBA(settings.TagColor)}>{waypoint.Tag}</color>");
                }

                Vector2 labelSize = _idLabelStyle.CalcSize(new GUIContent(sb.ToString()));

                Rect labelRect = new Rect(guiPoint.x - (labelSize.x / 2), guiPoint.y - labelOffset - offset - labelSize.y, labelSize.x, labelSize.y);
                Rect textureRect = new Rect(guiPoint.x - (textureSize / 2), guiPoint.y - offset - textureSize, textureSize, textureSize);

                GUILayout.BeginArea(labelRect);
                GUILayout.BeginHorizontal();

                GUILayout.Label(sb.ToString(), _idLabelStyle);



                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUI.color = isSelected == true ? settings.SelectedWaypointColor : waypoint.HasEvents == true ? settings.HasEventColor : settings.DefaultWaypointColor;
                GUI.DrawTexture(textureRect, WaypointResourceAsset.Instance.WaypointIcon);

                GUI.color = Color.white;

                Handles.EndGUI();
            }

            Handles.color = Color.white;

           
        }

        public void DrawConnections()
        {
            Color lineColor;
            WaypointPreferencesHandler.WaypointPreferences settings = WaypointPreferencesHandler.GetPreferencesSettings();
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
