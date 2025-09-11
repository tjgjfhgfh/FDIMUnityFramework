using UnityEngine;


/// <summary>
/// 扩展函数
/// </summary>
public static class SceneHelper
{
    public static int IntAddOne(this int number)
    {
        return number + 1;
    }


    public static void FindAllChildrenRecursive(this Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child != parent) // 排除自身
            {
                Debug.Log("子物体: " + child.name);
            }
        }
    }


    /// <summary>
    /// 遍历transform物体下的childName的物体
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static Transform FindDeepChild(this Transform transform, string childName)
    {
        // 遍历所有直接子物体
        foreach (Transform child in transform)
        {
            // 如果找到目标物体，返回其 Transform
            if (child.name == childName)
            {
                return child;
            }

            // 递归检查子物体
            Transform foundChild = FindDeepChild(child, childName);
            if (foundChild != null)
            {
                return foundChild;
            }
        }

        return null; // 如果没有找到，返回 null
    }
}