#if UNITY_IOS
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace MetalFrameCapture.Editor
{
    internal static class XcodePostProcess
    {
        [PostProcessBuild]
        private static void OnPostProcessBuild(BuildTarget target, string xcodeprojPath)
        {
            if (target != BuildTarget.iOS) return;

            var pbxProjectPath = PBXProject.GetPBXProjectPath(xcodeprojPath);
            var pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(pbxProjectPath));

            // Low-Level Native Plugin Interface にて Swift から Metal API にアクセスできるようにするための設定
            ReplaceNativeSources(xcodeprojPath);
            SetPublicHeader(ref pbxProject);

            // 検証用に GPU FrameCapture は常に有効にしておく
            EnableMetalFrameCapture(xcodeprojPath);

            File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());
        }

        private static void ReplaceNativeSources(string xcodeprojPath)
        {
            var frameworkPath = $"{xcodeprojPath}/UnityFramework/UnityFramework.h";
            var lines = File.ReadAllLines(frameworkPath).ToList();
            lines.Insert(0, "#import <UnityFramework/IUnityGraphicsMetal.h>");
            lines.Add("");
            lines.Add("// NOTE: Low-Level Native Plugin Interface にて Swift から Metal API にアクセスするために追加");
            lines.Add("__attribute__ ((visibility(\"default\")))");
            lines.Add("@interface UnityGraphicsBridge : NSObject {}");
            lines.Add("+ (IUnityGraphicsMetalV2*)getUnityGraphicsMetal;");
            lines.Add("@end");
            File.WriteAllLines(frameworkPath, lines);
        }

        private static void SetPublicHeader(ref PBXProject pbxProject)
        {
            // iOSビルド結果にある以下のヘッダーはpublicとして設定し直す
            const string sourcesDirectory = "Classes/Unity/";
            var sources = new[]
            {
                "IUnityInterface.h",
                "IUnityGraphicsMetal.h",
                "IUnityGraphics.h",
            };

            var frameworkGuid = pbxProject.GetUnityFrameworkTargetGuid();
            foreach (var source in sources)
            {
                var sourceGuid = pbxProject.FindFileGuidByProjectPath(sourcesDirectory + source);
                pbxProject.AddPublicHeaderToBuild(frameworkGuid, sourceGuid);
            }
        }

        private static void EnableMetalFrameCapture(in string xcodeprojPath)
        {
            // Scheme 側の有効化
            {
                var schemePath = $"{xcodeprojPath}/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
                var xcScheme = new XcScheme();
                xcScheme.ReadFromFile(schemePath);
                xcScheme.SetFrameCaptureModeOnRun(XcScheme.FrameCaptureMode.Metal);
                xcScheme.SetDebugExecutable(true);
                xcScheme.WriteToFile(schemePath);
            }

            // Info.plist 側の有効化
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
