namespace NativeShare
{
    /// <summary>
    /// ネイティブシェア機能のインターフェース
    /// </summary>
    public interface INativeShare
    {
        /// <summary>
        /// ファイルをネイティブシェアUIで共有する
        /// </summary>
        /// <param name="filePath">共有するファイルのパス</param>
        /// <param name="subject">共有時の件名</param>
        /// <param name="text">共有時のテキスト</param>
        void ShareFile(string filePath, string subject = "", string text = "");
    }
}
