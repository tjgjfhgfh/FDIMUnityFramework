using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// Addressables加载管理器
/// </summary>
public class AddressablesManager : SingletonPatternBase<AddressablesManager>
{
    private Dictionary<string, AsyncOperationHandle> _loadedAssets = new();
    private List<AsyncOperationHandle<GameObject>> _instantiatedObjects = new();

    /// 加载单个资源（泛型）
    public void LoadAsset<T>(string key, Action<T> onComplete) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(key, out var cached) && cached.IsDone)
        {
            onComplete?.Invoke(cached.Result as T);
            return;
        }

        var handle = Addressables.LoadAssetAsync<T>(key);
        _loadedAssets[key] = handle;
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
                onComplete?.Invoke(op.Result as T);
            else
                Debug.LogError($"[Addressables] 加载失败: {key}");
        };
    }

    /// 批量加载资源
    public void LoadAssets<T>(IEnumerable<string> keys, Action<IList<T>> onComplete) where T : UnityEngine.Object
    {
        var handle = Addressables.LoadAssetsAsync<T>(keys, null);
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
                onComplete?.Invoke(op.Result);
            else
                Debug.LogError("[Addressables] 批量加载失败");
        };
    }

    /// 实例化 GameObject（会记录以便销毁）
    public void Instantiate(string key, Vector3 pos, Quaternion rot, Transform parent = null,
        Action<GameObject> onComplete = null)
    {
        var handle = Addressables.InstantiateAsync(key, pos, rot, parent);
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _instantiatedObjects.Add(op);
                onComplete?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"[Addressables] 实例化失败: {key}");
            }
        };
    }

    /// 释放实例化对象
    public void ReleaseInstance(GameObject go)
    {
        Addressables.ReleaseInstance(go);
    }

    /// 加载 JSON 文本并解析为对象
    public void LoadJson<T>(string key, Action<T> onComplete)
    {
        LoadAsset<TextAsset>(key, json =>
        {
            try
            {
                var obj = JsonUtility.FromJson<T>(json.text);
                onComplete?.Invoke(obj);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Addressables] JSON 解析失败: {e.Message}");
            }
        });
    }

    /// 从图集中加载 Sprite
    public void LoadSpriteFromAtlas(string atlasKey, string spriteName, Action<Sprite> onComplete)
    {
        LoadAsset<SpriteAtlas>(atlasKey, atlas =>
        {
            var sprite = atlas.GetSprite(spriteName);
            if (sprite != null)
                onComplete?.Invoke(sprite);
            else
                Debug.LogError($"[Addressables] 图集中找不到 Sprite: {spriteName}");
        });
    }

    /// 卸载某个资源
    public void Unload(string key)
    {
        if (_loadedAssets.TryGetValue(key, out var handle))
        {
            Addressables.Release(handle);
            _loadedAssets.Remove(key);
        }
    }

    /// 卸载全部资源
    public void UnloadAll()
    {
        foreach (var kv in _loadedAssets)
            Addressables.Release(kv.Value);
        _loadedAssets.Clear();
    }
}