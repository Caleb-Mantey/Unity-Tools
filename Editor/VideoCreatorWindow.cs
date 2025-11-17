using UnityEngine;
using UnityEditor;

namespace Relu.Tools {
    public class VideoCreatorWindow : EditorWindow {
        int width = 1920;
        int height = 1080;
        CreateVideoPlayer.VideoPrimitive primitive = CreateVideoPlayer.VideoPrimitive.Plane;
        float visualSize = 1f;
        bool attachVideoEndLoad = true;
        int presetIndex = 0;

        [MenuItem("Relu/Create/Video Player...", priority = 9)]
        public static void ShowWindow() {
            var wnd = GetWindow<VideoCreatorWindow>(true, "Create Video Player", true);
            wnd.minSize = new Vector2(320, 200);
            wnd.Show();
        }

        void OnGUI() {
            GUILayout.Label("Video Creation Options", EditorStyles.boldLabel);

            string[] presets = new string[] { "HD (1920x1080)", "2K (2048x1080)", "4K (3840x2160)", "Custom" };
            int newPreset = EditorGUILayout.Popup("Resolution Preset", presetIndex, presets);
            if (newPreset != presetIndex) {
                presetIndex = newPreset;
                switch (presetIndex) {
                    case 0: width = 1920; height = 1080; break;
                    case 1: width = 2048; height = 1080; break;
                    case 2: width = 3840; height = 2160; break;
                    case 3: break;
                }
            }

            EditorGUILayout.BeginHorizontal();
            width = EditorGUILayout.IntField("Width", Mathf.Max(4, width));
            height = EditorGUILayout.IntField("Height", Mathf.Max(4, height));
            EditorGUILayout.EndHorizontal();

            primitive = (CreateVideoPlayer.VideoPrimitive)EditorGUILayout.EnumPopup("Primitive", primitive);
            visualSize = EditorGUILayout.FloatField("Visual Size", Mathf.Max(0.01f, visualSize));
            attachVideoEndLoad = EditorGUILayout.Toggle("Attach VideoEndLoadScene", attachVideoEndLoad);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create")) {
                CreateVideoPlayer.CreateWithOptions(width, height, primitive, visualSize, attachVideoEndLoad);
                Close();
            }
            if (GUILayout.Button("Create 360")) {
                CreateVideoPlayer.Create360WithOptions(width, height, primitive, visualSize * 25f, attachVideoEndLoad);
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

