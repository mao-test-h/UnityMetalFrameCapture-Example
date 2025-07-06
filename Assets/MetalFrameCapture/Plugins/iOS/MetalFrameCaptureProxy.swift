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
    func registerOnGpuCaptureComplete(_ delegate: @escaping () -> Void)
}

public final class MetalFrameCaptureAPI {
    static var delegate: MetalFrameCaptureDelegate? = nil
    public static func register(with delegate: MetalFrameCaptureDelegate) {
        MetalFrameCaptureAPI.delegate = delegate
    }
}

@_cdecl("metalFrameCapture_startGpuCapture")
public func startGpuCapture(filePath: UnsafePointer<CChar>) -> Int8 {
    let filePathString = String(cString: filePath)
    let ret = MetalFrameCapturePlugin.shared.startGpuCapture(filePath: filePathString)
    return ret ? 1 : 0
}

@_cdecl("metalFrameCapture_stopGpuCaptureDirect")
public func stopGpuCaptureDirect() {
    MetalFrameCapturePlugin.shared.stopGpuCaptureDirect()
}

public typealias OnGpuCaptureComplete = @convention(c) () -> Void
@_cdecl("metalFrameCapture_registerOnGpuCaptureComplete")
func registerOnGpuCaptureComplete(_ delegate: @escaping OnGpuCaptureComplete) {
    MetalFrameCaptureAPI.delegate!.registerOnGpuCaptureComplete(delegate)
}
