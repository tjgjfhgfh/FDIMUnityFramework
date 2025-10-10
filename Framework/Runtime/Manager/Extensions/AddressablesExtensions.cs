using UnityEngine;

namespace FDIM.Framework
{
    /// <summary>
    /// 扩展方法工具类
    /// </summary>
    public static class AddressablesExtensions
    {
        /// <summary>
        /// 通过Id直接返回路径
        /// </summary>
        public static T GetByPath<T>(this int id)
        {
            return Managers.ConfigManager.GetById<T>(id);
        }

    }
}
