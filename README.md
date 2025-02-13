# EasyCoroutine
**EasyCoroutine**是一个基于 Unity 协程的轻量级异步任务处理框架，旨在简化 Unity 中的异步操作管理。通过该框架，开发者可以轻松地创建、执行、等待和管理异步任务，支持链式调用、错误处理、对象池管理等功能，特别适用于资源加载、网络请求等异步场景。
## 主要特性
- <b>异步任务管理：</b>支持创建和管理异步任务，支持任务的等待、完成和错误处理
- <b>链式调用：</b>通过<b>Then</b>和<b>Catch</b>方法，支持任务的链式调用和错误捕获。
- <b>对象池管理：</b>通过<b>PooledWorker</b>和<b>PooledWorker&lt;Result&gt;</b>类，支持对象池管理，减少频繁创建和销毁对象的开销。
- <b>协程扩展：</b>提供了<b>GetAwaiter</b>和<b>Then</b>扩展方法，支持await语法和链式调用。
- <b>资源加载：</b>提供<b>WaitBundleAsset</b><b>和WaitBundleAssetMultiple</b>类，支持从AssetBundle中异步加载单个或多个资源。
- <b>自由控制：</b>通过<b>WaitPromise</b>和<b>WaitPromise&lt;Result&gt;</b>类，支持手动控制任务的完成和错误处理。
## 基本用法
```CSharp
using namespace EasyCoroutine;
using namespace UnityEngine;

// 创建一个异步任务
var worker = new Worker(() => {
    System.Threading.Thread.Sleep(1000);
    Debug.Log("Task completed!");
});

await worker;

worker.Then(() => Debug.Log("Task Succeed!"))
    .Catch(ex => Debug.LogError($"Task failed: {ex.Message}"));

// 通过Unity协程进行等待
await new WaitForSeconds(1f);

// 加载AssetBundle中的资源
var loader = new BundleAssetLoader<Texture2D>
{
    path = "path/to/assetbundle",
    assetName = "texture",
    autoUnloadBundle = true
}

var result = await loader;
if (result.asset != null)
    Debug.Log("Resource loaded:" + result.asset.name);

// 自由控制任务
var promise = new WaitPromise();

// 在适当的时候手动完成任务
// promise.Resolve();
await promise;

// 链式调用
WaitInstruction.Create(new WaitForSeconds(1f))
    .Then(() => {
        Debug.Log("wait 1s");
        return WaitInstruction.Create(new WaitForSeconds(2f));
    })
    .Then(() => {
        Debug.Log("wait 2s");
        return WaitInstruction.Create(new WaitForEndOfFrame());
    })
    .Then(() => {
        Debug.Log("wait one frame");
        var loader = new BundleAssetLoader<GameObject>
        {
            path = Path.Combine(Application.streamingAssetsPath, "sphere.asset"),
            assetName = "Sphere",
            autoUnloadBundle = true
        };
        return WaitBundleAsset<GameObject>.Create(loader);
    })
    .Then(worker => {
        Debug.Log($"loaded {worker.GetResult()}");
        var asset = worker.GetResult().asset;
        Instantiate(asset);
    })
    .Then(() => {
        Debug.Log("worker is finished");
    });
```

## 核心模块
<b>Worker</b>和<b>Worker&lt;Result&gt;</b>是整个EasyCoroutine系统的核心类，它们为异步任务的处理提供了基础支持。这两个类的作用贯穿了整个系统，定义了异步任务的基本行为，包括任务的创建、执行、完成、错误处理、链式调用等。

1. 异步任务的基础：
    - Worker和Worker&lt;Result&gt;是整个系统的核心，其他类（如WaitBundleAsset，WaitInstruction，WaitPromise等）都直接或简介依赖于它们。
    - 它们为异步任务提供了统一的接口和行为规范。
    - Worker类是一个轻量级的异步任务管理器，设计灵感来源于JavaScript的Promise。
    - Worker&lt;Result&gt;类扩展了Worker的功能，用于处理返回结果的异步操作。
2. 任务链式调用：
    - 通过<b>Then</b>和<b>Catch</b>方法，实现任务的链式调用，使得复杂的异步操作可以以清晰的方式编写。
3. 异步等待支持：
    - 通过实现<b>GetAwaiter</b>方法，支持<b>async await</b>语法
4. 错误处理：
    - 提供了统一的错误处理机制，您可以通过**Catch**方法捕获任务执行过程中的异常
5. 扩展性：
    - 通过继承Worker和Worker&lt;Result&gt;，可以轻松扩展新的异步任务类型。

## 安装
通过**Unity包管理器（Window/PackageManager）**安装:   
1. **add package from git url**，地址为git@github.com:Chendiancong/EasyCoroutine.git
2. clone仓库到本地，通过**add package from dist**进行安装
3. 也可以直接作为子模块添加到Assets目录下，这样在使用的同时也可以对他进行修改

## improvements
- [ ] 链式调用的时候，异常需要跨越多个Then向下传递，直到遇到第一个Catch

## License
EasyCoroutine采用 MIT 许可证，详情请参阅LICENSE文件。
