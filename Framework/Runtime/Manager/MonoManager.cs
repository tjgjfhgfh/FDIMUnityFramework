using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FDIM.Framework
{
    /// <summary>
    /// 作用：让不继承MonoBehaviour类可以开启协程，可以用FixedUpdate、Update、LateUpdate方法更新。
    /// </summary>
    public class MonoManager : SingletonPatternBase<MonoManager>
    {
        private MonoManager()
        {
        }
    
        private static bool isApplicationQuitting = false;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitQuitHandler()
        {
            Application.quitting += () => isApplicationQuitting = true;
        }
    
        private MonoController monoExecuter;
    
        private MonoController MonoExecuter
        {
            get
            {
                if (monoExecuter == null && !isApplicationQuitting)
                {
                    GameObject go = new GameObject(typeof(MonoController).Name);
                    monoExecuter = go.AddComponent<MonoController>();
                }
    
                return monoExecuter;
            }
        }
    
        public void AddFixedUpdateListener(UnityAction call) => MonoExecuter.AddFixedUpdateListener(call);
        public void RemoveFixedUpdateListener(UnityAction call) => MonoExecuter.RemoveFixedUpdateListener(call);
        public void RemoveAllFixedUpdateListeners() => MonoExecuter.RemoveAllFixedUpdateListeners();
    
        public void AddUpdateListener(UnityAction call) => MonoExecuter.AddUpdateListener(call);
        public void RemoveUpdateListener(UnityAction call) => MonoExecuter.RemoveUpdateListener(call);
        public void RemoveAllUpdateListeners() => MonoExecuter.RemoveAllUpdateListeners();
    
        public void AddLateUpdateListener(UnityAction call) => MonoExecuter.AddLateUpdateListener(call);
        public void RemoveLateUpdateListener(UnityAction call) => MonoExecuter.RemoveLateUpdateListener(call);
        public void RemoveAllLateUpdateListeners() => MonoExecuter.RemoveAllLateUpdateListeners();
    
        public void RemoveAllListeners() => MonoExecuter.RemoveAllListeners();
    
        public Coroutine StartCoroutine(IEnumerator routine) => MonoExecuter.StartCoroutine(routine);
    
        public void StopCoroutine(IEnumerator routine)
        {
            if (routine != null)
                MonoExecuter.StopCoroutine(routine);
        }
    
        public void StopCoroutine(Coroutine routine)
        {
            if (routine != null)
                MonoExecuter.StopCoroutine(routine);
        }
    
        public void StopAllCoroutines() => MonoExecuter.StopAllCoroutines();
    
        // 内部控制器类
        private class MonoController : MonoBehaviour
        {
            event UnityAction updateEvent;
            event UnityAction fixedUpdateEvent;
            event UnityAction lateUpdateEvent;
    
            void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }
    
            void FixedUpdate() => fixedUpdateEvent?.Invoke();
            void Update() => updateEvent?.Invoke();
            void LateUpdate() => lateUpdateEvent?.Invoke();
    
            public void AddFixedUpdateListener(UnityAction call) => fixedUpdateEvent += call;
            public void RemoveFixedUpdateListener(UnityAction call) => fixedUpdateEvent -= call;
            public void RemoveAllFixedUpdateListeners() => fixedUpdateEvent = null;
    
            public void AddUpdateListener(UnityAction call) => updateEvent += call;
            public void RemoveUpdateListener(UnityAction call) => updateEvent -= call;
            public void RemoveAllUpdateListeners() => updateEvent = null;
    
            public void AddLateUpdateListener(UnityAction call) => lateUpdateEvent += call;
            public void RemoveLateUpdateListener(UnityAction call) => lateUpdateEvent -= call;
            public void RemoveAllLateUpdateListeners() => lateUpdateEvent = null;
    
            public void RemoveAllListeners()
            {
                RemoveAllFixedUpdateListeners();
                RemoveAllUpdateListeners();
                RemoveAllLateUpdateListeners();
            }
        }
    }
}
