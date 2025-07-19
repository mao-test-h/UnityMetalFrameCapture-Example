using UnityEngine.Rendering;

namespace MetalFrameCapture
{
    public interface INativeProxy
    {
        INativeProxyDelegate Delegate { set; }
        void StartGpuCapture(string filePath, in CommandBuffer cmdBuf);
        void StopGpuCapture(in CommandBuffer cmdBuf);
    }

    public interface INativeProxyDelegate
    {
        void OnGpuCaptureFailed(string errorMessage);
        void OnGpuCaptureComplete();
    }
}
