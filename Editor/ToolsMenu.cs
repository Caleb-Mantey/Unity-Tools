using UnityEditor;
using UnityEngine;

namespace Relu.Tools
{
    public class ToolsMenu
    {
        [MenuItem("Relu/Tools/Create Default Folders")]
        public static void SetupFolders()
        {
            FolderGeneratorEditor.ShowWindow();
        }

        [MenuItem("Relu/Tools/XR/Basic Setup")]
        public static void SetupXRPackages()
        {
            PackagesSetup.SetupXRPackages();
        }

        [MenuItem("Relu/Tools/XR/XR Interaction Toolkit/Setup")]
        public static void AddXRInteractionToolkit()
        {
            PackagesSetup.InstallPackage("xr.interaction.toolkit");
        }
    }
}
