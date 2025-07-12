namespace MetalFrameCapture
{
    public interface INativeProxy
    {
        INativeProxyDelegate Delegate { set; }
        void StartGpuCapture(string filePath);
        void StopGpuCapture();
    }

    public interface INativeProxyDelegate
    {
        void OnGpuCaptureFailed(string errorMessage);
        void OnGpuCaptureComplete();
    }
}
