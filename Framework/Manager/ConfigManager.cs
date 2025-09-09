using LitJson;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ConfigManager : SingletonPatternBase<ConfigManager>
{
    [Header("数据格式切换")] [Tooltip("勾选后优先从 .bytes (Protobuf) 解析；否则走 JSON 解析")]
    public bool UseBinary = true;

    private GameData _gameData;
    private readonly Dictionary<Type, IList> _listCache = new();
    private readonly Dictionary<Type, object> _intIdCache = new();

    public void Init(TextAsset  asset)
    {
        Debug.Log($"ConfigManager: 开始初始化 (UseBinary={UseBinary})");
        var  ta=asset;
        // 1. 反序列化 JSON 或 Protobuf
        if (UseBinary)
        {
            if (ta == null)
            {
                Debug.LogError("ConfigManager: 找不到 GameData.bytes");
                return;
            }

            Debug.Log($"[Runtime] Loaded bytes length = {ta.bytes.Length}");
            try
            {
                using (var ms = new System.IO.MemoryStream(ta.bytes))
                {
                    _gameData = Serializer.Deserialize<GameData>(ms);
                }

                Debug.Log(
                    $"[Runtime] Protobuf 反序列化后 SceneIn = {_gameData.SceneIn.Count}, Keys = {_gameData.Keys.Count}");
            }
            catch (Exception e)
            {
                Debug.LogError($"ConfigManager: Protobuf 反序列化失败：{e}");
                return;
            }
        }
        else
        {
            if (ta == null)
            {
                Debug.LogError("ConfigManager: 找不到 GameData_Json.json");
                return;
            }

            Debug.Log($"[Runtime] Loaded JSON length = {ta.text.Length}");
            try
            {
                _gameData = JsonMapper.ToObject<GameData>(ta.text);
                Debug.Log($"[Runtime] JSON 反序列化后 SceneIn = {_gameData.SceneIn.Count}, Keys = {_gameData.Keys.Count}");
            }
            catch (Exception e)
            {
                Debug.LogError($"ConfigManager: JSON 反序列化失败：{e}");
                return;
            }
        }

        // 2. 构建缓存与索引
        BuildCaches();
    }

    private void BuildCaches()
    {
        _listCache.Clear();
        _intIdCache.Clear();
        int tableCount = 0;

        Type dataType = typeof(GameData);

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
                value = pi.GetValue(_gameData);
            }
            else if (m is FieldInfo fi)
            {
                memberType = fi.FieldType;
                value = fi.GetValue(_gameData);
            }
            else continue;

            // 只处理 List<>
            if (!memberType.IsGenericType || memberType.GetGenericTypeDefinition() != typeof(List<>))
                continue;

            var list = value as IList;
            if (list == null) continue;

            Type itemType = memberType.GetGenericArguments()[0];
            _listCache[itemType] = list;
            tableCount++;

            // —— 先找 public 属性 Id，再找 public 字段 Id —— 
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
                Debug.LogWarning($"ConfigManager: 类型 `{itemType.Name}` 没有整型 Id 字段或属性，跳过索引");
            }
        }

        Debug.Log($"ConfigManager: 初始化完成，共处理 {tableCount} 张表");
    }

    // 辅助：根据 keySelector 构建 Dictionary<int,T>
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
        Debug.Log($"ConfigManager: `{itemType.Name}` 索引 {count} 条");
    }

    /// <summary> 获取整张表 </summary>
    public List<T> GetList<T>()
    {
        if (_listCache.TryGetValue(typeof(T), out var list))
            return (List<T>)list;

        Debug.LogError($"ConfigManager.GetList: 未找到表 `{typeof(T).Name}`");
        return null;
    }

    /// <summary> 按整型 Id 查询 </summary>
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