using System;
using System.Collections.Generic;
using UnityEngine;
namespace FDIM.Framework

{
    public class TimerManager
    {
        private List<Timer> _timers;
        private static TimerManager _instance;

        public static TimerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TimerManager();
                }

                return _instance;
            }
        }

        public TimerManager()
        {
            _timers = new List<Timer>();
            // 注册 Update 方法到 MonoManager 的更新循环中
            Managers.MonoManager.AddUpdateListener(Update);
        }

        /// <summary>
        /// 创建一个简单的计时器
        /// </summary>
        public Timer CreateTimer(TimerHandler callBack, float time, int repeats = 1,
            Transform associatedTransform = null)
        {
            return Create(callBack, null, time, repeats, associatedTransform);
        }

        /// <summary>
        /// 创建一个带参数的计时器
        /// </summary>
        public Timer CreateTimerWithArgs(TimerArgsHandler callBack, float time, int repeats,
            Transform associatedTransform = null, params object[] args)
        {
            return Create(null, callBack, time, repeats, associatedTransform, args);
        }

        private Timer Create(TimerHandler callBack, TimerArgsHandler callBackArgs, float time, int repeats,
            Transform associatedTransform, params object[] args)
        {
            Timer timer = new Timer(callBack, callBackArgs, time, repeats, args, associatedTransform);
            _timers.Add(timer);
            return timer;
        }

        public Timer DestroyTimer(Timer timer)
        {
            if (timer != null)
            {
                _timers.Remove(timer);
                timer.CleanUp();
                timer = null;
            }

            return timer;
        }

        public void ClearAll()
        {
            if (_timers != null)
            {
                for (int i = 0; i < _timers.Count; i++)
                {
                    _timers[i].CleanUp();
                }

                _timers.Clear();
            }
        }

        /// <summary>
        /// 清理与指定 Transform 关联的所有计时器
        /// </summary>
        public void Clear(Transform associatedTransform)
        {
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                if (_timers[i].AssociatedTransform == associatedTransform)
                {
                    DestroyTimer(_timers[i]);
                }
            }
        }

        /// <summary>
        /// 固定更新检查更新的频率
        /// </summary>
        void Update()
        {
            if (_timers != null && _timers.Count != 0)
            {
                for (int i = _timers.Count - 1; i >= 0; i--)
                {
                    Timer timer = _timers[i];
                    float curTime = Time.time;
                    if (timer.Frequency + timer.LastTickTime > curTime)
                    {
                        continue;
                    }

                    timer.LastTickTime = curTime;
                    if (timer.Repeats-- == 0)
                    {
                        DestroyTimer(timer);
                    }
                    else
                    {
                        timer.Notify();
                    }
                }
            }
        }
    }

    public delegate void TimerHandler();

    public delegate void TimerArgsHandler(object[] args);

    public class Timer
    {
        public TimerHandler Handler; // 无参的委托
        public TimerArgsHandler ArgsHandler; // 带参数的委托
        public float Frequency; // 时间间隔
        public int Repeats; // 重复次数
        public object[] Args;
        public float LastTickTime;
        public Transform AssociatedTransform; // 关联的 Transform

        public event Action OnComplete; // 计时器完成一次工作时的事件
        public event Action OnDestroy; // 计时器被销毁时的事件

        public Timer()
        {
        }

        /// <summary>
        /// 创建一个时间事件对象
        /// </summary>
        public Timer(TimerHandler handler, TimerArgsHandler argsHandler, float frequency, int repeats, object[] args,
            Transform associatedTransform)
        {
            this.Handler = handler;
            this.ArgsHandler = argsHandler;
            this.Frequency = frequency;
            this.Repeats = repeats == 0 ? 1 : repeats;
            this.Args = args;
            this.LastTickTime = Time.time;
            this.AssociatedTransform = associatedTransform;
        }

        public void Notify()
        {
            Handler?.Invoke();
            ArgsHandler?.Invoke(Args);
            OnComplete?.Invoke();
        }

        /// <summary>
        /// 清理计时器，初始化参数并清理事件
        /// </summary>
        public void CleanUp()
        {
            Handler = null;
            ArgsHandler = null;
            Repeats = 1;
            Frequency = 0;
            OnDestroy?.Invoke();
            OnDestroy = null;
            OnComplete = null;
        }
    }


    public static class TimerManagerExtensions
    {
        /// <summary>
        /// 直接在 Transform 上创建一个计时器，自动将当前 Transform 作为关联对象传入。无参回调！
        /// </summary>
        public static Timer CreateTimer(this Transform transform, TimerHandler callback, float time, int repeats = 1)
        {
            return TimerManager.Instance.CreateTimer(callback, time, repeats, transform);
        }

        /// <summary>
        /// 直接在 Transform 上创建一个带参数的计时器，自动将当前 Transform 作为关联对象传入
        /// </summary>
        public static Timer CreateTimerWithArgs(this Transform transform, TimerArgsHandler callback, float time,
            int repeats, params object[] args)
        {
            return TimerManager.Instance.CreateTimerWithArgs(callback, time, repeats, transform, args);
        }

        /// <summary>
        /// 直接在 Transform 上调用，清理与当前 Transform 关联的所有计时器。有参回调！
        /// </summary>
        public static void ClearTimer(this Transform transform)
        {
            if (transform == null)
            {
                Debug.LogWarning("ClearTimer: 尝试对一个 null 的 Transform 清理计时器。");
                return;
            }

            TimerManager.Instance.Clear(transform);
        }
    }
}