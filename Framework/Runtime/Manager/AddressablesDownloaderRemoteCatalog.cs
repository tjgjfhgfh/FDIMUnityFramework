using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FDIM.Framework
{
    /// <summary>
    /// 运行时从远程 Catalog 自动全量下载 Addressables 资源
    /// </summary>
    public class AddressablesDownloaderRemoteCatalog : SingletonPatternBase<AddressablesDownloaderRemoteCatalog>
    {
        public async Task DownloadUpdatedFromRemoteCatalogAsync(string catalogUrl, Action<float> progressCallback = null)
        {
            try
            {
                Debug.Log($"[AddressablesDownloaderRemoteCatalog] 检查 Catalog 更新: {catalogUrl}");

                // 1️⃣ 加载远程 Catalog
                var catalogHandle = Addressables.LoadContentCatalogAsync(catalogUrl, true);
                await catalogHandle.Task;
                if (catalogHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("加载远程 catalog 失败");
                    return;
                }

                // 2️⃣ 检查 Catalog 更新
                var checkHandle = Addressables.CheckForCatalogUpdates(false);
                var catalogsToUpdate = await checkHandle.Task;

                if (catalogsToUpdate == null || catalogsToUpdate.Count == 0)
                {
                    Debug.Log("[AddressablesDownloaderRemoteCatalog] 没有资源更新，跳过下载。");
                    return;
                }

                // 3️⃣ 更新 Catalog
                var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate);
                var updatedLocators = await updateHandle.Task;
                Debug.Log($"[AddressablesDownloaderRemoteCatalog] 更新了 {updatedLocators.Count} 个 catalog");

                // 4️⃣ 获取所有需要下载的资源 keys
                var keys = updatedLocators.SelectMany(l => l.Keys).ToList();
                Debug.Log($"[AddressablesDownloaderRemoteCatalog] 需要检查的资源数量: {keys.Count}");

                long totalSize = 0;
                var keySizes = new Dictionary<object, long>();

                // 计算总大小
                foreach (var key in keys)
                {
                    var sizeHandle = Addressables.GetDownloadSizeAsync(key);
                    long size = await sizeHandle.Task;
                    keySizes[key] = size;
                    totalSize += size;
                }

                if (totalSize == 0)
                {
                    Debug.Log("[AddressablesDownloaderRemoteCatalog] 所有资源已是最新，无需下载。");
                    return;
                }

                long downloadedSize = 0;

                // 逐个下载资源并更新进度
                foreach (var key in keys)
                {
                    long keySize = keySizes[key];
                    if (keySize > 0)
                    {
                        Debug.Log($"[AddressablesDownloaderRemoteCatalog] 下载更新资源: {key} ({keySize / 1024f:F1} KB)");

                        var downloadHandle = Addressables.DownloadDependenciesAsync(key, true);

                        // 绑定实时进度
                        while (!downloadHandle.IsDone)
                        {
                            progressCallback?.Invoke((downloadedSize + (long)(downloadHandle.PercentComplete * keySize)) / (float)totalSize);
                            await Task.Yield();
                        }

                        // 完成后累加
                        downloadedSize += keySize;
                        progressCallback?.Invoke((float)downloadedSize / totalSize);

                        // 确保完成状态
                        await downloadHandle.Task;
                    }
                    else
                    {
                        // 没有下载量也要更新进度
                        progressCallback?.Invoke((float)downloadedSize / totalSize);
                    }
                }

                Debug.Log("[AddressablesDownloaderRemoteCatalog] 更新资源下载完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesDownloaderRemoteCatalog] 下载异常: {e}");
            }
        }

    }
}
