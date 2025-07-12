import Foundation

/// プラグインの初期化
/// NOTE: `OnGraphicsDeviceEvent -> kUnityGfxDeviceEventInitialize`のタイミングで呼び出される
@_cdecl("metalFrameCapture_onUnityGfxDeviceEventInitialize")
func onUnityGfxDeviceEventInitialize() {
    let unityMetal = UnityGraphicsBridge.getUnityGraphicsMetal().pointee
    MetalFrameCapturePlugin.shared = MetalFrameCapturePlugin(with: unityMetal)
}

/// Unity側から GL.IssuePluginEvent を呼ぶとレンダリングスレッドから呼び出されるメソッド
@_cdecl("metalFrameCapture_onRenderEvent")
func onRenderEvent(eventID: Int32) {
    MetalFrameCapturePlugin.shared.onRenderEvent(eventType: eventID)
}

// P/Invoke

public protocol MetalFrameCaptureDelegate {
    func registerOnGpuCaptureFiled(_ delegate: @escaping (UnsafePointer<CChar>) -> Void)
    func registerOnGpuCaptureComplete(_ delegate: @escaping () -> Void)
}

public final class MetalFrameCaptureAPI {
    static var delegate: MetalFrameCaptureDelegate? = nil
    public static func register(with delegate: MetalFrameCaptureDelegate) {
        MetalFrameCaptureAPI.delegate = delegate
    }
}

@_cdecl("metalFrameCapture_setFilePath")
public func metalFrameCapture_setFilePath(filePath: UnsafePointer<CChar>) {
    let filePathString = String(cString: filePath)
    MetalFrameCapturePlugin.shared.setFilePath(filePathString)
}

public typealias OnGpuCaptureFiled = @convention(c) (UnsafePointer<CChar>) -> Void
public typealias OnGpuCaptureComplete = @convention(c) () -> Void

@_cdecl("metalFrameCapture_registerDelegate")
func metalFrameCapture_registerDelegate(_ onFiled: @escaping OnGpuCaptureFiled, _ onComplete: @escaping OnGpuCaptureComplete) {
    MetalFrameCaptureAPI.delegate!.registerOnGpuCaptureFiled(onFiled)
    MetalFrameCaptureAPI.delegate!.registerOnGpuCaptureComplete(onComplete)
}
