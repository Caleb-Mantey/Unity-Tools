using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Relu.Tools
{
    public class ProjectGeneratorEditor : EditorWindow
    {
        
        private static string _companyName = "Relu";
        private static string _productName = "Product Name";
        private static string _rootNameSpace = "Relu";
        private static string _rootFolder = "_Project";
        
        private static List<string> _defaultFolders = new List<string>
        {
            "Art", "Audio", "Animations", "Documentation", "Materials", "Textures",
            "ExternalAssets", "UI", "Models", "Prefabs", "Scenes", "Scripts"
        };
        
        private ReorderableList _folderList;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            _folderList = new ReorderableList(_defaultFolders, typeof(string), true, true, true, true)
            {
                drawHeaderCallback = DrawHeader, drawElementCallback = DrawElement
            };
        }

        private static void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Folders");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = (List<string>)_folderList.list;
            element[index] = EditorGUI.TextField(rect, element[index]);
        }

        public static void ShowWindow()
        {
            GetWindow(typeof(ProjectGeneratorEditor), false, "Folder Structure");
        }

        private void OnGUI()
        {
            DrawHeaderSection();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.HelpBox("Enter your custom folder names below:", MessageType.Info);
            EditorGUILayout.Space(10);

            _folderList.DoLayoutList();
            
            EditorGUILayout.Space(15);

            if (GUILayout.Button("Create Folders"))
            {
                SetupProject();
            }
            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
            
        }
        
        private void DrawHeaderSection()
        {
            // Load your company logo image from the Packages folder
            string logoPath = "Packages/com.calebmantey.tools/logo.png";
            Texture2D logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
        
            if (logoTexture != null)
            {
                Rect logoRect = GUILayoutUtility.GetRect(position.width, 100, GUI.skin.box);
                GUI.DrawTexture(logoRect, logoTexture, ScaleMode.ScaleToFit);

                GUILayout.Space(5);
            }

            // Company title or tool name
            CenteredText("Relu Interactives", EditorStyles.boldLabel);
            CenteredText("Project Structure Generator", EditorStyles.largeLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Company Name:");
            _companyName = EditorGUILayout.TextField(_companyName);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Product Name:");
            _productName = EditorGUILayout.TextField(_productName);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("C# Root Name Space:");
            _rootNameSpace = EditorGUILayout.TextField(_rootNameSpace);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Root Folder:");
            _rootFolder = EditorGUILayout.TextField(_rootFolder);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private static void CenteredText(string textTitle, GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(textTitle, style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void SetupProject()
        {
            Debug.Log(Application.dataPath);

            CreateDirectories(_rootFolder, _defaultFolders.ToArray());
            
            PlayerSettings.productName = _productName;
            PlayerSettings.companyName = _companyName;
            EditorSettings.projectGenerationRootNamespace = _rootNameSpace;

            // Refresh the asset database to save changes
            AssetDatabase.SaveAssets();
            
            AssetDatabase.Refresh();
        }

        public static void CreateDirectories(string root, params string[] dir)
        {
            var fullpath = Path.Combine(Application.dataPath, root);
            foreach (var newDirectory in dir)
                Directory.CreateDirectory(Path.Combine(fullpath,newDirectory));
        }
    }
}
