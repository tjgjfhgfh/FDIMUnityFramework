using UnityEngine;

public static class TransformExtensions
{
    public static Transform SetActiveEx(this Transform t, bool active)
    {
        t.gameObject.SetActive(active);
        return t;
    }

    public static Transform SetNameEx(this Transform t, string name)
    {
        t.gameObject.name = name;
        return t;
    }

    public static Transform SetParentEx(this Transform t, Transform parent, bool worldPositionStays = true)
    {
        t.SetParent(parent, worldPositionStays);
        return t;
    }

    public static Transform SetLayerEx(this Transform t, int layer)
    {
        t.gameObject.layer = layer;
        return t;
    }

    public static Transform SetTagEx(this Transform t, string tag)
    {
        t.gameObject.tag = tag;
        return t;
    }

    public static Transform SetPositionEx(this Transform t, Vector3 pos)
    {
        t.position = pos;
        return t;
    }

    public static Transform SetLocalPositionEx(this Transform t, Vector3 pos)
    {
        t.localPosition = pos;
        return t;
    }

    public static Transform SetRotationEx(this Transform t, Quaternion rot)
    {
        t.rotation = rot;
        return t;
    }

    public static Transform SetLocalRotationEx(this Transform t, Quaternion rot)
    {
        t.localRotation = rot;
        return t;
    }

    public static Transform SetScaleEx(this Transform t, Vector3 scale)
    {
        t.localScale = scale;
        return t;
    }
}
