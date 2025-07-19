using UnityEngine.Rendering;

namespace MetalFrameCapture
{
    public sealed class NativeProxyForEditor : INativeProxy
    {
        public INativeProxyDelegate Delegate { get; set; }

        public void StartGpuCapture(string filePath, in CommandBuffer cmdBuf)
        {
            // do nothing
        }

        public void StopGpuCapture(in CommandBuffer cmdBuf)
        {
            // do nothing
        }
    }
}
