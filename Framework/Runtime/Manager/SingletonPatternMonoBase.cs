using UnityEngine;

namespace FDIM.Framework
{
    /// <summary>
    /// <para>继承MonoBehaviour的单例模式基类。</para>
    /// <para>作用：继承这个类就继承了MonoBehaviour，并且自带单例模式。</para>
    /// <para>使用方法：让类A继承这个类，继承时，泛型写类A。外部要访问单例模式的类的对象，只需通过类名.Instance.成员名来访问。</para>
    /// </summary>
    public class SingletonPatternMonoBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        //禁止外部new这个类的对象。
        //但是继承这个类的类依然可以new对象，如果要让它们也无法new对象，则需要手动把它们的构造函数也设为private修饰的。
        protected SingletonPatternMonoBase()
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
    
                    if (instance != null)
                        IsExisted = true;
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
