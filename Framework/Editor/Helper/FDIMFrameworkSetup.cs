#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FDIMFrameworkSetup
{
    [MenuItem("FDIM/Setup/Create Consumer asmdef")]
    public static void CreateConsumerAsmdef()
    {
        // 1) 找到 FDIM.Framework.asmdef 的 GUID
        string[] guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset FDIM.Framework");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("FDIM", "未找到 FDIM.Framework.asmdef，请确认包已安装。", "OK");
            return;
        }
        string frameworkGuid = guids[0];

        // 2) 目标存放位置
        string targetDir = "Assets/Game";
        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

        string asmdefPath = Path.Combine(targetDir, "Game.Runtime.asmdef");
        if (File.Exists(asmdefPath))
        {
            if (!EditorUtility.DisplayDialog("FDIM",
                "已存在 Game.Runtime.asmdef。要覆盖吗？", "覆盖", "取消")) return;
        }

        // 3) 生成 asmdef JSON，引用 FDIM.Framework（用 GUID）
        string json = @"{
  ""name"": ""Game.Runtime"",
  ""references"": [ ""GUID:" + frameworkGuid + @""" ],
  ""includePlatforms"": [],
  ""excludePlatforms"": [],
  ""allowUnsafeCode"": false,
  ""overrideReferences"": false,
  ""precompiledReferences"": [],
  ""autoReferenced"": true,
  ""defineConstraints"": [],
  ""noEngineReferences"": false
}";
        File.WriteAllText(asmdefPath, json);
        AssetDatabase.ImportAsset(asmdefPath);
        EditorUtility.DisplayDialog("FDIM", "已创建并引用 FDIM.Framework。等待编译完成即可使用。", "OK");
    }
}
#endif
