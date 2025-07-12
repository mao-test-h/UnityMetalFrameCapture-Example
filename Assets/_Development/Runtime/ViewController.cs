using System;
using System.IO;
using NativeShare;
using UnityEngine;
using UnityEngine.UI;

namespace MetalFrameCapture.Development
{
    public sealed class ViewController : MonoBehaviour, INativeProxyDelegate
    {
        [SerializeField] private Button startCaptureButton;
        [SerializeField] private Button startCaptureStop;

        private INativeProxy _nativeProxy;
        private INativeShare _nativeShare;
        private string _latestFilePath;

        void Start()
        {
            _nativeProxy = MetalFrameCaptureFactory.Create();

            startCaptureButton.onClick.AddListener(() =>
            {
                _latestFilePath = GetFilePath();
                _nativeProxy.StartGpuCapture(_latestFilePath);
            });

            startCaptureStop.onClick.AddListener(() =>
            {
                _nativeProxy.StopGpuCapture();
            });
        }

        private static string GetFilePath()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"GpuCapture_{timestamp}.gputrace";
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            return filePath;
        }

        public void OnGpuCaptureComplete()
        {
            var filePath = _latestFilePath;
            Debug.Log($"GPU capture completed: {filePath}");
            _nativeShare.ShareFile(filePath);
        }

        public void OnGpuCaptureFailed(string errorMessage)
        {
            Debug.LogError($"GPU capture failed: {errorMessage}");
        }
    }
}
