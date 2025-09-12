#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public static class FDIMEmbedGuard
{
    // �� package.json �� "name" ��ȫһ��
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
                "FDIM ����",
                $"��ǰ����Դ��{pkg.source}\n·����{pkg.assetPath}\n����� Git URL ���̶� tag/commit��",
                "��֪����"
            );
        }
    }
}
#endif
