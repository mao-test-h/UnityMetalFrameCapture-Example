import UIKit

/// Unity からの呼び出しを受け取るためのC関数群
@_cdecl("NativeShare_shareFile")
public func showShareSheet(filePath: UnsafePointer<CChar>, subject: UnsafePointer<CChar>, text: UnsafePointer<CChar>) {
    let filePathString = String(cString: filePath)
    let subjectString = String(cString: subject)
    let textString = String(cString: text)
    
    ShareSheetManager.shared.showShareSheet(filePath: filePathString, subject: subjectString, text: textString)
}

/// ShareSheetの表示を管理するクラス
public class ShareSheetManager {
    public static let shared = ShareSheetManager()
    
    private init() {}
    
    /// ファイルシェアシートを表示する
    /// - Parameters:
    ///   - filePath: 共有するファイルのパス
    ///   - subject: 共有時の件名
    ///   - text: 共有時のテキスト
    public func showShareSheet(filePath: String, subject: String, text: String) {
        DispatchQueue.main.async { [weak self] in
            self?.presentShareSheet(filePath: filePath, subject: subject, text: text)
        }
    }
    
    private func presentShareSheet(filePath: String, subject: String, text: String) {
        guard let rootViewController = getRootViewController() else {
            print("ShareSheetManager: Root view controller not found")
            return
        }
        
        // ファイルが存在するかチェック
        let fileURL = URL(fileURLWithPath: filePath)
        guard FileManager.default.fileExists(atPath: filePath) else {
            print("ShareSheetManager: File not found at path: \(filePath)")
            return
        }
        
        // 共有するアイテムを準備
        var activityItems: [Any] = []
        
        // テキストが指定されている場合は追加
        if !text.isEmpty {
            activityItems.append(text)
        }
        
        // ファイルを追加
        activityItems.append(fileURL)
        
        // UIActivityViewControllerを作成
        let activityViewController = UIActivityViewController(
            activityItems: activityItems,
            applicationActivities: nil
        )
        
        // 件名を設定（メール送信時などに使用される）
        if !subject.isEmpty {
            activityViewController.setValue(subject, forKey: "subject")
        }
        
        // iPadでの表示対応（ポップオーバー）
        if let popoverController = activityViewController.popoverPresentationController {
            popoverController.sourceView = rootViewController.view
            popoverController.sourceRect = CGRect(
                x: rootViewController.view.bounds.midX,
                y: rootViewController.view.bounds.midY,
                width: 0,
                height: 0
            )
            popoverController.permittedArrowDirections = []
        }
        
        // 完了時のハンドリング
        activityViewController.completionWithItemsHandler = { activityType, completed, returnedItems, error in
            if let error = error {
                print("ShareSheetManager: Share failed with error: \(error.localizedDescription)")
            } else if completed {
                print("ShareSheetManager: Share completed successfully")
            } else {
                print("ShareSheetManager: Share cancelled")
            }
        }
        
        // ShareSheetを表示
        rootViewController.present(activityViewController, animated: true, completion: nil)
    }
    
    /// ルートビューコントローラーを取得する
    private func getRootViewController() -> UIViewController? {
        guard let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
              let window = windowScene.windows.first else {
            return nil
        }
        
        return window.rootViewController ?? UnityFramework.getInstance()?.appController()?.rootViewController
    }
}
