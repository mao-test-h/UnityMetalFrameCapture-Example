namespace MetalFrameCapture
{
    public interface INativeProxy
    {
        INativeProxyDelegate Delegate { set; }
        bool StartGpuCapture(string filePath);
        void StopGpuCapture();
        void StopGpuCaptureDirect();
    }

    public interface INativeProxyDelegate
    {
        void OnGpuCaptureComplete();
    }
}
