using UnityEditor;
using UnityEngine;

namespace calebmantey
{
    public class ToolsMenu
    {
        [MenuItem("Relu/Tools/Create Default Folders")]
        public static void SetupFolders()
        {
            FolderGeneratorEditor.ShowWindow();
        }
    }
}
