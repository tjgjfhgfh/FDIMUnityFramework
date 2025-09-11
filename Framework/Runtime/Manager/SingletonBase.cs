using UnityEngine;

namespace Txx
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
                        instance = singletonObject.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        public bool destoryOnLoadScene;

        protected virtual void Awake()
        {
            // 如果该对象不是单例对象，则销毁它
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                // 将当前对象赋值给单例对象
                instance = this as T;
                if (!destoryOnLoadScene)
                    // 确保该对象在场景切换时不被销毁
                    DontDestroyOnLoad(gameObject);
            }
        }
    }
}