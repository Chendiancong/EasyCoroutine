# TaskFrameworkInUnity
在Unity中，原生提供的异步方法就是协程，但是协程的使用存在很多不够方便的地方。比方说，无法访问协程的返回值，而且迭代器的异常处理有一定的局限，我们不能在try-catch中加入另外一个生成器。
在支持了.Net4.x语法之后，我们可以使用一个很棒的特性，async/await异步方法。异步方法是一个语法糖，它将函数展开成多个段落，段落之间通过一定的回调机制按顺序执行。
支持返回值，更完整的堆栈跟踪，那么用异步方法来代替协程就有了更多的优势。实际上，async/await最终会被转化为若干方法和属性的动态调用，支持自定义异步行为，这也为我们基于协程实现特定的异步工作流实现了便捷。

通过实现INotifyCompletion接口，以及一些会被动态调用的属性，可以构建出一个自定义的可被等待的对象CustomAwaiter，再通过扩展方法的方式，我对一些在协程编码中常用的类型实现GetAwaiter方法，得到对应Awaiter，而这些Awaiter最终会被使用在异步工作流里面。
这些类型包括：WaitForSeconds, WaitForSecondsRealtime, WaitForEndOfFrame, WaitForFixedUpdate, AssetBundleCreateRequest, AssetBundleRequest
常见用法：
```CSharp
using namespace AsyncWork.Core;

public async void RunTask()
{
    Debug.Log("wait 1s");
    await new WaitForSeconds(1f);
    Debug.Log("wait fixed update");
    await new WaitForFixedUpdate();
    Debug.Log("wait end of frame");
    await new WaitForEndOfFrame();
    var instruction = new WaitForSeconds(1f);
    for (int i = 0; i < 3; ++i)
    {
        Debug.Log("wait 1s");
        await instruction;
    }
    Debug.Log("wait 1s");
    await Awaiter.Wait(instruction);
    Debug.Log("load asset");
    string path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "sphere.prefab.asset";
    GameObject asset = await Awaiter.Load<GameObject>(path)
                                .SetAssetName("Sphere");
    Debug.Log($"loaded {asset.name}");
    Instantiate(asset);
    Debug.Log("ok");
}

```

# 安装
通过Unity包管理器安装
1.可以通过git安装，地址为https://gitee.com/diancongchen/TaskFrameworkInUnity.git  
2.也可以clone下来，进行本地安装
