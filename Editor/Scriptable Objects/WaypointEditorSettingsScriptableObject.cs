using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hooch.Waypoint.Editor
{
    public class WaypointEditorSettingsScriptableObject : ScriptableObject
    {
        public static WaypointEditorSettingsScriptableObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CheckForSettingsFile();
                }

                return _instance;
            }
        }

        public float DefaultRadius => _defualtRadius;
        public LayerMask GroundDetectionLayer => _groundDetectionLayer;

        [SerializeField] private float _defualtRadius = 1;
        [SerializeField] private LayerMask _groundDetectionLayer;

        private static WaypointEditorSettingsScriptableObject _instance;
        private const string _FOLDER_NAME = "Waypoint System Settings";
        private const string _ASSET_NAME = "Settings.asset";

        private static WaypointEditorSettingsScriptableObject CheckForSettingsFile()
        {
            string assetsDirectory = "Assets";
            string directory = Path.Combine(assetsDirectory, _FOLDER_NAME);
            string path = Path.Combine(directory, _ASSET_NAME);

            WaypointEditorSettingsScriptableObject settings = null;

            Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(WaypointEditorSettingsScriptableObject));
            if (asset == null)
            {
                if (AssetDatabase.IsValidFolder(directory) == false)
                {
                    AssetDatabase.CreateFolder(assetsDirectory, _FOLDER_NAME);
                }

                settings = CreateInstance<WaypointEditorSettingsScriptableObject>();
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.Refresh();
            }
            else
            {
                settings = (WaypointEditorSettingsScriptableObject)asset;
            }

            return settings;
        }
    }
}
