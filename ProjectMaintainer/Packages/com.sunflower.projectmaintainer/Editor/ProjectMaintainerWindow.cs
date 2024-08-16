﻿using System.Collections.Generic;
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
        private string m_inputFilePath;
        private string m_outputPath;

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
            
            root.Add(new Label("Output missing id paths:"));
            var inputIdsField= new TextField();
            inputIdsField.RegisterValueChangedCallback((evt => { m_inputFilePath= evt.newValue; }));
            root.Add(inputIdsField);
            var outputPathField = new TextField();
            outputPathField.RegisterValueChangedCallback((evt => { m_outputPath= evt.newValue; }));
            root.Add(outputPathField);
            var button3 = new Button();
            button3.text = "OutputPathsFromInstanceIds";
            button3.clickable.clicked += OnOutputPathsFromInstanceIds;
            root.Add(button3);
        }

        private void OnOutputPathsFromInstanceIds()
        {
           var instanceIDs= File.ReadAllLines(this.m_inputFilePath);
           var paths= instanceIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();
           File.WriteAllLines(this.m_outputPath,paths);
        }

        private void OnDetect()
        {
            var instanceIds = new HashSet<string>();
            BrokenPrefabUtility.CollectMissingPrefabInstanceIDsByAssetPath(this.m_assetPathStr, instanceIds);
            foreach (var instanceID in instanceIds)
            {
                var output = $"instance id:\n{instanceID}";
                Debug.Log(output);
            }
        }

        private void OnSearch()
        {
            var missingInstanceIds = new HashSet<string>();
            BrokenPrefabUtility.CollectMissingPrefabInstanceIdsByFolderPath(this.m_searchRootStr, missingInstanceIds);

            foreach (var missingInstanceID in missingInstanceIds)
            {
                var output = $"missing instanceId:\n{missingInstanceID}\noriginal path:\n{AssetDatabase.GUIDToAssetPath(missingInstanceID)}";
                Debug.Log(output);
            }

            if (!string.IsNullOrEmpty(this.m_outResultPath))
            {
                File.WriteAllLines(this.m_outResultPath, missingInstanceIds);
            }
        }
    }
}