#import <UIKit/UIKit.h>
#import <Metal/Metal.h>
#import "UnityAppController.h"
#import "UnityFramework.h"

#include "Unity/IUnityInterface.h"
#include "Unity/IUnityGraphics.h"
#include "Unity/IUnityGraphicsMetal.h"

// MARK:- P/Invoke

// NOTE:
// - ここではC側で定義されてるシンボルが必要な関数のみ宣言
// - それ以外はSwift側の`NativeCallProxy`に定義すること

extern void metalFrameCapture_onUnityGfxDeviceEventInitialize();
extern void metalFrameCapture_onRenderEvent();

// GL.IssuePluginEvent で登録するコールバック関数のポインタを返す
UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API metalFrameCapture_getRenderEventFunc() {
    // Swift側で実装している`onRenderEvent`を返す
    return metalFrameCapture_onRenderEvent;
}


// MARK:- Low-level native plug-in interface

static IUnityInterfaces* g_UnityInterfaces = 0;
static IUnityGraphics* g_Graphics = 0;
static IUnityGraphicsMetalV2* g_MetalGraphics = 0;

// NOTE: 各定義は `IUnityGraphics.h` を参照
static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType) {
    switch (eventType) {
        case kUnityGfxDeviceEventInitialize:
            assert(g_Graphics->GetRenderer() == kUnityGfxRendererMetal);
            metalFrameCapture_onUnityGfxDeviceEventInitialize();
            break;
        case kUnityGfxDeviceEventShutdown:
            assert(g_Graphics->GetRenderer() == kUnityGfxRendererMetal);
            break;
        default:
            // ignore others
            break;
    }
}

// Pluginのロードイベント
void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces) {
    g_UnityInterfaces = unityInterfaces;
    g_Graphics = UNITY_GET_INTERFACE(g_UnityInterfaces, IUnityGraphics);
    g_MetalGraphics = UNITY_GET_INTERFACE(g_UnityInterfaces, IUnityGraphicsMetalV2);

    // IUnityGraphics にイベントを登録
    // NOTE: kUnityGfxDeviceEventInitialize の後にプラグインのロードを受けるので、コールバックは手動で行う必要があるとのこと
    g_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
    OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

// Pluginのアンロードイベント
void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload() {
    g_Graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}


// MARK:- UnityGraphicsBridgeの実装

@implementation UnityGraphicsBridge {
}
+ (IUnityGraphicsMetalV2*)getUnityGraphicsMetal {
    return g_MetalGraphics;
}
@end


// MARK:- UnityPluginLoad と UnityPluginUnload の登録 (iOSのみ)

// Unityが UnityAppController と言う UIApplicationDelegate の実装クラスを持っているので、
// メンバ関数である shouldAttachRenderDelegate をオーバーライドすることで登録を行う必要がある。
@interface MyAppController : UnityAppController {
}
- (void)shouldAttachRenderDelegate;
@end

@implementation MyAppController

- (void)shouldAttachRenderDelegate {
    UnityRegisterPlugin(&UnityPluginLoad, &UnityPluginUnload);
}
@end

// 定義したサブクラスはこちらのマクロを経由して登録する必要がある
IMPL_APP_CONTROLLER_SUBCLASS(MyAppController);
