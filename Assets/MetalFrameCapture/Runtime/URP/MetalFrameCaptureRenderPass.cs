using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace MetalFrameCapture.URP
{
    public sealed class MetalFrameCaptureStartPass : ScriptableRenderPass
    {
        private sealed class PassData
        {
        }

        private readonly INativeProxy _nativeProxy;
        private string _fileName;

        public MetalFrameCaptureStartPass(INativeProxy nativeProxy)
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
                _nativeProxy.StartGpuCapture(_fileName, cmd);
            });
        }
    }

    public class MetalFrameCaptureStopPass : ScriptableRenderPass
    {
        private sealed class PassData
        {
        }

        private readonly INativeProxy _nativeProxy;

        public MetalFrameCaptureStopPass(INativeProxy nativeProxy)
        {
            renderPassEvent = RenderPassEvent.AfterRendering;
            _nativeProxy = nativeProxy;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using var builder = renderGraph.AddUnsafePass("MetalFrameCapture_Stop", out PassData _, profilingSampler);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData _, UnsafeGraphContext graphContext) =>
            {
                var cmd = CommandBufferHelpers.GetNativeCommandBuffer(graphContext.cmd);
                _nativeProxy.StopGpuCapture(cmd);
            });
        }
    }
}
