using UnityEngine;

namespace NativeShare
{
    public static class NativeShareFactory
    {
        public static INativeShare Create()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new NativeShareIOS();
#elif UNITY_EDITOR
            return new NativeShareEditor();
#else
            return new NativeShareDummy();
#endif
        }

        public static INativeShare Create(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return new NativeShareIOS();
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    return new NativeShareEditor();
                default:
                    return new NativeShareDummy();
            }
        }
    }
}
