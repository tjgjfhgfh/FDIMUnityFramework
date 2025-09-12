#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public static class FDIMEmbedGuard
{
    // 与 package.json 的 "name" 完全一致
    private const string PackageName = "com.fdim.framework";

    static FDIMEmbedGuard()
    {
        EditorApplication.delayCall += Check;
    }

    private static void Check()
    {
        var pkgs = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
        var pkg = pkgs.FirstOrDefault(p => p.name == PackageName);
        if (pkg == null) return;

        if (pkg.source == UnityEditor.PackageManager.PackageSource.Embedded ||
            pkg.source == UnityEditor.PackageManager.PackageSource.Local)
        {
            EditorUtility.DisplayDialog(
                "FDIM 警告",
                $"当前包来源：{pkg.source}\n路径：{pkg.assetPath}\n请改用 Git URL 并固定 tag/commit。",
                "我知道了"
            );
        }
    }
}
#endif
