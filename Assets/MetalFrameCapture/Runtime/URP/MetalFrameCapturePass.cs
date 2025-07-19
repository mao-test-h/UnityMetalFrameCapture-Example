using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace MetalFrameCapture.URP
{
    public sealed class MetalFrameCapturePass : ScriptableRenderPass
    {
        private sealed class PassData
        {
        }

        private readonly INativeProxy _nativeProxy;
        private string _fileName;
        private bool _isCapture;

        public MetalFrameCapturePass(INativeProxy nativeProxy)
        {
            renderPassEvent = RenderPassEvent.BeforeRendering;
            _nativeProxy = nativeProxy;
        }

        public void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using var builder = renderGraph.AddUnsafePass("MetalFrameCapture_Start", out PassData _, profilingSampler);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData _, UnsafeGraphContext graphContext) =>
            {
                var cmd = CommandBufferHelpers.GetNativeCommandBuffer(graphContext.cmd);
                if (_isCapture)
                {
                    _nativeProxy.StopGpuCapture(cmd);
                    _isCapture = false;
                }
                else
                {
                    _nativeProxy.StartGpuCapture(_fileName, cmd);
                    _isCapture = true;
                }
            });
        }
    }
}
