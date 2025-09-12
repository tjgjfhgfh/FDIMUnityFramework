using UnityEngine;


namespace FDIM.Framework
{
    /// <summary>
    /// ��չ����
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
                if (child != parent) // �ų�����
                {
                    Debug.Log("������: " + child.name);
                }
            }
        }
    
    
        /// <summary>
        /// ����transform�����µ�childName������
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform FindDeepChild(this Transform transform, string childName)
        {
            // ��������ֱ��������
            foreach (Transform child in transform)
            {
                // ����ҵ�Ŀ�����壬������ Transform
                if (child.name == childName)
                {
                    return child;
                }
    
                // �ݹ���������
                Transform foundChild = FindDeepChild(child, childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
    
            return null; // ���û���ҵ������� null
        }
    }
}
