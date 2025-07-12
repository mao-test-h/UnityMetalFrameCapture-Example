using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MetalFrameCapture.URP
{
    public sealed class MetalFrameCaptureRenderFeature : ScriptableRendererFeature
    {
        [Serializable]
        public sealed class Settings
        {
            public bool captureEnabled;
        }

        [SerializeField] private Settings settings = new Settings();

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
            settings.captureEnabled = enabled;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.captureEnabled == false && _startPass == null && _stopPass == null) return;
            renderer.EnqueuePass(_startPass);
            renderer.EnqueuePass(_stopPass);
        }
    }
}
