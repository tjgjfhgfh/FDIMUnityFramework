using UnityEngine;

namespace FDIM.Framework
{
    /// <summary>
    /// 扩展方法工具类
    /// </summary>
    public static class FDIMExtensions
    {

        /// <summary>
        /// 通过Id直接返回路径
        /// </summary>
        public static T GetByPath<T>(this int id)
        {
            return Managers.ConfigManager.GetById<T>(id);
        }

        /// <summary>
        /// 在transform下递归查找指定名字的子对象
        /// </summary>
        /// <param name="transform">父Transform</param>
        /// <param name="childName">子对象名字</param>
        /// <returns>找到的Transform或null</returns>
        public static Transform FindDeepChild(this Transform transform, string childName)
        {
            // 先遍历直接子对象
            foreach (Transform child in transform)
            {
                // 找到目标子对象，返回该Transform
                if (child.name == childName)
                {
                    return child;
                }

                // 递归查找子对象
                Transform foundChild = FindDeepChild(child, childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null; // 如果未找到，返回null
        }
    }
}
