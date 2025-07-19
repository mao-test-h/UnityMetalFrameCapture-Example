using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

namespace MetalFrameCapture.URP
{
    public sealed class MetalFrameCaptureRenderFeature : ScriptableRendererFeature
    {
        private MetalFrameCapturePass _pass;
        private bool _captureEnabled;

        public override void Create()
        {
            // do nothing
        }

        public void SetNativeProxy(INativeProxy nativeProxy)
        {
            _pass = new MetalFrameCapturePass(nativeProxy);
        }

        public void SetCaptureEnabled(bool enabled)
        {
            _captureEnabled = enabled;
        }

        public void SetFileName(string fileName)
        {
            Assert.IsNotNull(_pass);
            _pass.SetFileName(fileName);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_captureEnabled == false || _pass == null) return;
            renderer.EnqueuePass(_pass);
        }
    }
}
