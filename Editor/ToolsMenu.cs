using UnityEditor;
using UnityEngine;

namespace Relu.Tools
{
    public class ToolsMenu
    {
        private const string packageId = "com.example.yourpackagename"; // Replace with your package ID

        [MenuItem("Relu/Create Project Structure")]
        public static void SetupFolders()
        {
            ProjectGeneratorEditor.ShowWindow();
        }

        [MenuItem("Relu/Switch to Android")]
        public static void SwitchToAndroid()
        {
            // Set the build target to Android
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        #region XRTools
        [MenuItem("Relu/XR/Basic Setup")]
        public static void SetupXRPackages()
        {
            PackagesSetup.SetupXRPackages();
        }

        [MenuItem("Relu/XR/XR Interaction Toolkit/Setup")]
        public static void AddXRInteractionToolkit()
        {
            PackagesSetup.InstallPackage("xr.interaction.toolkit");
        }

        [MenuItem("Relu/XR/XR Interaction Toolkit/Install Samples")]
        public static void OpenPackage()
        {
            UnityEditor.PackageManager.Requests.ListRequest listRequest = UnityEditor.PackageManager.Client.List();
            while (!listRequest.IsCompleted)
            {
                // Wait for the PackageManager to be ready
            }

            // Optionally, you can search for a specific package after opening the PackageManager window
            string packageNameToSearch = "com.xr.interaction.toolkit";
            UnityEditor.PackageManager.Requests.SearchRequest searchRequest = UnityEditor.PackageManager.Client.Search(packageNameToSearch);
            while (!searchRequest.IsCompleted)
            {
                // Wait for the search to complete
            }

        }
        #endregion

        #region CodeGeneration
        [MenuItem("Relu/Create/Code/Monobehaviour")]
        public static void CreateMonoBehaviourScript()
        {
            CodeGeneration.CreateScript("Monobehaviour.cs.txt", "NewScript.cs");
        }

        [MenuItem("Relu/Create/Code/ScriptableObject")]
        public static void CreateScriptableObject()
        {
            CodeGeneration.CreateScript("ScriptableObjects.cs.txt", "NewScriptableObject.cs");
        }

        [MenuItem("Relu/Create/Code/Interface")]
        public static void CreateInterface()
        {
            CodeGeneration.CreateScript("Interface.cs.txt", "NewInterface.cs");
        }

        [MenuItem("Relu/Create/Code/Abstract Class")]
        public static void CreateAbstractClass()
        {
            CodeGeneration.CreateScript("AbstractClass.cs.txt", "NewAbstractClass.cs");
        }

        [MenuItem("Relu/Create/Code/Editor/Editor Window")]
        public static void CreateEditorWindowScript()
        {
            CodeGeneration.CreateScript("EditorWindow.cs.txt", "NewEditorWindow.cs");
        }

        [MenuItem("Relu/Create/Code/Editor/Editor Script")]
        public static void CreateEditorScript()
        {
            CodeGeneration.CreateScript("Editor.cs.txt", "NewEditor.cs");
        }
        #endregion
    }
}
