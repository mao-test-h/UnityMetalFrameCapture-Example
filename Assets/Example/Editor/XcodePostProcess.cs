#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Example.Editor
{
    internal static class XcodePostProcess
    {
        [PostProcessBuild]
        private static void OnPostProcessBuild(BuildTarget target, string xcodeprojPath)
        {
            if (target != BuildTarget.iOS) return;

            EnableMetalFrameCapture(xcodeprojPath);
        }

        private static void EnableMetalFrameCapture(in string xcodeprojPath)
        {
            // ref: https://docs.unity3d.com/ScriptReference/iOS.Xcode.XcScheme.html
            var schemePath = $"{xcodeprojPath}/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
            var xcScheme = new XcScheme();
            xcScheme.ReadFromFile(schemePath);
            xcScheme.SetFrameCaptureModeOnRun(XcScheme.FrameCaptureMode.Metal);
            xcScheme.SetDebugExecutable(true);
            xcScheme.WriteToFile(schemePath);
        }
    }
}
#endif
