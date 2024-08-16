using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Sunflower.ProjectMaintainer
{
    public static class BrokenPrefabUtility
    {
        public static bool IsPrefabMissing(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(go);
            
            return prefabStatus == PrefabInstanceStatus.MissingAsset;
        }

        public static string GetMissingPrefabGuid(GameObject go)
        {
            if (!IsPrefabMissing(go))
            {
                return string.Empty;
            }
            
            var so = new SerializedObject(go);
            var prop = so.GetIterator();
            while (prop.Next(true))
            {
                if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (prop.objectReferenceValue != null) continue;
                if (prop.objectReferenceInstanceIDValue == 0) continue;
                var assetPath = AssetDatabase.GetAssetPath(prop.objectReferenceInstanceIDValue);
                return AssetDatabase.AssetPathToGUID(assetPath);
            }
            
            return string.Empty;
        }

        public static void CollectMissingPrefabGuids(GameObject go,IList<string>guids,bool recursive=true)
        {
            var children=go.GetComponentsInChildren<Transform>(recursive);
            foreach (var child in children)
            {
                if (IsPrefabMissing(child.gameObject))
                {
                    guids.Add(GetMissingPrefabGuid(child.gameObject));
                }
            }
        }
    }
}