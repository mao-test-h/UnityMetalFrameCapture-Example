using UnityEngine;

namespace NativeShare
{
    public sealed class NativeShareDummy : INativeShare
    {
        public void ShareFile(string filePath, string subject = "", string text = "")
        {
            Debug.Log($"NativeShareDummy: File sharing is not supported on this platform. " +
                      $"File: {filePath}, Subject: {subject}, Text: {text}");
        }
    }
}
