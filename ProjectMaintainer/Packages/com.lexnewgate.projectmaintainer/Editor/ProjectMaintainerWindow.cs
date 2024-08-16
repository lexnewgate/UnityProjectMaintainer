using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Sunflower.ProjectMaintainer
{
    public class ProjectMaintainerWindow: EditorWindow
    {
       
        [MenuItem("Examples/My Editor Window")]
        public static void ShowExample()
        {
            // MyEditorWindow wnd = GetWindow<MyEditorWindow>();
            // wnd.titleContent = new GUIContent("MyEditorWindow");
            
            var window = GetWindow<ProjectMaintainerWindow>();
            window.titleContent = new GUIContent("Project Maintainer");
        }

        private string m_searchFieldStr;

        public void CreateGUI()
        {
            var root = rootVisualElement;
            root.Add(new Label("SearchPath:"));
            
            var searchField = new TextField();
            // searchField.isDelayed = false;
            searchField.RegisterValueChangedCallback((evt =>
            {
                m_searchFieldStr = evt.newValue; 
            }));
            root.Add(searchField);
            
            
            var button = new Button();
            button.text = "Search";
            button.clickable.clicked += OnSearch;
            root.Add(button);
        }

        private void OnSearch()
        {
            Debug.Log(this.m_searchFieldStr);
        }
    }
}