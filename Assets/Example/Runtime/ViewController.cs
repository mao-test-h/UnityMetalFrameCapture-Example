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
        [SerializeField] private Button beginCaptureToFile;
        [SerializeField] private Button beginCaptureToXcode;
        [SerializeField] private Button endCapture;
        [SerializeField] private Button captureNextFrameToFile;
        [SerializeField] private Button captureNextFrameToXcode;
        [SerializeField] private Button shareButton;

        private INativeShare _nativeShare;
        private string _latestFilePath;

        private void Start()
        {
            _nativeShare = NativeShareFactory.Create();

            // サポート状況を表示
            // DevTools → Xcode のキャプチャに対応しているか
            // GPUTraceDocument → アプリ上からのキャプチャに対応しているか
            var isGpuTrace = FrameCapture.IsDestinationSupported(FrameCaptureDestination.GPUTraceDocument);
            var isDevTool = FrameCapture.IsDestinationSupported(FrameCaptureDestination.DevTools);
            isSupportedText.text = $"[Supported]: DevTools: {isDevTool}, GPUTrace: {isGpuTrace}";
            Debug.Log(isSupportedText.text);

            SetEventBeginCapture();
            SetEventCaptureNextFrame();

            shareButton.onClick.AddListener(ShareLatestFile);
        }

        private void SetEventBeginCapture()
        {
            // アプリ上でキャプチャを開始し、結果を指定したファイルに書き込む。
            // NOTE: キャプチャを終了する際には `EndCapture()` を呼び出すこと
            beginCaptureToFile.onClick.AddListener(() =>
            {
                _latestFilePath = GetFilePath();
                FrameCapture.BeginCaptureToFile(_latestFilePath);
                Debug.Log($"Begin capture to file: {_latestFilePath}");
            });

            // Xcode でキャプチャを開始し、終了時に Xcode の Metal debugger を立ち上げる。
            // NOTE: キャプチャの停止周りについては同上
            beginCaptureToXcode.onClick.AddListener(() =>
            {
                FrameCapture.BeginCaptureToXcode();
                Debug.Log("Begin capture to Xcode");
            });

            // キャプチャの停止
            endCapture.onClick.AddListener(() =>
            {
                FrameCapture.EndCapture();
                Debug.Log("End capture");
                ShareLatestFile();
            });
        }

        private void SetEventCaptureNextFrame()
        {
            // 次の1フレームを対象にアプリ上でキャプチャを開始し、結果を指定したファイルに書き込む。( `BeginCaptureToFile()` の1フレーム版)
            // NOTE: こちらは `FrameCapture.EndCapture()` を呼び出す必要はない
            captureNextFrameToFile.onClick.AddListener(() =>
            {
                _latestFilePath = GetFilePath();
                FrameCapture.CaptureNextFrameToFile(_latestFilePath);
                Debug.Log($"Capture next frame to file: {_latestFilePath}");
            });

            // 次の1フレームを対象に Xcode でキャプチャを開始し、終了時に Xcode の Metal debugger を立ち上げる。( `BeginCaptureToXcode()` の1フレーム版 )
            // NOTE: キャプチャの停止周りについては同上
            captureNextFrameToXcode.onClick.AddListener(() =>
            {
                FrameCapture.CaptureNextFrameToXcode();
                Debug.Log("Capture next frame to Xcode");
            });
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

        private static string GetFilePath()
        {
            // NOTE: キャプチャファイルの拡張子は `.gputrace`
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"GpuCapture_{timestamp}.gputrace";
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            return filePath;
        }
    }
}
