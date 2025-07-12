namespace MetalFrameCapture
{
    public sealed class NativeProxyForEditor : INativeProxy
    {
        public INativeProxyDelegate Delegate { get; set; }

        public void StartGpuCapture(string filePath)
        {
            // do nothing
        }

        public void StopGpuCapture()
        {
            // do nothing
        }
    }
}
