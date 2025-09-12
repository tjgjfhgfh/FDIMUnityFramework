#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FDIMEmbedGuard
{
    static FDIMEmbedGuard()
    {
        string embeddedPath = "Packages/com.fdim.framework";
        if (Directory.Exists(embeddedPath))
        {
            Debug.LogError("[FDIM] 检测到包被 Embedded（可写）。请改用 Git URL 引入，并固定 tag/commit。");
            EditorUtility.DisplayDialog("FDIM 警告",
                "检测到包被 Embedded（可写）。\n请改为使用 Git URL，并固定 tag/commit。",
                "我知道了");
        }
    }
}
#endif
