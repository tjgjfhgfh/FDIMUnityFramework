using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FDIM.Framework
{
    /// <summary>
    /// 事件心中管理器。用于添加命令的事件、移除命令的事件、发送命令。
    /// 通过这个管理器，在多个对象监听命令后，发送命令，可以让它们自动执行对应的逻辑。
    /// </summary>
    public class EventCenterManager : SingletonPatternBase<EventCenterManager>
    {
        //键表示命令。
        //值表示具体要执行的逻辑。
        Dictionary<string, IEventInfo> eventsDictionary = new Dictionary<string, IEventInfo>();
    
        /// <summary>
        /// 添加一条命令的无参数事件
        /// </summary>
        /// <param name="command">要监听的命令。一般是个自定义的枚举。</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void AddListener(object command, UnityAction call)
        {
            string key = command.GetType().Name + "_" + command.ToString();
            //如果字典中该事件的名字已经存在，则把事件注册进其中，否则在字典中新建一个键值对表示这个事件
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo).action += call;
            else
                eventsDictionary.Add(key, new EventInfo(call));
        }
    
        /// <summary>
        /// 添加一条命令的有1个参数的事件
        /// </summary>
        /// <typeparam name="T">命令的参数的类型</typeparam>
        /// <param name="command">要监听的命令。一般是个自定义的枚举。</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void AddListener<T>(object command, UnityAction<T> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T).Name;
            //如果字典中该事件的名字已经存在，则把事件注册进其中；否则在字典中新建一个键值对表示这个事件
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T>).action += call;
            else
                eventsDictionary.Add(key, new EventInfo<T>(call));
        }
    
        /// <summary>
        /// 添加一条命令的有2个参数的事件
        /// </summary>
        /// <typeparam name="T0">命令的第1个参数的类型</typeparam>
        /// <typeparam name="T1">命令的第2个参数的类型</typeparam>
        /// <param name="command">要监听的命令。一般是个自定义的枚举。</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void AddListener<T0, T1>(object command, UnityAction<T0, T1> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name;
            //如果字典中该事件的名字已经存在，则把事件注册进其中；否则在字典中新建一个键值对表示这个事件
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1>).action += call;
            else
                eventsDictionary.Add(key, new EventInfo<T0, T1>(call));
        }
    
        /// <summary>
        /// 添加一条命令的有3个参数的事件
        /// </summary>
        /// <typeparam name="T0">命令的第1个参数的类型</typeparam>
        /// <typeparam name="T1">命令的第2个参数的类型</typeparam>
        /// <typeparam name="T2">命令的第3个参数的类型</typeparam>
        /// <param name="command">要监听的命令。一般是个自定义的枚举。</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void AddListener<T0, T1, T2>(object command, UnityAction<T0, T1, T2> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name;
            //如果字典中该事件的名字已经存在，则把事件注册进其中；否则在字典中新建一个键值对表示这个事件
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2>).action += call;
            else
                eventsDictionary.Add(key, new EventInfo<T0, T1, T2>(call));
        }
    
        /// <summary>
        /// 添加一条命令的有4个参数的事件
        /// </summary>
        /// <typeparam name="T0">命令的第1个参数的类型</typeparam>
        /// <typeparam name="T1">命令的第2个参数的类型</typeparam>
        /// <typeparam name="T2">命令的第3个参数的类型</typeparam>
        /// <typeparam name="T3">命令的第4个参数的类型</typeparam>
        /// <param name="command">要监听的命令。一般是个自定义的枚举。</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void AddListener<T0, T1, T2, T3>(object command, UnityAction<T0, T1, T2, T3> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name + "_" + typeof(T3).Name;
            //如果字典中该事件的名字已经存在，则把事件注册进其中；否则在字典中新建一个键值对表示这个事件
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2, T3>).action += call;
            else
                eventsDictionary.Add(key, new EventInfo<T0, T1, T2, T3>(call));
        }
    
        /// <summary>
        /// 移除一条命令的无参数事件
        /// </summary>
        /// <param name="command">要取消监听的命令</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void RemoveListener(object command, UnityAction call)
        {
            string key = command.GetType().Name + "_" + command.ToString();
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo).action -= call;
        }
    
        /// <summary>
        /// 移除一条命令的所有无参数事件
        /// </summary>
        /// <param name="command">要取消监听的命令</param>
        public void RemoveListeners(object command)
        {
            string key = command.GetType().Name + "_" + command.ToString();
            //如果字典中存在要移除的命令，则把这个命令的所有事件移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo).action = null;
        }
    
        /// <summary>
        /// 移除一条命令的有1个参数的事件
        /// </summary>
        /// <typeparam name="T">参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void RemoveListener<T>(object command, UnityAction<T> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T>).action -= call;
        }
    
        /// <summary>
        /// 移除一条命令的所有有1个参数的事件
        /// </summary>
        /// <typeparam name="T">参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        public void RemoveListeners<T>(object command)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T).Name;
            //如果字典中存在要移除的命令，则把这个命令的所有事件移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T>).action = null;
        }
    
        /// <summary>
        /// 移除一条命令的有2个参数的事件
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void RemoveListener<T0, T1>(object command, UnityAction<T0, T1> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1>).action -= call;
        }
    
        /// <summary>
        /// 移除一条命令的所有有2个参数的事件
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        public void RemoveListeners<T0, T1>(object command)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1>).action = null;
        }
    
        /// <summary>
        /// 移除一条命令的有3个参数的事件
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第3个参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void RemoveListener<T0, T1, T2>(object command, UnityAction<T0, T1, T2> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2>).action -= call;
        }
    
        /// <summary>
        /// 移除一条命令的所有有3个参数的事件
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第3个参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        public void RemoveListeners<T0, T1, T2>(object command)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2>).action = null;
        }
    
        /// <summary>
        /// 移除一条命令的有4个参数的事件
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第3个参数的类型</typeparam>
        /// <typeparam name="T3">事件的第4个参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        /// <param name="call">接收到命令时要执行的委托方法</param>
        public void RemoveListener<T0, T1, T2, T3>(object command, UnityAction<T0, T1, T2, T3> call)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name + "_" + typeof(T3).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2, T3>).action -= call;
        }
    
        /// <summary>
        /// 移除一条命令的所有有4个参数的事件
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第3个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第4个参数的类型</typeparam>
        /// <param name="command">要取消监听的命令</param>
        public void RemoveListeners<T0, T1, T2, T3>(object command)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name + "_" + typeof(T3).Name;
            //如果字典中存在要移除的命令，则把它移除掉。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2, T3>).action = null;
        }
    
        /// <summary>
        /// 移除事件中心的所有事件。可以考虑在切换场景时调用。
        /// </summary>
        public void RemoveAllListeners()
        {
            eventsDictionary.Clear();
        }
    
        /// <summary>
        /// 发送一条无参数的事件命令。
        /// </summary>
        /// <param name="command">要发送的命令</param>
        public void Dispatch(object command)
        {
            string key = command.GetType().Name + "_" + command.ToString();
            //如果字典中该事件的名字存在，且该事件不为空，则执行该事件，不存在则什么也不做。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo).action?.Invoke();
        }
    
        /// <summary>
        /// 发送一条有1个参数的事件命令
        /// </summary>
        /// <typeparam name="T">参数的类型</typeparam>
        /// <param name="command">要发送的命令</param>
        /// <param name="parameter">这条命令要传递的参数。外部可以根据这个参数的值执行不同的逻辑。</param>
        public void Dispatch<T>(object command, T parameter)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T).Name;
            //如果字典中该事件的名字存在，且该事件不为空，则执行该事件，不存在则什么也不做。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T>).action?.Invoke(parameter);
        }
    
        /// <summary>
        /// 发送一条有2个参数的事件的命令
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <param name="command">要发送的命令</param>
        /// <param name="parameter0">事件的第1个参数</param>
        /// <param name="parameter1">事件的第2个参数</param>
        public void Dispatch<T0, T1>(object command, T0 parameter0, T1 parameter1)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name;
            //如果字典中该事件的名字存在，且该事件不为空，则执行该事件，不存在则什么也不做。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1>).action?.Invoke(parameter0, parameter1);
        }
    
        /// <summary>
        /// 发送一条有3个参数的事件的命令
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第3个参数的类型</typeparam>
        /// <param name="command">要发送的命令</param>
        /// <param name="parameter0">事件的第1个参数</param>
        /// <param name="parameter1">事件的第2个参数</param>
        /// <param name="parameter2">事件的第3个参数</param>
        public void Dispatch<T0, T1, T2>(object command, T0 parameter0, T1 parameter1, T2 parameter2)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name;
            //如果字典中该事件的名字存在，且该事件不为空，则执行该事件，不存在则什么也不做。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2>).action?.Invoke(parameter0, parameter1, parameter2);
        }
    
        /// <summary>
        /// 发送一条有4个参数的事件的命令
        /// </summary>
        /// <typeparam name="T0">事件的第1个参数的类型</typeparam>
        /// <typeparam name="T1">事件的第2个参数的类型</typeparam>
        /// <typeparam name="T2">事件的第3个参数的类型</typeparam>
        /// <typeparam name="T3">事件的第4个参数的类型</typeparam>
        /// <param name="command">要发送的命令</param>
        /// <param name="parameter0">事件的第1个参数</param>
        /// <param name="parameter1">事件的第2个参数</param>
        /// <param name="parameter2">事件的第3个参数</param>
        /// <param name="parameter3">事件的第4个参数</param>
        public void Dispatch<T0, T1, T2, T3>(object command, T0 parameter0, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            string key = command.GetType().Name + "_" + command.ToString() + "_" + typeof(T0).Name + "_" + typeof(T1).Name +
                         "_" + typeof(T2).Name + "_" + typeof(T3).Name;
            //如果字典中该事件的名字存在，且该事件不为空，则执行该事件，不存在则什么也不做。
            if (eventsDictionary.ContainsKey(key))
                (eventsDictionary[key] as EventInfo<T0, T1, T2, T3>).action?.Invoke(parameter0, parameter1, parameter2,
                    parameter3);
        }
    
        /// <summary>
        /// 在控制台打印当前事件中心管理器的事件字典中键值对的信息
        /// </summary>
        public void PrintEventsDictionaryInfo()
        {
            Debug.Log($"当前事件中心管理器的事件字典中有{eventsDictionary.Count}个键值对。");
    
            int i = 0;
            foreach (KeyValuePair<string, IEventInfo> item in eventsDictionary)
            {
                Debug.Log($"第{i}个键是{item.Key}");
                i++;
            }
        }
    
        private interface IEventInfo
        {
        } //用于里氏替换原则
    
        private class EventInfo : IEventInfo
        {
            public UnityAction action;
    
            public EventInfo(UnityAction call)
            {
                action += call;
            }
        }
    
        private class EventInfo<T> : IEventInfo
        {
            public UnityAction<T> action;
    
            public EventInfo(UnityAction<T> call)
            {
                action += call;
            }
        }
    
        private class EventInfo<T0, T1> : IEventInfo
        {
            public UnityAction<T0, T1> action;
    
            public EventInfo(UnityAction<T0, T1> call)
            {
                action += call;
            }
        }
    
        private class EventInfo<T0, T1, T2> : IEventInfo
        {
            public UnityAction<T0, T1, T2> action;
    
            public EventInfo(UnityAction<T0, T1, T2> call)
            {
                action += call;
            }
        }
    
        private class EventInfo<T0, T1, T2, T3> : IEventInfo
        {
            public UnityAction<T0, T1, T2, T3> action;
    
            public EventInfo(UnityAction<T0, T1, T2, T3> call)
            {
                action += call;
            }
        }
    }
}
