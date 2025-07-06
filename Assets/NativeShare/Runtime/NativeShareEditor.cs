using UnityEngine;

namespace NativeShare
{
    public sealed class NativeShareEditor : INativeShare
    {
        public void ShareFile(string filePath, string subject = "", string text = "")
        {
            Debug.Log($"Share file (Editor Mode): {filePath}");
            Debug.Log($"Subject: {subject}");
            Debug.Log($"Text: {text}");
            ShowInFileExplorer(filePath);
        }

        private static void ShowInFileExplorer(string filePath)
        {
            try
            {
#if UNITY_EDITOR_WIN
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
#elif UNITY_EDITOR_OSX
                System.Diagnostics.Process.Start("open", $"-R \"{filePath}\"");
#elif UNITY_EDITOR_LINUX
                System.Diagnostics.Process.Start("xdg-open", Path.GetDirectoryName(filePath));
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to show file in explorer: {ex.Message}");
            }
        }
    }
}
