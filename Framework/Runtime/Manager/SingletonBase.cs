using UnityEngine;
namespace FDIM.Framework
{
    public class SingletonBase<T> where T : new()
    {
        private static T instance;
        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }

                return instance;
            }
        }
    }

    public class SingletonMonoBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    // 如果在场景中找不到该类型的对象，则创建一个新对象
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        Debug.Log(singletonObject.name);
                        instance = singletonObject.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        public bool destoryOnLoadScene;


        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                DestroyImmediate(gameObject);          // ← 立刻销毁
                return;                                // ← 关键！别再往下走
            }
            instance = this as T;
            if (!destoryOnLoadScene)
                DontDestroyOnLoad(gameObject);
        }
    }
}