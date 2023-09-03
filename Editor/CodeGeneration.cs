using UnityEditor;

namespace Relu.Tools
{
    public static class CodeGeneration
    {
        public static void CreateScript(string script, string filename)
        {
            string path = $"Packages/com.calebmantey.tools/Templates/Scripts/{script}";
            
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, filename);
        }
    }
}
