using UnityEngine;

namespace FDIM.Framework
{
    /// <summary>
    /// <para>继承MonoBehaviour的自动单例模式基类。</para>
    /// <para>切换场景也不会销毁，在整个游戏过程中会一直存在。</para>
    /// </summary>
    public class SingletonPatternMonoAutoBase_DontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
    {
        //禁止外部new这个类的对象。
        //但是继承这个类的类依然可以new对象，如果要让它们也无法new对象，则需要手动把它们的构造函数也设为private修饰的。
        protected SingletonPatternMonoAutoBase_DontDestroyOnLoad()
        {
        }
    
        //记录单例对象是否存在。用于防止在OnDestroy函数中访问单例对象而报错。
        //在OnDestroy函数中访问单例对象之前，应使用if语句判断这个变量是否为true。如果为true，再访问该单例对象。
        public static bool IsExisted { get; private set; } = false;
    
        //提供一个属性给外部访问，这个属性就相当于唯一的单例对象。
        private static T instance;
    
        public static T Instance
        {
            get
            {
                //如果单例对象为null，则会尝试在场景中查找并获取该脚本作为单例对象。
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    //如果单例对象还是为null，说明场景中不存在该脚本，此时会创建一个空物体并挂载该脚本，并把该脚本作为单例对象。
                    if (instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go); //让物体不会因切换场景而销毁。
                        IsExisted = true;
                    }
                }
    
                //如果单例对象不为null，则直接返回它。
                return instance;
            }
        }
    
        /// <summary>
        /// 重写后的逻辑必须写在base.OnDestroy函数之后。
        /// </summary>
        protected virtual void OnDestroy()
        {
            IsExisted = false;
        }
    }
}
