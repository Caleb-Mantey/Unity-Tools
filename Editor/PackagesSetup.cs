using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Relu.Tools
{
    public static class PackagesSetup
    {
        public static async void SetupXRPackages()
        {
            var url = GetGistUrl("d290ee31516845f42577f70367434ba8/raw/d3009b0885b9a490236e2f3a8365759b63c41c0b/unity-xr-packages.txt");
            var contents = await GetContents(url);
            var packageList = GetPackageNames(contents);
            Debug.Log(contents);
            Debug.Log(packageList);

            foreach (var package in packageList)
            {
                Debug.Log(package);
                InstallPackage(package);
            }
        }
        
        public static void InstallPackage(string package)
        {
            UnityEditor.PackageManager.Client.Add($"com.unity.{package}");
        }

        static string GetGistUrl(string id, string user = "Caleb-Mantey") => $"https://gist.githubusercontent.com/{user}/{id}";

        static async Task<string> GetContents(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }

        static string[] GetPackageNames(string names)
        {
            return names.Split('\n');
        }
    }
}
