using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Relu.Utils {
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoRenderTargetBinder : MonoBehaviour {
        public bool is360 = false;

        VideoPlayer vp;
        Renderer parentRenderer;

        void Awake() {
            vp = GetComponent<VideoPlayer>();
            parentRenderer = GetComponentInParent<Renderer>();
        }

        void OnEnable() {
            if (vp == null) vp = GetComponent<VideoPlayer>();
            vp.prepareCompleted += OnPrepared;
            // If already prepared, set up immediately
            if (vp.isPrepared) {
                SetupRenderTarget();
            } else {
                // Try to prepare if not prepared
                try { vp.Prepare(); } catch { }
                StartCoroutine(WaitForTexture());
            }
        }

        void OnDisable() {
            if (vp != null) vp.prepareCompleted -= OnPrepared;
        }

        IEnumerator WaitForTexture() {
            float timeout = 5f;
            float t = 0f;
            while (vp.texture == null && t < timeout) {
                t += Time.deltaTime;
                yield return null;
            }
            SetupRenderTarget();
        }

        void OnPrepared(VideoPlayer source) {
            SetupRenderTarget();
        }

        void SetupRenderTarget() {
            if (vp == null) return;
            Texture src = vp.texture;
            if (src == null) {
                // nothing to do
                return;
            }

            try {
                if (is360) CreateCubeFromSource(src);
                else Create2DFromSource(src);
            } catch (System.Exception ex) {
                Debug.LogError("VideoRenderTargetBinder: failed to create render target - " + ex.Message);
            }
        }

        void Create2DFromSource(Texture src) {
            int w = Mathf.Max(16, src.width);
            int h = Mathf.Max(16, src.height);
            var rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
            rt.Create();

            vp.targetTexture = rt;
            if (parentRenderer != null) {
                parentRenderer.sharedMaterial = Instantiate(parentRenderer.sharedMaterial);
                parentRenderer.sharedMaterial.mainTexture = rt;
            }
        }

        void CreateCubeFromSource(Texture src) {
            // For equirectangular source, determine face size from source height (face ~ height/2)
            int srcH = Mathf.Max(16, src.height);
            int faceSize = Mathf.Max(16, srcH / 2);
            faceSize = Mathf.NextPowerOfTwo(faceSize);

            var rt = new RenderTexture(faceSize, faceSize, 0, RenderTextureFormat.ARGB32);
            rt.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rt.useMipMap = false;
            rt.autoGenerateMips = false;
            rt.Create();

            // Find conversion shader
            var sh = Shader.Find("Hidden/EquirectangularToCubemap");
            if (sh == null) {
                Debug.LogError("VideoRenderTargetBinder: missing shader 'Hidden/EquirectangularToCubemap'. Cannot convert equirectangular video to cubemap.");
                return;
            }

            var mat = new Material(sh);
            mat.SetTexture("_MainTex", src);

            // Render each cubemap face
            for (int face = 0; face < 6; face++) {
                Graphics.SetRenderTarget(rt, 0, (CubemapFace)face);
                GL.Clear(true, true, Color.black);
                mat.SetInt("_FaceIndex", face);
                Graphics.Blit(src, mat);
            }

            // restore
            Graphics.SetRenderTarget(null);

            vp.targetTexture = rt;
            if (parentRenderer != null) {
                parentRenderer.sharedMaterial = Instantiate(parentRenderer.sharedMaterial);
                parentRenderer.sharedMaterial.mainTexture = rt;
            }
        }
    }
}

