using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace MetalFrameCapture
{
    public sealed class NativeProxyForIOS : INativeProxy
    {
        private enum EventType
        {
            StartGpuCapture = 0,
            StopGpuCapture,
        }

        public INativeProxyDelegate Delegate
        {
            get => _delegate;
            set => _delegate = value;
        }

        private static INativeProxyDelegate _delegate;

        public NativeProxyForIOS()
        {
            RegisterOnGpuCaptureComplete();
        }

        public void StartGpuCapture(string filePath)
        {
            NativeMethod(filePath);
            CallRenderEventFunc(EventType.StartGpuCapture);
            return;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_setFilePath")]
            static extern void NativeMethod(string filePath);
        }

        public void StopGpuCapture()
        {
            CallRenderEventFunc(EventType.StopGpuCapture);
        }

        private static void CallRenderEventFunc(EventType eventType)
        {
            GL.IssuePluginEvent(NativeMethod(), (int)eventType);
            return;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_getRenderEventFunc")]
            static extern IntPtr NativeMethod();
        }

        private delegate void OnFiledCallback(string errorMessage);

        private delegate void OnCompleteCallback();

        private static void RegisterOnGpuCaptureComplete()
        {
            NativeMethod(OnFiled, OnComplete);
            return;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_registerDelegate")]
            static extern void NativeMethod(OnFiledCallback onFiledCallback, OnCompleteCallback onCompleteCallback);

            [MonoPInvokeCallback(typeof(OnFiledCallback))]
            static void OnFiled(string errorMessage)
            {
                _delegate?.OnGpuCaptureFailed(errorMessage);
            }

            [MonoPInvokeCallback(typeof(OnCompleteCallback))]
            static void OnComplete()
            {
                _delegate?.OnGpuCaptureComplete();
            }
        }
    }
}
