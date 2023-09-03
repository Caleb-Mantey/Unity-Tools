using UnityEditor;
using UnityEngine;

namespace Relu.Tools
{
    public class ToolsMenu
    {
        private const string packageId = "com.example.yourpackagename"; // Replace with your package ID
        
        [MenuItem("Relu/Tools/Create Default Structure")]
        public static void SetupFolders()
        {
            ProjectGeneratorEditor.ShowWindow();
        }
        
        [MenuItem("Relu/Tools/ Switch to Android")]
        public static void SwitchToAndroid()
        {
            // Set the build target to Android
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        #region XRTools

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

            [MenuItem("Relu/Tools/XR/XR Interaction Toolkit/Install Samples")]
            public static void OpenPackage()
            {
                //PlayerSettings.op
            }

        #endregion
    }
}
