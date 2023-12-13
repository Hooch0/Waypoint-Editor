using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hooch.Waypoint.Editor
{
    public static class WaypointUtility
    {
        private const string _WAYPOINT_SCENE_ASSET_FILE_NAME = "WaypointSceneAsset.asset";

        public static void DrawLineArrow(Vector3 start, Vector3 end, Color lineColor, Color arrowHeadColor, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowHeadThickness = 1)
        {
            Color cachedColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(start, end);

            Vector3 direction = (end - start).normalized;

            if (direction.magnitude == 0) return;

            //Disabled Up and Down arrow
            //Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + arrowHeadAngle, 0 ,0) * new Vector3(0, 0, 1);
            //Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - arrowHeadAngle, 0 ,0) * new Vector3(0, 0, 1);
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle,0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle,0) * new Vector3(0, 0, 1);

            Handles.color = arrowHeadColor;

            //Disabled Up and Down arrow
            //Handles.DrawLine(end, end + up * arrowHeadLength, arrowHeadThickness);
            //Handles.DrawLine(end, end + down * arrowHeadLength, arrowHeadThickness);
            Handles.DrawLine(end, end + right * arrowHeadLength, arrowHeadThickness);
            Handles.DrawLine(end, end + left * arrowHeadLength, arrowHeadThickness);

            Handles.color = cachedColor;
        }
    
        public static bool Raycast(Ray ray, out RaycastHit hit) => Physics.Raycast(ray, out hit, Mathf.Infinity, WaypointEditorSettingsAsset.Instance.GroundDetectionLayer);

        public static void DebugLog_WaypointControl(int controlID, uint id, string log) => Debug.Log($"ControlID: {controlID} -- ID: {id}\nLog: {log}");

        public static WaypointSceneController CreateSceenController()
        {
            GameObject go = new GameObject("Waypoint Controller", typeof(WaypointSceneController));

            WaypointSceneController sceneData = go.GetComponent<WaypointSceneController>();
            return sceneData;
        }

        public static WaypointSceneAsset GetOrCreateSceneAsset()
        {
            WaypointSceneAsset asset = null;

            (string parentPath, string folderName, string folderPath, string filePath) = GetScenePathInfo();


            if (AssetDatabase.IsValidFolder(folderPath) == false)
            {
                AssetDatabase.CreateFolder(parentPath, folderPath);
            }

            asset = (WaypointSceneAsset)AssetDatabase.LoadAssetAtPath(filePath, typeof(WaypointSceneAsset));
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<WaypointSceneAsset>();
                AssetDatabase.CreateAsset(asset, filePath);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        public static WaypointSceneAsset GetSceneAsset()
        {
            WaypointSceneAsset asset = null;

            (string parentPath, string folderName, string folderPath, string filePath) = GetScenePathInfo();


            if (AssetDatabase.IsValidFolder(folderPath) == false)
            {
                AssetDatabase.CreateFolder(parentPath, folderPath);
            }

            asset = (WaypointSceneAsset)AssetDatabase.LoadAssetAtPath(filePath, typeof(WaypointSceneAsset));

            return asset;
        }

        private static (string parentPath, string folderName, string folderPath, string filePath) GetScenePathInfo()
        {
            Scene currentScene = EditorSceneManager.GetActiveScene();

            string parentPath = Path.GetDirectoryName(currentScene.path);

            string folderName = $"{currentScene.name}";
            string folderPath = Path.Combine(parentPath, folderName);
            string filePath = Path.Combine(folderPath, _WAYPOINT_SCENE_ASSET_FILE_NAME);

            return (parentPath, folderName, folderPath, filePath);
        }

    }
}
