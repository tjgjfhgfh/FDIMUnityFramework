using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
public static class HybridCLRInstaller
{
    // AddRequest 用于拉取 Package
    static AddRequest addRequest;

    // 类加载时自动执行
    static HybridCLRInstaller()
    {
        EditorApplication.update += CheckHybridCLR;
    }

    // 检查 HybridCLR 是否安装
    private static void CheckHybridCLR()
    {
        EditorApplication.update -= CheckHybridCLR;

        if (!IsHybridCLRInstalled())
        {
            // 弹窗提示用户
            bool confirmed = EditorUtility.DisplayDialog(
                "HybridCLR 未安装",
                "检测到项目未拉取 HybridCLR，是否自动添加？",
                "确定",
                "取消"
            );

            if (confirmed)
            {
                // Git 仓库地址，可替换成你自己的 URL
                string packageUrl = "https://github.com/focus-creative-games/hybridclr_unity.git";

                // 开始拉取 Package
                addRequest = Client.Add(packageUrl);

                // 注册更新回调
                EditorApplication.update += MonitorAddRequest;
            }
        }
    }

    // 监控 Package 拉取进度
    private static void MonitorAddRequest()
    {
        if (addRequest.IsCompleted)
        {
            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log("HybridCLR 安装成功！");
            }
            else
            {
                Debug.LogError("HybridCLR 安装失败: " + addRequest.Error.message);
            }

            // 刷新 AssetDatabase
            AssetDatabase.Refresh();

            // 取消回调
            EditorApplication.update -= MonitorAddRequest;
        }
    }

    // 判断 HybridCLR 是否已安装
    private static bool IsHybridCLRInstalled()
    {
        string manifestPath = "Packages/manifest.json";
        if (!System.IO.File.Exists(manifestPath))
            return false;

        string manifestContent = System.IO.File.ReadAllText(manifestPath, System.Text.Encoding.UTF8);
        // 判断 manifest 中是否包含 hybridclr_unity
        return manifestContent.Contains("hybridclr_unity");
    }
}
