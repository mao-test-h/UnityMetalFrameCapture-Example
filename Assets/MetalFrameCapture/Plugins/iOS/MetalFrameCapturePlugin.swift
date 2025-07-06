import Foundation
import Metal

final class MetalFrameCapturePlugin {
    
    private enum EventType: Int32 {
        case stopCapture = 0
    }
    
    public static var shared: MetalFrameCapturePlugin! = nil
    
    private let unityMetal: IUnityGraphicsMetalV2
    private var onGpuCaptureComplete: (() -> Void)? = nil
    
    init(with unityMetal: IUnityGraphicsMetalV2) {
        self.unityMetal = unityMetal
        MetalFrameCaptureAPI.register(with: self)
    }
    
    func onRenderEvent(eventType: Int32) {
        switch EventType(rawValue: eventType)! {
        case .stopCapture:
            stopGpuCapture()
            break
        }
    }
    
    func startGpuCapture(filePath: String) -> Bool {
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
            print("Failed to GPU capture", error)
            return false
        }
        
        print("GPU capture started -> \(filePath)")
        return true
    }
    
    func stopGpuCaptureDirect() {
        MTLCaptureManager.shared().stopCapture()
        print("GPU capture stopped (internal)")
        onGpuCaptureComplete?()
    }
    
    private func stopGpuCapture() {
        //unityMetal.EndCurrentCommandEncoder()
        MTLCaptureManager.shared().stopCapture()
        print("GPU capture stopped")
        onGpuCaptureComplete?()
    }
}

extension MetalFrameCapturePlugin: MetalFrameCaptureDelegate {
    func registerOnGpuCaptureComplete(_ delegate: @escaping () -> Void) {
        onGpuCaptureComplete = {
            delegate()
        }
    }
}
