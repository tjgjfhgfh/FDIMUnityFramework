using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace FDIM.Framework
{
    /// <summary>
    /// Addressables加载管理器
    /// </summary>
    public class AddressablesManager : SingletonPatternBase<AddressablesManager>
    {
        private Dictionary<string, AsyncOperationHandle> _loadedAssets = new();

        #region  lambda表达式 版本

        /// <summary>
        /// 加载单个资源（泛型）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="onComplete"></param>
        public void LoadAsset<T>(string key, Action<T> onComplete) where T : UnityEngine.Object
        {
            if (_loadedAssets.TryGetValue(key, out var cached) && cached.IsDone)
            {
                onComplete?.Invoke(cached.Result as T);

                Managers.LogMessage.Log("从字典读取");
                return;
            }
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    onComplete?.Invoke(op.Result as T);
                    _loadedAssets[key] = handle;
                }
                else
                    Debug.LogError($"[Addressables] 加载失败: {key}");
            };
            Managers.LogMessage.Log("从Addressables加载");

        }

        /// <summary>
        /// 批量加载多个资源，并在全部完成后回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="onComplete"></param>
        public void LoadAssets<T>(IEnumerable<string> keys, Action<IList<T>> onComplete) where T : UnityEngine.Object
        {
            List<T> loadedList = new();
            List<AsyncOperationHandle<T>> handles = new();

            int totalCount = 0;
            int completedCount = 0;

            foreach (var key in keys)
            {
                totalCount++;
                var handle = Addressables.LoadAssetAsync<T>(key);
                handles.Add(handle);
                handle.Completed += op =>
                {
                    completedCount++;

                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        loadedList.Add(op.Result);
                        _loadedAssets[key] = handle;
                    }
                    else
                        Debug.LogError($"[Addressables] 资源加载失败: {key}");

                    // 当全部加载完成时触发回调
                    if (completedCount == totalCount)
                        onComplete?.Invoke(loadedList);
                };
            }
        }
        #endregion

        #region Async/Await 版本
        /// <summary>
        /// 异步加载单个资源
        /// </summary>
        public async Task<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            if (_loadedAssets.TryGetValue(key, out var cached) && cached.IsDone)
            {
                Managers.LogMessage.Log($"[AddressablesAsync] 从字典读取: {key}");
                return cached.Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            Managers.LogMessage.Log($"[AddressablesAsync] 从Addressables加载: {key}");

            try
            {
                var result = await handle.Task;
                _loadedAssets[key] = handle;
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesAsync] 加载失败: {key}\n{e}");
                return null;
            }
        }


        /// <summary>
        /// 加载BG音频专用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<AudioClip> LoadAudioBGAsync(string key)
        {
            if (_loadedAssets.TryGetValue(key, out var cached) && cached.IsDone)
            {
                Managers.LogMessage.Log($"[AddressablesAsync] 从字典读取: {key}");
                return cached.Result as AudioClip;
            }


            var handle = Addressables.LoadAssetAsync<AudioClip>(key);
            Managers.LogMessage.Log($"[AddressablesAsync] 从Addressables加载: {key}");

            try
            {
                AudioClip result = await handle.Task;
                _loadedAssets[key] = handle;
                Managers.AudioManager.PlayBGM(result);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesAsync] 加载失败: {key}\n{e}");
                return null;
            }

        }

        /// <summary>
        /// 加载角色对话音频专用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<AudioClip> LoadAudioVoiceAsync(string key)
        {
            if (_loadedAssets.TryGetValue(key, out var cached) && cached.IsDone)
            {
                Managers.LogMessage.Log($"[AddressablesAsync] 从字典读取: {key}");
                return cached.Result as AudioClip;
            }


            var handle = Addressables.LoadAssetAsync<AudioClip>(key);
            Managers.LogMessage.Log($"[AddressablesAsync] 从Addressables加载: {key}");

            try
            {
                AudioClip result = await handle.Task;
                _loadedAssets[key] = handle;
                Managers.AudioManager.PlayVoice(result);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesAsync] 加载失败: {key}\n{e}");
                return null;
            }

        }


        /// <summary>
        /// 加载物体专用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<GameObject> LoadGameobjectAsync(string key)
        {
            if (_loadedAssets.TryGetValue(key, out var cached) && cached.IsDone)
            {
                Managers.LogMessage.Log($"[AddressablesAsync] 从字典读取: {key}");
                return cached.Result as GameObject;
            }


            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            Managers.LogMessage.Log($"[AddressablesAsync] 从Addressables加载: {key}");

            try
            {
                GameObject result = await handle.Task;
                _loadedAssets[key] = handle;
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesAsync] 加载失败: {key}\n{e}");
                return null;
            }

        }


        /// <summary>
        /// 批量异步加载多个资源，并返回列表
        /// </summary>
        public async Task<IList<T>> LoadAssetsAsync<T>(IEnumerable<string> keys) where T : UnityEngine.Object
        {
            var results = new List<T>();
            var tasks = new List<Task<T>>();

            foreach (var key in keys)
                tasks.Add(LoadAssetAsync<T>(key));

            var loaded = await Task.WhenAll(tasks);

            foreach (var item in loaded)
                if (item != null)
                    results.Add(item);

            return results;
        }

        /// <summary>
        /// 异步加载 JSON 文件并解析为对象
        /// </summary>
        public async Task<T> LoadJsonAsync<T>(string key)
        {
            var jsonAsset = await LoadAssetAsync<TextAsset>(key);
            if (jsonAsset == null)
            {
                Debug.LogError($"[AddressablesAsync] JSON 加载失败: {key}");
                return default;
            }

            try
            {
                return JsonUtility.FromJson<T>(jsonAsset.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesAsync] JSON 解析失败: {e.Message}");
                return default;
            }
        }
        #endregion

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
}
