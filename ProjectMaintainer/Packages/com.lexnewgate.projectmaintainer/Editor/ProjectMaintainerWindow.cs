using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Sunflower.ProjectMaintainer
{
    public class ProjectMaintainerWindow : EditorWindow
    {
        private string m_searchRootStr;
        private string m_assetPathStr;
        private string m_outResultPath;

        [MenuItem("Tools/Project Maintainer")]
        public static void ShowExample()
        {
            var window = GetWindow<ProjectMaintainerWindow>();
            window.titleContent = new GUIContent("Project Maintainer");
        }


        public void CreateGUI()
        {
            var root = rootVisualElement;
            
            root.Add(new Label("SearchPath:"));

            var searchRootField = new TextField();
            searchRootField.RegisterValueChangedCallback((evt => { m_searchRootStr = evt.newValue; }));
            root.Add(searchRootField);
            
            
            root.Add(new Label("OutputPath:"));
            var outResultField = new TextField();
            outResultField.RegisterValueChangedCallback((evt => { m_outResultPath = evt.newValue; }));
            root.Add(outResultField);
            
            var button = new Button();
            button.text = "Search";
            button.clickable.clicked += OnSearch;
            root.Add(button);

            var assetPathField = new TextField();
            assetPathField.RegisterValueChangedCallback((evt => { m_assetPathStr = evt.newValue; }));
            root.Add(assetPathField);

            var button2 = new Button();
            button2.text = "Detect";
            button2.clickable.clicked += OnDetect;
            root.Add(button2);

        }

        private void OnDetect()
        {
            var guids = new List<string>();
            BrokenPrefabUtility.CollectMissingPrefabGuidsByAssetPath(this.m_assetPathStr, guids);
            foreach (var guid in guids)
            {
                var output = $"guid:\n{guid}\noriginal path:\n{AssetDatabase.GUIDToAssetPath(guid)}";
                Debug.Log(output);
            }
        }

        private void OnSearch()
        {
            var missingGuids = new List<string>();
            BrokenPrefabUtility.CollectMissingPrefabGuidsByFolderPath(this.m_searchRootStr, missingGuids);

            foreach (var missingGuid in missingGuids)
            {
                var output = $"guid:\n{missingGuid}\noriginal path:\n{AssetDatabase.GUIDToAssetPath(missingGuid)}";
                Debug.Log(output);
            }

            if (!string.IsNullOrEmpty(this.m_outResultPath))
            {
                var missAssetPaths = missingGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
                File.WriteAllLines(this.m_outResultPath, missAssetPaths);
            }
        }
    }
}