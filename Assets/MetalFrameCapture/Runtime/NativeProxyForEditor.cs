namespace MetalFrameCapture
{
    public sealed class NativeProxyForEditor : INativeProxy
    {
        public INativeProxyDelegate Delegate { get; set; }

        public bool StartGpuCapture(string filePath)
        {
            // do nothing
            return true;
        }

        public void StopGpuCapture()
        {
            // do nothing
        }

        public void StopGpuCaptureDirect()
        {
            // do nothing
        }
    }
}
