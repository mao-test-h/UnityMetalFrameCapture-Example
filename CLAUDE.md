# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## プロジェクト概要

Unity Metal Frame Capture は、iOS デバイス上で Metal フレームキャプチャ機能を提供する Unity パッケージです。Metal API のレンダリングパイプラインをキャプチャし、デバッグとパフォーマンス解析を支援します。

## 開発環境

- **Unity バージョン**: 6000.0+ (URP - Universal Render Pipeline)
- **対象プラットフォーム**: iOS
- **開発言語**: C#, Swift, Objective-C

## アーキテクチャ

### コア構造

1. **MetalFrameCapture** - メインパッケージ
   - `Runtime/` - 実行時コード
     - `INativeProxy.cs` - ネイティブプラグインインターフェース
     - `MetalFrameCaptureFactory.cs` - プラットフォーム別実装のファクトリー
     - `NativeProxyForIOS.cs` - iOS 実装
     - `NativeProxyForEditor.cs` - エディタ用ダミー実装
   - `Editor/` - エディタ拡張
     - `XcodePostProcess.cs` - iOS ビルド後処理（Xcode プロジェクト設定）
   - `Plugins/iOS/` - iOS ネイティブプラグイン
     - `MetalFrameCapturePlugin.swift` - Metal API との連携
     - `MetalFrameCaptureProxy.swift` - Unity-Swift ブリッジ

2. **NativeShare** - ファイル共有機能パッケージ
   - 同様のアーキテクチャパターンを使用
   - プラットフォーム別実装をファクトリーパターンで管理

### 重要な設計パターン

1. **プラットフォーム抽象化**
   - インターフェース (`INativeProxy`, `INativeShare`) で機能を定義
   - ファクトリーパターンでプラットフォーム別実装を切り替え
   - コンパイル時条件分岐 (`#if UNITY_IOS`) を使用

2. **Unity-ネイティブ連携**
   - Low-Level Native Plugin Interface を使用
   - Swift から Unity の Metal API にアクセス
   - `XcodePostProcess.cs` で必要なヘッダーファイルを public 化

3. **ビルド後処理**
   - Xcode プロジェクトの自動設定
   - Metal Frame Capture の有効化
   - 必要なフレームワーク設定の追加

## 依存関係

- **R3** - リアクティブプログラミング
- **UniTask** - 非同期処理
- **Universal Render Pipeline (URP)** - レンダリングパイプライン
- **NuGetForUnity** - NuGet パッケージ管理

## 開発時の注意点

1. iOS ビルド時は必ず `XcodePostProcess.cs` が実行される
2. Swift ファイルの変更時は Unity エディタの再起動が必要な場合がある
3. Metal API へのアクセスは RenderThread から行う必要がある
4. URP のレンダリング機能と連携する場合は、レンダーフィーチャーの実装が必要
