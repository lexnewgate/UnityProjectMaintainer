using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Sunflower.ProjectMaintainer
{
    public static class BrokenPrefabUtility
    {
        public enum AssetType
        {
            None,
            Prefab,
            Scene
        }

        public static void CollectMissingPrefabGuidsByFolderPath(string folderPath, HashSet<string> missingGuids)
        {
            var ownerGuids = new List<string>();
            ownerGuids.AddRange(AssetDatabase.FindAssets("t:Scene", new[] { folderPath }));
            ownerGuids.AddRange(AssetDatabase.FindAssets("t:Prefab", new[] { folderPath }));

            foreach (var ownerGuid in ownerGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(ownerGuid);
                CollectMissingPrefabGuidsByAssetPath(path, missingGuids);
            }
        }

        public static void CollectMissingPrefabGuidsByAssetPath(string assetPath, HashSet<string> guids)
        {
            var assetType = GetAssetTypeByPath(assetPath);
            if (assetType == AssetType.Prefab)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (go == null)
                {
                    return;
                }

                CollectMissingPrefabGuids(go, guids);
            }
            else if (assetType == AssetType.Scene)
            {
                var scene = EditorSceneManager.OpenScene(assetPath);
                CollectMissingPrefabGuids(scene, guids);
            }
        }

        public static AssetType GetAssetTypeByPath(string assetPath)
        {
            if (assetPath.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase))
            {
                return AssetType.Prefab;
            }

            if (assetPath.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase))
            {
                return AssetType.Scene;
            }

            return AssetType.None;
        }

        public static void CollectMissingPrefabGuids(Scene scene, HashSet<string> guids)
        {
            var rootGos = scene.GetRootGameObjects();
            foreach (var rootGo in rootGos)
            {
                CollectMissingPrefabGuids(rootGo, guids);
            }
        }

        public static string GetMissingPrefabGuid(GameObject go)
        {
            var so = new SerializedObject(go);
            var prop = so.GetIterator();
            while (prop.Next(true))
            {
                if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (prop.objectReferenceValue != null) continue;
                if (prop.objectReferenceInstanceIDValue == 0) continue;

                string guid = string.Empty;
                long localid = 0;

                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prop.objectReferenceInstanceIDValue, out guid,
                        out localid))
                {
                    return guid;  
                }

                Debug.LogError("Failed to get guid for instance id: " + prop.objectReferenceInstanceIDValue);
            }

            return string.Empty;
        }

        public static void CollectMissingPrefabGuids(GameObject go, HashSet<string> guids)
        {
            var children = go.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                var missingPrefabGuid = GetMissingPrefabGuid(child.gameObject);
                if (string.IsNullOrEmpty(missingPrefabGuid))
                {
                    continue;
                }

                guids.Add(GetMissingPrefabGuid(child.gameObject));
            }
        }
    }
}