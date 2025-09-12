#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FDIMFrameworkSetup
{
    [MenuItem("FDIM/Setup/Create Consumer asmdef")]
    public static void CreateConsumerAsmdef()
    {
        // 1) �ҵ� FDIM.Framework.asmdef �� GUID
        string[] guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset FDIM.Framework");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("FDIM", "δ�ҵ� FDIM.Framework.asmdef����ȷ�ϰ��Ѱ�װ��", "OK");
            return;
        }
        string frameworkGuid = guids[0];

        // 2) Ŀ����λ��
        string targetDir = "Assets/Game";
        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

        string asmdefPath = Path.Combine(targetDir, "Game.Runtime.asmdef");
        if (File.Exists(asmdefPath))
        {
            if (!EditorUtility.DisplayDialog("FDIM",
                "�Ѵ��� Game.Runtime.asmdef��Ҫ������", "����", "ȡ��")) return;
        }

        // 3) ���� asmdef JSON������ FDIM.Framework���� GUID��
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
        EditorUtility.DisplayDialog("FDIM", "�Ѵ��������� FDIM.Framework���ȴ�������ɼ���ʹ�á�", "OK");
    }
}
#endif
