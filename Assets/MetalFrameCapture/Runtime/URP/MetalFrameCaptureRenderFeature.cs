using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

namespace MetalFrameCapture.URP
{
    public sealed class MetalFrameCaptureRenderFeature : ScriptableRendererFeature
    {
        private bool _captureEnabled = false;

        private MetalFrameCaptureStartPass _startPass;
        private MetalFrameCaptureStopPass _stopPass;

        public override void Create()
        {
            // do nothing
        }

        public void SetNativeProxy(INativeProxy nativeProxy)
        {
            _startPass = new MetalFrameCaptureStartPass(nativeProxy);
            _stopPass = new MetalFrameCaptureStopPass(nativeProxy);
        }

        public void SetCaptureEnabled(bool enabled)
        {
            _captureEnabled = enabled;
        }

        public void SetFileName(string fileName)
        {
            Assert.IsNotNull(_startPass);
            _startPass.SetFileName(fileName);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_captureEnabled == false || _startPass == null && _stopPass == null) return;
            renderer.EnqueuePass(_startPass);
            renderer.EnqueuePass(_stopPass);
        }
    }
}
