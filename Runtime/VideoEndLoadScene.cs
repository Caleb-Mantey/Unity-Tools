using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

namespace Relu.Utils {
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoEndLoadScene : MonoBehaviour {

        public enum SceneReferenceType { ByBuildIndex, ByName }

        [Header("Load on Video End")]
        [Tooltip("If enabled, the scene will be loaded when the video finishes.")]
        [SerializeField] private bool loadOnVideoEnd = true;

        [Tooltip("Choose whether to reference the scene by build index or by name.")]
        [SerializeField] private SceneReferenceType referenceType = SceneReferenceType.ByBuildIndex;

        [Tooltip("Build index of the scene to load (used when reference type is ByBuildIndex).")]
        [SerializeField] private int sceneBuildIndex = 0;

        [Tooltip("Name of the scene to load (used when reference type is ByName). Must match the scene name in Build Settings.")]
        [SerializeField] private string sceneName = "";

        [Tooltip("Enable to print debug messages when the video ends and when attempting to load the scene.")]
        [SerializeField] private bool debugLog = true;

        VideoPlayer videoPlayer;

        void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        void OnEnable() {
            if (videoPlayer != null)
                videoPlayer.loopPointReached += OnVideoEnd;
        }

        void OnDisable() {
            if (videoPlayer != null)
                videoPlayer.loopPointReached -= OnVideoEnd;
        }

        private void OnVideoEnd(VideoPlayer source) {
            if (!loadOnVideoEnd)
                return;

            int buildIndex = -1;

            if (referenceType == SceneReferenceType.ByBuildIndex) {
                buildIndex = sceneBuildIndex;
            } else {
                buildIndex = GetBuildIndexBySceneName(sceneName);
                if (buildIndex == -1) {
                    if (debugLog) Debug.LogError($"VideoEndLoadScene: Scene '{sceneName}' not found in Build Settings.");
                    return;
                }
            }

            if (debugLog) Debug.Log($"VideoEndLoadScene: Loading scene build index {buildIndex} using LoadingScreenManager.");

            // Use the existing LoadingScreenManager API which accepts a build index
            LoadingScreenManager.LoadScene(buildIndex);
        }

        private int GetBuildIndexBySceneName(string name) {
            if (string.IsNullOrEmpty(name))
                return -1;

            int scenes = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < scenes; i++) {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName == name)
                    return i;
            }

            return -1;
        }
    }
}

