using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject SetActiveEx(this GameObject go, bool active)
    {
        go.SetActive(active);
        return go;
    }

    public static GameObject SetName(this GameObject go, string name)
    {
        go.name = name;
        return go;
    }

    public static GameObject SetParentEx(this GameObject go, Transform parent)
    {
        go.transform.SetParent(parent);
        return go;
    }
}
