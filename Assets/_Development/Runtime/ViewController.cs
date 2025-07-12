using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MetalFrameCapture.URP;
using NativeShare;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace MetalFrameCapture.Development
{
    public sealed class ViewController : MonoBehaviour, INativeProxyDelegate
    {
        [SerializeField] private Camera baseCamera;
        [SerializeField] private Button startCaptureButton;
        [SerializeField] private Button startCaptureStop;

        private MetalFrameCaptureRenderFeature _renderFeature;

        private INativeProxy _nativeProxy;
        private INativeShare _nativeShare;
        private string _latestFilePath;

        private void Start()
        {
            _nativeShare = NativeShareFactory.Create();
            _nativeProxy = MetalFrameCaptureFactory.Create();
            _nativeProxy.Delegate = this;

            _renderFeature = GetRendererFeatures<MetalFrameCaptureRenderFeature>(baseCamera).First();
            _renderFeature.SetNativeProxy(_nativeProxy);

            startCaptureButton.onClick.AddListener(() =>
            {
                _latestFilePath = GetFilePath();
                _renderFeature.SetFileName(_latestFilePath);
                _renderFeature.SetCaptureEnabled(true);
            });

            startCaptureStop.onClick.AddListener(() =>
            {
                //_nativeProxy.StopGpuCapture();
            });
        }

        private void OnDestroy()
        {
            _renderFeature.SetCaptureEnabled(false);
        }

        private static string GetFilePath()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"GpuCapture_{timestamp}.gputrace";
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            return filePath;
        }

        public void OnGpuCaptureFailed(string errorMessage)
        {
            Debug.LogError($"GPU capture failed: {errorMessage}");
            _renderFeature.SetCaptureEnabled(false);
        }

        public void OnGpuCaptureComplete()
        {
            var filePath = _latestFilePath;
            Debug.Log($"GPU capture completed: {filePath}");
            _nativeShare.ShareFile(filePath);
            _renderFeature.SetCaptureEnabled(false);
        }

        private static List<T> GetRendererFeatures<T>(Camera baseCamera) where T : ScriptableRendererFeature
        {
            var rendererFeatures = new List<T>();
            var baseCameraData = baseCamera.GetUniversalAdditionalCameraData();

            var cameras = new List<Camera> { baseCamera };
            cameras.AddRange(baseCameraData.cameraStack);

            var propertyInfo = typeof(ScriptableRenderer)
                .GetField("m_RendererFeatures", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(propertyInfo);

            for (int ic = 0, nc = cameras.Count; ic < nc; ic++)
            {
                var cameraData = cameras[ic].GetUniversalAdditionalCameraData();
                var cameraRendererFeatures = propertyInfo.GetValue(cameraData.scriptableRenderer) as List<ScriptableRendererFeature>;
                if (cameraRendererFeatures == null)
                {
                    continue;
                }

                for (int ir = 0, nr = cameraRendererFeatures.Count; ir < nr; ir++)
                {
                    var rendererFeature = cameraRendererFeatures[ir] as T;
                    if (rendererFeature != null)
                    {
                        rendererFeatures.Add(rendererFeature);
                    }
                }
            }

            return rendererFeatures;
        }
    }
}
