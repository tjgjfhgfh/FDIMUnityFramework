using UnityEngine;

public static class TransformExtensions
{
    public static Transform SetPosition(this Transform t, Vector3 pos)
    {
        t.position = pos;
        return t;
    }

    public static Transform SetLocalPosition(this Transform t, Vector3 pos)
    {
        t.localPosition = pos;
        return t;
    }

    public static Transform SetRotation(this Transform t, Quaternion rot)
    {
        t.rotation = rot;
        return t;
    }

    public static Transform SetLocalRotation(this Transform t, Quaternion rot)
    {
        t.localRotation = rot;
        return t;
    }

    public static Transform SetScale(this Transform t, Vector3 scale)
    {
        t.localScale = scale;
        return t;
    }

    public static Transform SetParentEx(this Transform t, Transform parent)
    {
        t.SetParent(parent);
        return t;
    }
}
