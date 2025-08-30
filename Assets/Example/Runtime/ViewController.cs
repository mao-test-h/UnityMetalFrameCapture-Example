using System;
using System.IO;
using NativeShare;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.UI;

namespace Example
{
    public sealed class ViewController : MonoBehaviour
    {
        [SerializeField] private Text isSupportedText;
        [SerializeField] private Button shareButton;
        
        [Header("To Xcode")]
        [SerializeField] private Button beginCaptureToXcode;
        [SerializeField] private Button endCaptureToXcode;
        [SerializeField] private Button captureNextFrameToXcode;
        
        [Header("To File")]
        [SerializeField] private Button beginCaptureToFile;
        [SerializeField] private Button endCaptureToFile;
        [SerializeField] private Button captureNextFrameToFile;
        
        private INativeShare _nativeShare;
        private string _latestFilePath;

        private void Start()
        {
            _nativeShare = NativeShareFactory.Create();

            // サポート状況を表示
            // DevTools → キャプチャしたら Xcode の Metal Debugger を立ち上げる
            // GPUTraceDocument → キャプチャしたトレースファイルを保存する
            var isGpuTrace = FrameCapture.IsDestinationSupported(FrameCaptureDestination.GPUTraceDocument);
            var isDevTool = FrameCapture.IsDestinationSupported(FrameCaptureDestination.DevTools);
            isSupportedText.text = $"[Supported]: DevTools: {isDevTool}, GPUTrace: {isGpuTrace}";
            Debug.Log(isSupportedText.text);

            CaptureToXcodeExample();
            CaptureToFileExample();

            shareButton.onClick.AddListener(ShareLatestFile);
        }

        private void CaptureToXcodeExample()
        {
            // キャプチャを開始し、キャプチャ終了後に Xcode の Metal Debugger を立ち上げる
            // NOTE: 終了させる際には次の `EndCapture()` を呼び出すこと
            beginCaptureToXcode.onClick.AddListener(() =>
            {
                FrameCapture.BeginCaptureToXcode();
                Debug.Log("Begin capture to Xcode");
            });

            // キャプチャ終了
            endCaptureToXcode.onClick.AddListener(() =>
            {
                FrameCapture.EndCapture();
                Debug.Log("End capture");
            });
    
            // 次の 1フレームだけキャプチャ (こちらは `EndCapture` を呼ぶ必要は無し)
            captureNextFrameToXcode.onClick.AddListener(() =>
            {
                FrameCapture.CaptureNextFrameToXcode();
                Debug.Log("Capture next frame to Xcode");
            });
        }

        private void CaptureToFileExample()
        {
            // キャプチャを開始し、キャプチャ終了後にトレースデータを指定したファイルに書き込む。
            // NOTE: 終了させる際には次の `EndCapture()` を呼び出すこと
            beginCaptureToFile.onClick.AddListener(() =>
            {
                _latestFilePath = GetFilePath();
                FrameCapture.BeginCaptureToFile(_latestFilePath);
                Debug.Log($"Begin capture to file: {_latestFilePath}");
            });
            
            // キャプチャ終了
            endCaptureToFile.onClick.AddListener(() =>
            {
                FrameCapture.EndCapture();
                Debug.Log("End capture");
                
                // 終了後にシェアUIを呼び出す
                ShareLatestFile();
            });

            // 次の 1フレームだけキャプチャ (こちらは `EndCapture` を呼ぶ必要は無し)
            captureNextFrameToFile.onClick.AddListener(() =>
            {
                _latestFilePath = GetFilePath();
                FrameCapture.CaptureNextFrameToFile(_latestFilePath);
                Debug.Log($"Capture next frame to file: {_latestFilePath}");
                
                // 終了後にシェアUIを呼び出す
                ShareLatestFile();
            });
        }
        
        private static string GetFilePath()
        {
            // NOTE: キャプチャファイルの拡張子は `.gputrace`
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"GpuCapture_{timestamp}.gputrace";
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            return filePath;
        }

        private void ShareLatestFile()
        {
            if (!string.IsNullOrEmpty(_latestFilePath))
            {
                _nativeShare.ShareFile(_latestFilePath);
            }
            else
            {
                Debug.LogWarning("No file to share. Please start a capture first.");
            }
        }

    }
}
