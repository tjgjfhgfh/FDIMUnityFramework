using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject SetActiveEx(this GameObject go, bool active)
    {
        go.SetActive(active);
        return go;
    }

    public static GameObject SetNameEx(this GameObject go, string name)
    {
        go.name = name;
        return go;
    }

    public static GameObject SetParentEx(this GameObject go, Transform parent, bool worldPositionStays = true)
    {
        go.transform.SetParent(parent, worldPositionStays);
        return go;
    }

    public static GameObject SetLayerEx(this GameObject go, int layer)
    {
        go.layer = layer;
        return go;
    }

    public static GameObject SetTagEx(this GameObject go, string tag)
    {
        go.tag = tag;
        return go;
    }

    public static GameObject SetPositionEx(this GameObject go, Vector3 pos)
    {
        go.transform.position = pos;
        return go;
    }

    public static GameObject SetLocalPositionEx(this GameObject go, Vector3 pos)
    {
        go.transform.localPosition = pos;
        return go;
    }

    public static GameObject SetRotationEx(this GameObject go, Quaternion rot)
    {
        go.transform.rotation = rot;
        return go;
    }

    public static GameObject SetLocalRotationEx(this GameObject go, Quaternion rot)
    {
        go.transform.localRotation = rot;
        return go;
    }

    public static GameObject SetScaleEx(this GameObject go, Vector3 scale)
    {
        go.transform.localScale = scale;
        return go;
    }

}
