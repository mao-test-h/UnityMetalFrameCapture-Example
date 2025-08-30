#if UNITY_IOS
using System.IO;
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
            // Scheme 側の有効化
            // ref: https://docs.unity3d.com/ScriptReference/iOS.Xcode.XcScheme.html
            {
                var schemePath = $"{xcodeprojPath}/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
                var xcScheme = new XcScheme();
                xcScheme.ReadFromFile(schemePath);
                xcScheme.SetFrameCaptureModeOnRun(XcScheme.FrameCaptureMode.Metal);
                xcScheme.SetDebugExecutable(true);
                xcScheme.WriteToFile(schemePath);
            }

            // Info.plist 側の有効化
            // ref: http://xcodedeveloper.apple.com/documentation/xcode/capturing-a-metal-workload-programmatically
            {
                var plistPath = Path.Combine(xcodeprojPath, "Info.plist");
                var plist = new PlistDocument();
                plist.ReadFromFile(plistPath);
                var root = plist.root;
                root.SetBoolean("MetalCaptureEnabled", true);
                plist.WriteToFile(plistPath);
            }
        }
    }
}
#endif
