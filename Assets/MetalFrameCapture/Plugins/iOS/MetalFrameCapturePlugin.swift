import Foundation
import Metal

final class MetalFrameCapturePlugin {
    
    private enum EventType: Int32 {
        case startCapture = 0
        case stopCapture = 1
    }
    
    public static var shared: MetalFrameCapturePlugin! = nil
    
    private let unityMetal: IUnityGraphicsMetalV2
    
    private var filePath = ""
    private var onGpuCaptureFailed: ((String) -> Void)? = nil
    private var onGpuCaptureComplete: (() -> Void)? = nil
    
    init(with unityMetal: IUnityGraphicsMetalV2) {
        self.unityMetal = unityMetal
        MetalFrameCaptureAPI.register(with: self)
    }
    
    func onRenderEvent(eventType: Int32) {
        switch EventType(rawValue: eventType)! {
        case .startCapture:
            startGpuCapture()
            break
        case .stopCapture:
            stopGpuCapture()
            break
        }
    }
    
    func setFilePath(_ filePath: String) {
        self.filePath = filePath
    }
    
    func startGpuCapture() {
        guard let device: MTLDevice = unityMetal.MetalDevice() else {
            preconditionFailure("MTLDeviceが見つからない")
        }
        
        let captureManager = MTLCaptureManager.shared()
        let captureDescriptor = MTLCaptureDescriptor()
        captureDescriptor.captureObject = device
        captureDescriptor.destination = .gpuTraceDocument
        captureDescriptor.outputURL = URL(fileURLWithPath: filePath)
        
        do {
            try captureManager.startCapture(with: captureDescriptor)
        }
        catch {
            onGpuCaptureFailed?(error.localizedDescription)
            return
        }
        
        print("GPU capture started -> \(filePath)")
        return
    }
    
    private func stopGpuCapture() {
        MTLCaptureManager.shared().stopCapture()
        print("GPU capture stopped")
        onGpuCaptureComplete?()
    }
}

extension MetalFrameCapturePlugin: MetalFrameCaptureDelegate {
    
    func registerOnGpuCaptureFiled(_ delegate: @escaping (UnsafePointer<CChar>) -> Void) {
        onGpuCaptureFailed = { str in
            let utfText = (str as NSString).utf8String!;
            delegate(utfText)
        }
    }
    
    func registerOnGpuCaptureComplete(_ delegate: @escaping () -> Void) {
        onGpuCaptureComplete = {
            delegate()
        }
    }
}
