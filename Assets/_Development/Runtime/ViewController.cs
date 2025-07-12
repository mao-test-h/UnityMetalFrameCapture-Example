using UnityEngine;
using UnityEngine.UI;

namespace MetalFrameCapture.Development
{
    public sealed class ViewController : MonoBehaviour
    {
        [SerializeField] private Button startCaptureButton;
        [SerializeField] private Button startCaptureStop;

        void Start()
        {
            startCaptureButton.onClick.AddListener(() =>
            {
                /*
                var filePath = Application.persistentDataPath + "/capture.mp4";
                var nativeProxy = MetalFrameCaptureFactory.Create();
                nativeProxy.Delegate = new NativeProxyDelegate();
                nativeProxy.StartGpuCapture(filePath);
                 */
            });

            startCaptureStop.onClick.AddListener(() =>
            {
                /*
                 var nativeProxy = MetalFrameCaptureFactory.Create();
                 nativeProxy.StopGpuCapture();
                 */
            });
        }
    }
}
