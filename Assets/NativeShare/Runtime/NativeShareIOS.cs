using System.Runtime.InteropServices;

namespace NativeShare
{
    public sealed class NativeShareIOS : INativeShare
    {
        public void ShareFile(string filePath, string subject = "", string text = "")
        {
            NativeMethod(filePath, subject, text);
            return;

            [DllImport("__Internal", EntryPoint = "NativeShare_shareFile")]
            static extern void NativeMethod(string filePath, string subject, string text);
        }
    }
}
