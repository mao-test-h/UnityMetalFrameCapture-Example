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
            StopGpuCapture = 0,
        }

        private delegate void OnCompleteCallback();

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

        public bool StartGpuCapture(string filePath)
        {
            return NativeMethod(filePath) == 1;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_startGpuCapture")]
            static extern byte NativeMethod(string filePath);
        }

        public void StopGpuCapture()
        {
            CallRenderEventFunc(EventType.StopGpuCapture);
        }

        public void StopGpuCaptureDirect()
        {
            NativeMethod();
            return;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_stopGpuCaptureDirect")]
            static extern byte NativeMethod();
        }

        private void RegisterOnGpuCaptureComplete()
        {
            NativeMethod(OnCallback);
            return;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_registerOnGpuCaptureComplete")]
            static extern void NativeMethod(OnCompleteCallback callback);

            [MonoPInvokeCallback(typeof(OnCompleteCallback))]
            static void OnCallback()
            {
                _delegate?.OnGpuCaptureComplete();
            }
        }

        private static void CallRenderEventFunc(EventType eventType)
        {
            GL.IssuePluginEvent(NativeMethod(), (int)eventType);
            return;

            [DllImport("__Internal", EntryPoint = "metalFrameCapture_getRenderEventFunc")]
            static extern IntPtr NativeMethod();
        }
    }
}
