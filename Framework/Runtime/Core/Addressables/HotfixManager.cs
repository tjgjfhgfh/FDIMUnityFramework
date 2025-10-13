using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using FDIM.Framework;
using HybridCLR;


namespace FDIM.Framework
{
    public class HotfixManager : SingletonPatternBase<HotfixManager>
    {
        // 缓存已加载的 Assembly
        private readonly List<Assembly> _loadedAssemblies = new();
        // 类型名到 Type 的缓存
        private readonly Dictionary<string, Type> _typeCache = new();

        /// <summary>
        /// 加载并执行热更 DLL
        /// </summary>
        public async Task LoadHotfixDLLAsync(string dllKey)
        {
            var textAsset = await AddressablesManager.Instance.LoadAssetAsync<TextAsset>(dllKey);
            if (textAsset == null || textAsset.bytes == null || textAsset.bytes.Length == 0)
            {
                Debug.LogError($"[HotfixManager] DLL 加载失败: {dllKey}");
                return;
            }

            byte[] dllBytes = textAsset.bytes;

            try
            {
                var ret = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                if (ret != LoadImageErrorCode.OK)
                    Debug.LogWarning($"LoadMetadataForAOTAssembly 返回错误: {ret}");
            }
            catch (Exception eMeta)
            {
                Debug.LogWarning($"[HotfixManager] LoadMetadataForAOTAssembly 失败: {eMeta}");
            }

            Assembly asm = Assembly.Load(dllBytes);
            Debug.Log($"[HotfixManager] Assembly.Load 成功: {asm.FullName}");

            _loadedAssemblies.Add(asm);

            // 缓存所有 MonoBehaviour 类型
            foreach (var type in asm.GetTypes())
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    string fullName = type.FullName; // HotfixNamespace.Test1
                    if (!_typeCache.ContainsKey(fullName))
                        _typeCache[fullName] = type;
                }
            }

            InvokeHotfixEntry(asm);
        }

        /// <summary>
        /// 动态挂载热更 MonoBehaviour，只需传类型全名，自动找对应 Assembly
        /// </summary>
        public void AddHotfixComponent(GameObject go, string typeFullName)
        {
            if (_typeCache.TryGetValue(typeFullName, out var type))
            {
                go.AddComponent(type);
                Debug.Log($"[HotfixManager] 已动态挂载热更脚本 {typeFullName} 到 {go.name}");
            }
            else
            {
                // 类型不在缓存里，尝试在已加载 Assembly 中查找
                foreach (var asm in _loadedAssemblies)
                {
                    type = asm.GetType(typeFullName);
                    if (type != null && typeof(MonoBehaviour).IsAssignableFrom(type))
                    {
                        _typeCache[typeFullName] = type; // 缓存起来
                        go.AddComponent(type);
                        Debug.Log($"[HotfixManager] 已动态挂载热更脚本 {typeFullName} 到 {go.name}");
                        return;
                    }
                }
                Debug.LogWarning($"[HotfixManager] 找不到热更类型: {typeFullName}");
            }
        }

        private void InvokeHotfixEntry(Assembly asm)
        {
            try
            {
                Type entryType = asm.GetType("HotfixEntry");
                if (entryType != null)
                {
                    var m = entryType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                            ?? entryType.GetMethod("Start", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                    m?.Invoke(null, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[HotfixManager] InvokeHotfixEntry 异常: {e}");
            }
        }
    }
}