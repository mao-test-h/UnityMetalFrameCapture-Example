using UnityEngine;

namespace MetalFrameCapture
{
    public static class MetalFrameCaptureFactory
    {
        public static INativeProxy Create()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new NativeProxyForIOS();
#elif UNITY_EDITOR
            return new NativeProxyForEditor();
#else
            return new NativeProxyForEditor(); // Use Editor implementation as dummy for unsupported platforms
#endif
        }

        public static INativeProxy Create(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return new NativeProxyForIOS();
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                    return new NativeProxyForEditor();
                default:
                    return new NativeProxyForEditor(); // Use Editor implementation as dummy
            }
        }
    }
}
