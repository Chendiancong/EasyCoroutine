# EasyCoroutine
通过对原有功能的封装，实现一系列自定义的Awaiter，支持在异步函数中等待Unity协程。
# 使用
```CSharp
using namespace EasyCoroutine;

public async Task RunTask()
{
    Debug.Log("wait 1s");
    await new WaitForSeconds(1f);
    Debug.Log("wait fixed update");
    await new WaitForFixedUpdate();
    Debug.Log("wait end of frame");
    await new WaitForEndOfFrame();
    var loader = new BundleAssetLoader<GameObject>
    {
        path = $"{Application.streamingAssetsPath}{Path.DirectorySeparatorChar}sphere.prefab.asset",
        assetName = "Sphere",
        autoUnloadBundle = true
    };
    var result = await loader;
    Debug.Log($"loaded {result.asset.name}");
    Instantiate(result.asset);
}

```

# 安装
通过Unity包管理器安装:   
1.可以通过git安装，地址为https://gitee.com/diancongchen/EasyCoroutine.git   
2.也可以clone下来，进行本地安装
