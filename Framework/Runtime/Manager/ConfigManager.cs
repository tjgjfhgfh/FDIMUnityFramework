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
        [Header("���ݸ�ʽ�л�")] [Tooltip("��ѡ�����ȴ� .bytes (Protobuf) ������������ JSON ����")]
        public bool UseBinary = true;
    
        private GameData _gameData;
        private readonly Dictionary<Type, IList> _listCache = new();
        private readonly Dictionary<Type, object> _intIdCache = new();
    
        public void Init(TextAsset  asset)
        {
            Debug.Log($"ConfigManager: ��ʼ��ʼ�� (UseBinary={UseBinary})");
            var  ta=asset;
            // 1. �����л� JSON �� Protobuf
            if (UseBinary)
            {
                if (ta == null)
                {
                    Debug.LogError("ConfigManager: �Ҳ��� GameData.bytes");
                    return;
                }
    
                Debug.Log($"[Runtime] Loaded bytes length = {ta.bytes.Length}");
                try
                {
                    using (var ms = new System.IO.MemoryStream(ta.bytes))
                    {
                        _gameData = Serializer.Deserialize<GameData>(ms);
                    }
    
                    //Debug.Log(
                    //    $"[Runtime] Protobuf �����л��� SceneIn = {_gameData.SceneIn.Count}, Keys = {_gameData.Keys.Count}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"ConfigManager: Protobuf �����л�ʧ�ܣ�{e}");
                    return;
                }
            }
            else
            {
                if (ta == null)
                {
                    Debug.LogError("ConfigManager: �Ҳ��� GameData_Json.json");
                    return;
                }
    
                Debug.Log($"[Runtime] Loaded JSON length = {ta.text.Length}");
                try
                {
                    _gameData = JsonMapper.ToObject<GameData>(ta.text);
                    //Debug.Log($"[Runtime] JSON �����л��� SceneIn = {_gameData.SceneIn.Count}, Keys = {_gameData.Keys.Count}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"ConfigManager: JSON �����л�ʧ�ܣ�{e}");
                    return;
                }
            }
    
            // 2. ��������������
            BuildCaches();
        }
    
        private void BuildCaches()
        {
            _listCache.Clear();
            _intIdCache.Clear();
            int tableCount = 0;
    
            Type dataType = typeof(GameData);
    
            // ֧�� public ���� �� public �ֶ�
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
    
                // ֻ���� List<>
                if (!memberType.IsGenericType || memberType.GetGenericTypeDefinition() != typeof(List<>))
                    continue;
    
                var list = value as IList;
                if (list == null) continue;
    
                Type itemType = memberType.GetGenericArguments()[0];
                _listCache[itemType] = list;
                tableCount++;
    
                // ���� ���� public ���� Id������ public �ֶ� Id ���� 
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
                    Debug.LogWarning($"ConfigManager: ���� `{itemType.Name}` û������ Id �ֶλ����ԣ���������");
                }
            }
    
            Debug.Log($"ConfigManager: ��ʼ����ɣ������� {tableCount} �ű�");
        }
    
        // ���������� keySelector ���� Dictionary<int,T>
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
            Debug.Log($"ConfigManager: `{itemType.Name}` ���� {count} ��");
        }
    
        /// <summary> ��ȡ���ű� </summary>
        public List<T> GetList<T>()
        {
            if (_listCache.TryGetValue(typeof(T), out var list))
                return (List<T>)list;
    
            Debug.LogError($"ConfigManager.GetList: δ�ҵ��� `{typeof(T).Name}`");
            return null;
        }
    
        /// <summary> ������ Id ��ѯ </summary>
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
