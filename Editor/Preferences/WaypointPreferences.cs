using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    [InitializeOnLoad]
    public class WaypointPreferencesHandler
    {
        private const string _ID_COLOR_KEY_R = "Varadia.WaypointSettings.IDColor_R";
        private const string _ID_COLOR_KEY_G = "Varadia.WaypointSettings.IDColor_G";
        private const string _ID_COLOR_KEY_B = "Varadia.WaypointSettings.IDColor_B";
        private const string _ID_COLOR_KEY_A = "Varadia.WaypointSettings.IDColor_A";

        private const string _TAG_COLOR_KEY_R = "Varadia.WaypointSettings.TagColor_R";
        private const string _TAG_COLOR_KEY_G = "Varadia.WaypointSettings.TagColor_G";
        private const string _TAG_COLOR_KEY_B = "Varadia.WaypointSettings.TagColor_B";
        private const string _TAG_COLOR_KEY_A = "Varadia.WaypointSettings.TagColor_A";

        private const string _RADIUS_COLOR_KEY_R = "Varadia.WaypointSettings.radiusColor_R";
        private const string _RADIUS_COLOR_KEY_G = "Varadia.WaypointSettings.radiusColor_G";
        private const string _RADIUS_COLOR_KEY_B = "Varadia.WaypointSettings.radiusColor_B";
        private const string _RADIUS_COLOR_KEY_A = "Varadia.WaypointSettings.radiusColor_A";

        private const string _ARROW_HEAD_COLOR_KEY_R = "Varadia.WaypointSettings.arrowHeadColor_R";
        private const string _ARROW_HEAD_COLOR_KEY_G = "Varadia.WaypointSettings.arrowHeadColor_G";
        private const string _ARROW_HEAD_COLOR_KEY_B = "Varadia.WaypointSettings.arrowHeadColor_B";
        private const string _ARROW_HEAD_COLOR_KEY_A = "Varadia.WaypointSettings.arrowHeadColor_A";

        private const string _LINE_COLOR_KEY_R = "Varadia.WaypointSettings.lineColor_R";
        private const string _LINE_COLOR_KEY_G = "Varadia.WaypointSettings.lineColor_G";
        private const string _LINE_COLOR_KEY_B = "Varadia.WaypointSettings.lineColor_B";
        private const string _LINE_COLOR_KEY_A = "Varadia.WaypointSettings.lineColor_A";

        private const string _SELECTED_LINE_COLOR_KEY_R = "Varadia.WaypointSettings.selectedLineColor_R";
        private const string _SELECTED_LINE_COLOR_KEY_G = "Varadia.WaypointSettings.selectedLineColor_G";
        private const string _SELECTED_LINE_COLOR_KEY_B = "Varadia.WaypointSettings.selectedLineColor_B";
        private const string _SELECTED_LINE_COLOR_KEY_A = "Varadia.WaypointSettings.selectedLineColor_A";

        private const string _SELECTED_WAYPOINT_COLOR_KEY_R = "Varadia.WaypointSettings.selectedWaypointColor_R";
        private const string _SELECTED_WAYPOINT_COLOR_KEY_G = "Varadia.WaypointSettings.selectedWaypointColor_G";
        private const string _SELECTED_WAYPOINT_COLOR_KEY_B = "Varadia.WaypointSettings.selectedWaypointColor_B";
        private const string _SELECTED_WAYPOINT_COLOR_KEY_A = "Varadia.WaypointSettings.selectedWaypointColor_A";

        private const string _HAS_EVENT_COLOR_KEY_R = "Varadia.WaypointSettings.hasEventColor_R";
        private const string _HAS_EVENT_COLOR_KEY_G = "Varadia.WaypointSettings.hasEventColor_G";
        private const string _HAS_EVENT_COLOR_KEY_B = "Varadia.WaypointSettings.hasEventColor_B";
        private const string _HAS_EVENT_COLOR_KEY_A = "Varadia.WaypointSettings.hasEventColor_A";

        private const string _DEFAULT_WAYPOINT_COLOR_KEY_R = "Varadia.WaypointSettings.defaultWaypointColor_R";
        private const string _DEFAULT_WAYPOINT_COLOR_KEY_G = "Varadia.WaypointSettings.defaultWaypointColor_G";
        private const string _DEFAULT_WAYPOINT_COLOR_KEY_B = "Varadia.WaypointSettings.defaultWaypointColor_B";
        private const string _DEFAULT_WAYPOINT_COLOR_KEY_A = "Varadia.WaypointSettings.defaultWaypointColor_A";


        public class WaypointPreferences
        {
            public float DefaultRadius { get; set; }
            public Color IDColor { get; set; }
            public Color TagColor { get; set; }
            public Color RadiusColor { get; set; }
            public Color ArrowHeadColor { get; set; }
            public Color LineColor { get; set; }
            public Color SelectedLineColor { get; set; }
            public Color SelectedWaypointColor { get; set; }
            public Color HasEventColor { get; set; }
            public Color DefaultWaypointColor { get; set; }
        }

        public static WaypointPreferences GetPreferencesSettings()
        {
            return new WaypointPreferences
            {
                IDColor = new Color(EditorPrefs.GetFloat(_ID_COLOR_KEY_R, Color.cyan.r), 
                                            EditorPrefs.GetFloat(_ID_COLOR_KEY_G, Color.cyan.g), 
                                            EditorPrefs.GetFloat(_ID_COLOR_KEY_B, Color.cyan.b), 
                                            EditorPrefs.GetFloat(_ID_COLOR_KEY_A, Color.cyan.a)),

                TagColor = new Color(EditorPrefs.GetFloat(_TAG_COLOR_KEY_R, Color.green.r),
                                            EditorPrefs.GetFloat(_TAG_COLOR_KEY_G, Color.green.g),
                                            EditorPrefs.GetFloat(_TAG_COLOR_KEY_B, Color.green.b),
                                            EditorPrefs.GetFloat(_TAG_COLOR_KEY_A, Color.green.a)),

                RadiusColor = new Color(EditorPrefs.GetFloat(_RADIUS_COLOR_KEY_R, Color.cyan.r), 
                                            EditorPrefs.GetFloat(_RADIUS_COLOR_KEY_G, Color.cyan.g), 
                                            EditorPrefs.GetFloat(_RADIUS_COLOR_KEY_B, Color.cyan.b), 
                                            EditorPrefs.GetFloat(_RADIUS_COLOR_KEY_A, Color.cyan.a)),

                ArrowHeadColor = new Color(EditorPrefs.GetFloat(_ARROW_HEAD_COLOR_KEY_R, Color.magenta.r), 
                                            EditorPrefs.GetFloat(_ARROW_HEAD_COLOR_KEY_G, Color.magenta.g), 
                                            EditorPrefs.GetFloat(_ARROW_HEAD_COLOR_KEY_B, Color.magenta.b), 
                                            EditorPrefs.GetFloat(_ARROW_HEAD_COLOR_KEY_A, Color.magenta.a)),

                LineColor = new Color(EditorPrefs.GetFloat(_LINE_COLOR_KEY_R, Color.yellow.r), 
                                            EditorPrefs.GetFloat(_LINE_COLOR_KEY_G, Color.yellow.g), 
                                            EditorPrefs.GetFloat(_LINE_COLOR_KEY_B, Color.yellow.b), 
                                            EditorPrefs.GetFloat(_LINE_COLOR_KEY_A, Color.yellow.a)),

                SelectedLineColor = new Color(EditorPrefs.GetFloat(_SELECTED_LINE_COLOR_KEY_R, Color.cyan.r),
                                            EditorPrefs.GetFloat(_SELECTED_LINE_COLOR_KEY_G, Color.cyan.g),
                                            EditorPrefs.GetFloat(_SELECTED_LINE_COLOR_KEY_B, Color.cyan.b),
                                            EditorPrefs.GetFloat(_SELECTED_LINE_COLOR_KEY_A, Color.cyan.a)),

                SelectedWaypointColor = new Color(EditorPrefs.GetFloat(_SELECTED_WAYPOINT_COLOR_KEY_R, Color.cyan.r),
                                            EditorPrefs.GetFloat(_SELECTED_WAYPOINT_COLOR_KEY_G, Color.cyan.g),
                                            EditorPrefs.GetFloat(_SELECTED_WAYPOINT_COLOR_KEY_B, Color.cyan.b),
                                            EditorPrefs.GetFloat(_SELECTED_WAYPOINT_COLOR_KEY_A, Color.cyan.a)),

                HasEventColor = new Color(EditorPrefs.GetFloat(_HAS_EVENT_COLOR_KEY_R, Color.magenta.r),
                                            EditorPrefs.GetFloat(_HAS_EVENT_COLOR_KEY_G, Color.magenta.g),
                                            EditorPrefs.GetFloat(_HAS_EVENT_COLOR_KEY_B, Color.magenta.b),
                                            EditorPrefs.GetFloat(_HAS_EVENT_COLOR_KEY_A, Color.magenta.a)),

                DefaultWaypointColor = new Color(EditorPrefs.GetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_R, 1.0f),
                                            EditorPrefs.GetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_G, 0.8f),
                                            EditorPrefs.GetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_B, 0.25f),
                                            EditorPrefs.GetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_A, 1.0f))
            };
        }

        public static void SetPreferencesSettings(WaypointPreferences settings)
        {
            EditorPrefs.SetFloat(_ID_COLOR_KEY_R, settings.IDColor.r);
            EditorPrefs.SetFloat(_ID_COLOR_KEY_G, settings.IDColor.g);
            EditorPrefs.SetFloat(_ID_COLOR_KEY_B, settings.IDColor.b);
            EditorPrefs.SetFloat(_ID_COLOR_KEY_A, settings.IDColor.a);

            EditorPrefs.SetFloat(_TAG_COLOR_KEY_R, settings.TagColor.r);
            EditorPrefs.SetFloat(_TAG_COLOR_KEY_G, settings.TagColor.g);
            EditorPrefs.SetFloat(_TAG_COLOR_KEY_B, settings.TagColor.b);
            EditorPrefs.SetFloat(_TAG_COLOR_KEY_A, settings.TagColor.a);

            EditorPrefs.SetFloat(_RADIUS_COLOR_KEY_R, settings.RadiusColor.r);
            EditorPrefs.SetFloat(_RADIUS_COLOR_KEY_G, settings.RadiusColor.g);
            EditorPrefs.SetFloat(_RADIUS_COLOR_KEY_B, settings.RadiusColor.b);
            EditorPrefs.SetFloat(_RADIUS_COLOR_KEY_A, settings.RadiusColor.a);

            EditorPrefs.SetFloat(_ARROW_HEAD_COLOR_KEY_R, settings.ArrowHeadColor.r);
            EditorPrefs.SetFloat(_ARROW_HEAD_COLOR_KEY_G, settings.ArrowHeadColor.g);
            EditorPrefs.SetFloat(_ARROW_HEAD_COLOR_KEY_B, settings.ArrowHeadColor.b);
            EditorPrefs.SetFloat(_ARROW_HEAD_COLOR_KEY_A, settings.ArrowHeadColor.a);

            EditorPrefs.SetFloat(_LINE_COLOR_KEY_R, settings.LineColor.r);
            EditorPrefs.SetFloat(_LINE_COLOR_KEY_G, settings.LineColor.g);
            EditorPrefs.SetFloat(_LINE_COLOR_KEY_B, settings.LineColor.b);
            EditorPrefs.SetFloat(_LINE_COLOR_KEY_A, settings.LineColor.a);

            EditorPrefs.SetFloat(_SELECTED_LINE_COLOR_KEY_R, settings.SelectedLineColor.r);
            EditorPrefs.SetFloat(_SELECTED_LINE_COLOR_KEY_G, settings.SelectedLineColor.g);
            EditorPrefs.SetFloat(_SELECTED_LINE_COLOR_KEY_B, settings.SelectedLineColor.b);
            EditorPrefs.SetFloat(_SELECTED_LINE_COLOR_KEY_A, settings.SelectedLineColor.a);

            EditorPrefs.SetFloat(_SELECTED_WAYPOINT_COLOR_KEY_R, settings.SelectedWaypointColor.r);
            EditorPrefs.SetFloat(_SELECTED_WAYPOINT_COLOR_KEY_G, settings.SelectedWaypointColor.g);
            EditorPrefs.SetFloat(_SELECTED_WAYPOINT_COLOR_KEY_B, settings.SelectedWaypointColor.b);
            EditorPrefs.SetFloat(_SELECTED_WAYPOINT_COLOR_KEY_A, settings.SelectedWaypointColor.a);

            EditorPrefs.SetFloat(_HAS_EVENT_COLOR_KEY_R, settings.HasEventColor.r);
            EditorPrefs.SetFloat(_HAS_EVENT_COLOR_KEY_G, settings.HasEventColor.g);
            EditorPrefs.SetFloat(_HAS_EVENT_COLOR_KEY_B, settings.HasEventColor.b);
            EditorPrefs.SetFloat(_HAS_EVENT_COLOR_KEY_A, settings.HasEventColor.a);

            EditorPrefs.SetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_R, settings.DefaultWaypointColor.r);
            EditorPrefs.SetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_G, settings.DefaultWaypointColor.g);
            EditorPrefs.SetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_B, settings.DefaultWaypointColor.b);
            EditorPrefs.SetFloat(_DEFAULT_WAYPOINT_COLOR_KEY_A, settings.DefaultWaypointColor.a);
        }
    }

    internal class WaypointPreferencesGUIContent
    {
        private static GUIContent _IDColorLabel = new GUIContent("ID Color", "The Color of the currently selectied waypoints ID.");
        private static GUIContent _TagColorLabel = new GUIContent("Tag Color", "The Color of the currently selectied waypoints Tag.");
        private static GUIContent _radiusColorLabel = new GUIContent("Radius Color", "The Color of Radius around the currently selected waypoints.");  
        private static GUIContent _arrowHeadColorLabel = new GUIContent("Arrow Head Color", "The Color of the Arrow head of the connections between waypoints.");
        private static GUIContent _lineColorLabel = new GUIContent("Line Color", "The line color of the connection.");
        private static GUIContent _selectedLineColorLabel = new GUIContent("Selected Line Color", "The line color of the connection when a waypoint is selected.");
        private static GUIContent _selectedWaypointColorLabel = new GUIContent("Selected Waypoint Color", "The waypoint icon color when a waypoint is selected.");
        private static GUIContent _hasEventColorLabel = new GUIContent("Has Event Color", "The waypoint icon color when a waypoint has at least 1 event.");
        private static GUIContent _defaultWaypointColorLabel = new GUIContent("Default Waypoint Color", "The default waypoint icon color.");




        public static void DrawPreferencesButtons(WaypointPreferencesHandler.WaypointPreferences settings)
        {
            EditorGUI.indentLevel += 1;
            GUIStyle headerStyle = EditorStyles.boldLabel;
            headerStyle.fontSize = 14;
            headerStyle.alignment = TextAnchor.MiddleCenter;


            EditorGUILayout.LabelField("Keybinds", headerStyle);
            DrawKeybinds($"control + left_click", "Places a new waypoint.");
            DrawKeybinds($"delete", "Deletes any selected waypoints.");

            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Colors", headerStyle);

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Waypoint", headerStyle);
            settings.SelectedWaypointColor = EditorGUILayout.ColorField(_selectedWaypointColorLabel, settings.SelectedWaypointColor);
            settings.DefaultWaypointColor = EditorGUILayout.ColorField(_defaultWaypointColorLabel, settings.DefaultWaypointColor);

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Properties", headerStyle);
            settings.IDColor = EditorGUILayout.ColorField(_IDColorLabel, settings.IDColor);
            settings.TagColor = EditorGUILayout.ColorField(_TagColorLabel, settings.TagColor);
            settings.RadiusColor = EditorGUILayout.ColorField(_radiusColorLabel, settings.RadiusColor);
            settings.HasEventColor = EditorGUILayout.ColorField(_hasEventColorLabel, settings.HasEventColor);

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Line", headerStyle);
            settings.ArrowHeadColor = EditorGUILayout.ColorField(_arrowHeadColorLabel, settings.ArrowHeadColor);
            settings.LineColor = EditorGUILayout.ColorField(_lineColorLabel, settings.LineColor);
            settings.SelectedLineColor = EditorGUILayout.ColorField(_selectedLineColorLabel, settings.SelectedLineColor);

            EditorGUI.indentLevel -= 1;
        }

        private static void DrawKeybinds(string keybind, string usage)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(keybind);
            EditorGUILayout.LabelField(usage);
            EditorGUILayout.EndHorizontal();
        }

    }

#if UNITY_2018_3_OR_NEWER
    static class WaypointPreferencesProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Varadia/Waypoint Settings", SettingsScope.User)
            {
                label = "Waypoint System Settings",

                guiHandler = (searchContext) =>
                {
                    WaypointPreferencesHandler.WaypointPreferences settings = WaypointPreferencesHandler.GetPreferencesSettings();
                    EditorGUI.BeginChangeCheck();
                    WaypointPreferencesGUIContent.DrawPreferencesButtons(settings);
                    if (EditorGUI.EndChangeCheck())
                    {
                        WaypointPreferencesHandler.SetPreferencesSettings(settings);
                    }
                },

                // Keywords for the search bar in the Unity Preferences menu
                keywords = new HashSet<string>(new[] { "Waypoint" })
            };

            return provider;
        }
    }
#endif

}