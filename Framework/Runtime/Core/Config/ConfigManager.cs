using LitJson;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FDIM.Framework
{
    public class ConfigManager : SingletonPatternBase<ConfigManager>
    {
        private readonly Dictionary<Type, IList> _listCache = new();
        private readonly Dictionary<Type, object> _intIdCache = new();

        /// <summary>
        /// UseBinary代表二进制orJson
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <param name="UseBinary"></param>
        public void Init<T>(TextAsset asset, bool UseBinary ) where T : class
        {
            Debug.Log($"ConfigManager: 初始化中 (UseBinary={UseBinary})");

            if (asset == null)
            {
                Debug.LogError("ConfigManager: 资产文件为空！");
                return;
            }

            // 1. 选择解析 JSON 或 Protobuf
            if (UseBinary)
            {
                Debug.Log($"[Runtime] Loaded bytes length = {asset.bytes.Length}");
                try
                {
                    using (var ms = new System.IO.MemoryStream(asset.bytes))
                    {
                        // 使用泛型 T 来反序列化数据
                        var data = Serializer.Deserialize<T>(ms);
                        BuildCaches(data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"ConfigManager: Protobuf 反序列化失败：{e}");
                    return;
                }
            }
            else
            {
                Debug.Log($"[Runtime] Loaded JSON length = {asset.text.Length}");
                try
                {
                    // 使用泛型 T 来反序列化数据
                    var data = JsonMapper.ToObject<T>(asset.text);
                    BuildCaches(data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"ConfigManager: JSON 反序列化失败：{e}");
                    return;
                }
            }
        }

        // 修改 BuildCaches 方法，接收泛型数据并根据类型构建缓存
        private void BuildCaches<T>(T gameData) where T : class
        {
            _listCache.Clear();
            _intIdCache.Clear();
            int tableCount = 0;

            Type dataType = typeof(T);

            // 支持 public 属性 和 public 字段
            IEnumerable<MemberInfo> members = new List<MemberInfo>()
                .Concat(dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .Concat(dataType.GetFields(BindingFlags.Public | BindingFlags.Instance));

            foreach (var m in members)
            {
                Type memberType;
                object value;
                if (m is PropertyInfo pi)
                {
                    memberType = pi.PropertyType;
                    value = pi.GetValue(gameData);
                }
                else if (m is FieldInfo fi)
                {
                    memberType = fi.FieldType;
                    value = fi.GetValue(gameData);
                }
                else continue;

                // 仅处理 List<>
                if (!memberType.IsGenericType || memberType.GetGenericTypeDefinition() != typeof(List<>))
                    continue;

                var list = value as IList;
                if (list == null) continue;

                Type itemType = memberType.GetGenericArguments()[0];
                _listCache[itemType] = list;
                tableCount++;

                // 尝试通过 public 属性 Id 或者 public 字段 Id 创建索引
                var idProp = itemType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)
                             ?? itemType.GetProperty("id", BindingFlags.Public | BindingFlags.Instance);
                var idField = itemType.GetField("Id", BindingFlags.Public | BindingFlags.Instance)
                              ?? itemType.GetField("id", BindingFlags.Public | BindingFlags.Instance);

                if (idProp != null && idProp.PropertyType == typeof(int))
                {
                    CreateIntIndex(itemType, list, o => (int)idProp.GetValue(o));
                }
                else if (idField != null && idField.FieldType == typeof(int))
                {
                    CreateIntIndex(itemType, list, o => (int)idField.GetValue(o));
                }
                else
                {
                    Debug.LogWarning($"ConfigManager: `{itemType.Name}` 没有找到 Id 属性或字段，无法创建索引");
                }
            }

            Debug.Log($"ConfigManager: 初始化完成，共解析了 {tableCount} 个表");
            Managers.EventCenterManager.Dispatch("ConfigManagerInitCompleted");
        }

        // 创建字典索引
        private void CreateIntIndex(Type itemType, IList list, Func<object, int> keySelector)
        {
            Type dictT = typeof(Dictionary<,>).MakeGenericType(typeof(int), itemType);
            var dict = Activator.CreateInstance(dictT) as IDictionary;

            int count = 0;
            foreach (var obj in list)
            {
                int key = keySelector(obj);
                if (!dict.Contains(key))
                {
                    dict.Add(key, obj);
                    count++;
                }
            }

            _intIdCache[itemType] = dict;
            Debug.Log($"ConfigManager: `{itemType.Name}` 索引创建了 {count} 项");
        }

        /// <summary> 获取指定类型的列表 </summary>
        public List<T> GetList<T>()
        {
            if (_listCache.TryGetValue(typeof(T), out var list))
                return (List<T>)list;

            Debug.LogError($"ConfigManager.GetList: 未找到 `{typeof(T).Name}` 的列表");
            return null;
        }

        /// <summary> 根据 Id 查询对象 </summary>
        public T GetById<T>(int id)
        {
            if (_intIdCache.TryGetValue(typeof(T), out var dictObj)
                && dictObj is Dictionary<int, T> dict
                && dict.TryGetValue(id, out var val))
            {
                return val;
            }

            return default;
        }
    }
}
