using System.IO;
using UnityEditor;
using UnityEngine;

namespace calebmantey
{
    public class ToolsMenu
    {
        [MenuItem("Tools/Relu/Create Default Folders")]
        public static void SetupFolders()
        {
            FolderStructure.CreateDefaultFolders();
        }
    }
    
    
    public class FolderStructure
    {
        public static void CreateDefaultFolders()
        {
            Debug.Log(Application.dataPath);

            CreateDirectories("_Project", "Art", "Audio", "Animations", "Documentation", "Materials", "Textures", "ExternalAssets", "UI", "Models", "Prefabs", "Scenes", "Scripts");
            
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
