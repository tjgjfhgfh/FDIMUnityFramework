using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using FDIM.Framework;
using HybridCLR;

namespace FDIM.Framework
{
    /// <summary>
    /// HybridCLR + Addressables 热更管理器
    /// 支持 MonoBehaviour 与 普通类调用
    /// </summary>
    public class HotfixManager : SingletonPatternBase<HotfixManager>
    {
        private readonly List<Assembly> _loadedAssemblies = new();
        private readonly Dictionary<string, Type> _monoCache = new();   // MonoBehaviour 缓存
        private readonly Dictionary<string, Type> _typeCache = new();   // 所有类型缓存

        /// <summary>
        /// 加载热更 DLL，并自动调用入口
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
            AddressablesManager.Instance.Unload(dllKey); // 释放 TextAsset

            try
            {
                var ret = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                if (ret != LoadImageErrorCode.OK)
                    Debug.LogWarning($"[HotfixManager] LoadMetadataForAOTAssembly 返回: {ret}");
            }
            catch { /* 非 IL2CPP 时忽略 */ }

            Assembly asm = Assembly.Load(dllBytes);
            _loadedAssemblies.Add(asm);

            // 缓存类型
            CacheAssemblyTypes(asm);

            Debug.Log($"[HotfixManager] 成功加载 DLL: {asm.FullName}，共 {asm.GetTypes().Length} 个类型");

            InvokeHotfixEntry(asm);
        }

        private void CacheAssemblyTypes(Assembly asm)
        {
            foreach (var type in asm.GetTypes())
            {
                _typeCache[type.FullName] = type;
                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                    _monoCache[type.FullName] = type;
            }
        }

        #region === MonoBehaviour 动态挂载 ===
        public void AddHotfixComponent(GameObject go, string typeFullName)
        {
            if (_monoCache.TryGetValue(typeFullName, out var type))
            {
                go.AddComponent(type);
                Debug.Log($"[HotfixManager] 动态挂载热更脚本 {typeFullName} 到 {go.name}");
            }
            else
            {
                Debug.LogWarning($"[HotfixManager] 找不到 MonoBehaviour 类型: {typeFullName}");
            }
        }
        #endregion

        #region === 通用类型调用支持 ===

        /// <summary>
        /// 调用热更中静态函数（无返回值）
        /// 例：InvokeStaticMethod("Hotfix.GameLogic", "Init");
        /// </summary>
        public void InvokeStaticMethod(string typeFullName, string methodName, params object[] args)
        {
            if (!_typeCache.TryGetValue(typeFullName, out var type))
            {
                Debug.LogError($"[HotfixManager] 找不到类型: {typeFullName}");
                return;
            }

            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                Debug.LogError($"[HotfixManager] 找不到方法 {methodName} 于 {typeFullName}");
                return;
            }

            try
            {
                method.Invoke(null, args);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HotfixManager] 调用 {typeFullName}.{methodName} 异常: {e}");
            }
        }

        /// <summary>
        /// 获取热更中类型实例
        /// 例：var obj = CreateInstance("Hotfix.Data.Item");
        /// </summary>
        public object CreateInstance(string typeFullName, params object[] args)
        {
            if (!_typeCache.TryGetValue(typeFullName, out var type))
            {
                Debug.LogError($"[HotfixManager] 找不到类型: {typeFullName}");
                return null;
            }

            try
            {
                return Activator.CreateInstance(type, args);
            }
            catch (Exception e)
            {
                Debug.LogError($"[HotfixManager] 实例化 {typeFullName} 异常: {e}");
                return null;
            }
        }

        /// <summary>
        /// 泛型支持（如创建 List<int>）
        /// </summary>
        public Type GetGenericType(string genericFullName, params Type[] genericArgs)
        {
            if (!_typeCache.TryGetValue(genericFullName, out var type))
            {
                Debug.LogError($"[HotfixManager] 找不到泛型类型定义: {genericFullName}");
                return null;
            }
            return type.MakeGenericType(genericArgs);
        }

        #endregion

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
                Debug.LogError($"[HotfixManager] 入口调用异常: {e}");
            }
        }
    }
  
}
