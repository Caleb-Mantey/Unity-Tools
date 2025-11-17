using UnityEngine;
using UnityEditor;
using UnityEngine.Video;
using Relu.Utils;

namespace Relu.Tools
{
    // Adds "Relu/Create/Video Player" and "Relu/Create/360 Video" menu items
    public static class CreateVideoPlayer
    {
        public static void Create()
        {
            // Create a screen (plane) that will show the video
            GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Plane);
            screen.name = "Video Screen (Relu)";
            Undo.RegisterCreatedObjectUndo(screen, "Create Video Screen");

            // Create assets under Assets/Relu/VideoAssets so materials and RTs persist
            int width = 1920;
            int height = 1080;
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.name = "VideoRenderTexture (Relu)";
            // default 2D; if caller wants cube it will be overridden later by the binder

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.name = "VideoMaterial (Relu)";
            mat.mainTexture = rt;

            // Ensure target folder exists
            string parentFolder = "Assets/Relu";
            if (!AssetDatabase.IsValidFolder(parentFolder)) AssetDatabase.CreateFolder("Assets", "Relu");
            string assetFolder = parentFolder + "/VideoAssets";
            if (!AssetDatabase.IsValidFolder(assetFolder)) AssetDatabase.CreateFolder(parentFolder, "VideoAssets");

            // Create persistent assets (store the initial RT asset)
            string rtPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + rt.name + ".renderTexture");
            AssetDatabase.CreateAsset(rt, rtPath);
            string matPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + mat.name + ".mat");
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            mat.mainTexture = rt;

            var renderer = screen.GetComponent<Renderer>();
            renderer.sharedMaterial = mat;

            // Create the Video Player object and parent it to the screen
            GameObject videoGO = new GameObject("Video Player (Relu)");
            Undo.RegisterCreatedObjectUndo(videoGO, "Create Video Player");
            videoGO.transform.SetParent(screen.transform, false);
            videoGO.transform.localPosition = Vector3.zero;
            var vp = videoGO.AddComponent<VideoPlayer>();
            vp.playOnAwake = true;
            vp.renderMode = VideoRenderMode.RenderTexture;
            // don't assign targetTexture yet for 360 — binder will create correctly sized RT at runtime
            vp.targetTexture = rt;
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;

            // Add an AudioSource so audio can be routed if the user assigns audio tracks
            var audio = videoGO.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            vp.SetTargetAudioSource(0, audio);

            // Attach the VideoEndLoadScene component so the video can trigger scene loading when it ends
            Undo.AddComponent<VideoEndLoadScene>(videoGO);

            // Attach a runtime binder that will create a properly sized RenderTexture at runtime and assign it
            var binder = Undo.AddComponent<VideoRenderTargetBinder>(videoGO);
            binder.is360 = false; // plane default

            // Adjust the plane size to match the RenderTexture aspect ratio
            // Plane default size is 10x10 units. To get the correct aspect we scale X (width) and Z (height) accordingly.
            float aspect = (float)width / height;
            // To make the plane a vertically oriented screen facing forward, rotate it so its normal points along +Z
            screen.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            // Make the visual height = 1 unit (so scaleZ = 0.1 because default plane is 10 units), and width = aspect * height
            float planeUnit = 0.1f; // 1 unit visual height => 0.1 local scale because plane is 10 units
            screen.transform.localScale = new Vector3(aspect * planeUnit, 1f, planeUnit);

            // Select the created screen in the editor
            Selection.activeGameObject = screen;
        }

        public static void Create360()
        {
            // Create a cube that will act as the inward-facing 360 container
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "360 Video Cube (Relu)";
            Undo.RegisterCreatedObjectUndo(cube, "Create 360 Video Cube");

            // Create material and RenderTexture as assets in Assets/Relu/VideoAssets
            int width = 4096; // default high-res for 360
            int height = 2048;
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.name = "360VideoRenderTexture (Relu)";

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.name = "360VideoMaterial (Relu)";
            mat.mainTexture = rt;

            string parentFolder = "Assets/Relu";
            if (!AssetDatabase.IsValidFolder(parentFolder)) AssetDatabase.CreateFolder("Assets", "Relu");
            string assetFolder = parentFolder + "/VideoAssets";
            if (!AssetDatabase.IsValidFolder(assetFolder)) AssetDatabase.CreateFolder(parentFolder, "VideoAssets");

            string rtPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + rt.name + ".rendertexture");
            AssetDatabase.CreateAsset(rt, rtPath);
            string matPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + mat.name + ".mat");
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            mat.mainTexture = rt;

            var renderer = cube.GetComponent<Renderer>();
            renderer.sharedMaterial = mat;

            // Create the Video Player object and parent it to the cube
            GameObject videoGO = new GameObject("Video Player (Relu)");
            Undo.RegisterCreatedObjectUndo(videoGO, "Create Video Player");
            videoGO.transform.SetParent(cube.transform, false);
            videoGO.transform.localPosition = Vector3.zero;

            var vp = videoGO.AddComponent<VideoPlayer>();
            vp.playOnAwake = true;
            vp.renderMode = VideoRenderMode.RenderTexture;
            vp.targetTexture = rt;
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;

            var audio = videoGO.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            vp.SetTargetAudioSource(0, audio);

            // Attach VideoEndLoadScene
            Undo.AddComponent<VideoEndLoadScene>(videoGO);

            // Attach the binder and mark it as 360 so it will create a cube RT at runtime sized from the video
            var binder360 = Undo.AddComponent<VideoRenderTargetBinder>(videoGO);
            binder360.is360 = true;

            // Scale the cube large and invert one axis so its normals point inward (so the video texture is visible from inside)
            float cubeSize = 50f;
            cube.transform.localScale = new Vector3(-cubeSize, cubeSize, cubeSize); // negative X flips normals

            // Ensure the cube sits centered at origin
            cube.transform.position = Vector3.zero;

            // Select the created cube in the editor
            Selection.activeGameObject = cube;
        }

        // Add helper creation APIs so the standalone EditorWindow can call them
        public enum VideoPrimitive { Plane, Quad, Sphere, Cube }

        // Public helper used by the EditorWindow to create video objects with options
        public static void CreateWithOptions(int width, int height, VideoPrimitive primitive, float visualSize = 1f, bool attachVideoEndLoad = true) {
            // Create visual primitive
            GameObject screen = null;
            switch (primitive) {
                case VideoPrimitive.Plane:
                    screen = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    break;
                case VideoPrimitive.Quad:
                    screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    break;
                case VideoPrimitive.Sphere:
                    screen = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                case VideoPrimitive.Cube:
                    screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
            }

            if (screen == null) return;

            screen.name = (primitive == VideoPrimitive.Sphere || primitive == VideoPrimitive.Cube) ? "360 Video Container (Relu)" : "Video Screen (Relu)";
            Undo.RegisterCreatedObjectUndo(screen, "Create Video Screen");

            // Material + RenderTexture (create as assets in Assets/Relu/VideoAssets)
            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.name = "VideoMaterial (Relu)";
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.name = "VideoRenderTexture (Relu)";
            mat.mainTexture = rt;

            string parentFolder = "Assets/Relu";
            if (!AssetDatabase.IsValidFolder(parentFolder)) AssetDatabase.CreateFolder("Assets", "Relu");
            string assetFolder = parentFolder + "/VideoAssets";
            if (!AssetDatabase.IsValidFolder(assetFolder)) AssetDatabase.CreateFolder(parentFolder, "VideoAssets");

            string rtPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + rt.name + ".rendertexture");
            AssetDatabase.CreateAsset(rt, rtPath);
            string matPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + mat.name + ".mat");
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // If we're creating a sphere (360), make the material render the inside by culling front faces
            if (primitive == VideoPrimitive.Sphere) {
                // Some shaders expose a _Cull property; set it to Front to render backfaces (visible from inside)
                // Fallback: if the shader doesn't respond to _Cull, this call is harmless.
                mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Front);
            }

            var renderer = screen.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = mat;

            // Video player child
            GameObject videoGO = new GameObject("Video Player (Relu)");
            Undo.RegisterCreatedObjectUndo(videoGO, "Create Video Player");
            videoGO.transform.SetParent(screen.transform, false);
            videoGO.transform.localPosition = Vector3.zero;

            var vp = videoGO.AddComponent<VideoPlayer>();
            vp.playOnAwake = true;
            vp.renderMode = VideoRenderMode.RenderTexture;
            vp.targetTexture = rt;
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;

            var audio = videoGO.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            vp.SetTargetAudioSource(0, audio);

            if (attachVideoEndLoad) Undo.AddComponent<VideoEndLoadScene>(videoGO);

            // Layout adjustments
            if (primitive == VideoPrimitive.Plane) {
                float aspect = (float)width / height;
                screen.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                float planeUnit = visualSize / 10f; // plane default size is 10
                screen.transform.localScale = new Vector3(aspect * planeUnit, 1f, planeUnit);
            } else if (primitive == VideoPrimitive.Quad) {
                float aspect = (float)width / height;
                screen.transform.rotation = Quaternion.identity;
                screen.transform.localScale = new Vector3(aspect * visualSize, visualSize, 1f);
            } else if (primitive == VideoPrimitive.Sphere) {
                // For inward-facing sphere we use a material that culls front faces so the interior is visible
                float size = Mathf.Max(10f, visualSize * 10f);
                screen.transform.localScale = new Vector3(size, size, size);
                screen.transform.position = Vector3.zero;
            } else if (primitive == VideoPrimitive.Cube) {
                float size = Mathf.Max(10f, visualSize * 10f);
                // Cube faces: keep negative X for cube to invert normals (simple approach); consider using inverted mesh for better results
                screen.transform.localScale = new Vector3(-size, size, size);
                screen.transform.position = Vector3.zero;
            }

            // Attach binder and set is360 depending on primitive
            var binderComp = screen.GetComponentInChildren<VideoRenderTargetBinder>();
            if (binderComp != null) binderComp.is360 = (primitive == VideoPrimitive.Sphere || primitive == VideoPrimitive.Cube);

            Selection.activeGameObject = screen;
        }

        public static void Create360WithOptions(int width, int height, VideoPrimitive primitive, float visualSize = 25f, bool attachVideoEndLoad = true) {
            VideoPrimitive use = (primitive == VideoPrimitive.Sphere || primitive == VideoPrimitive.Cube) ? primitive : VideoPrimitive.Sphere;
            CreateWithOptions(width, height, use, visualSize, attachVideoEndLoad);
        }
    }
}
