#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class FDIMNamespaceTool_InPlace
{
    private const string TargetNs = "FDIM.Framework";
    private const bool NO_BACKUP = true; // ������ .bak���ĳ� false �������ݣ�

    // �����˵�
    [MenuItem("FDIM/Namespace(�͵�)/�����������ռ���ļ����")]
    public static void AddOnly_Menu() => ProcessSelected(addOnly: true);

    [MenuItem("FDIM/Namespace(�͵�)/ǿ��ȫ���滻Ϊ FDIM.Framework")]
    public static void Force_Menu() => ProcessSelected(addOnly: false);

    // �Ҽ��˵���Project ��������ļ�/�ļ��У�
    [MenuItem("Assets/FDIM/Namespace(�͵�)/������������ռ��")]
    public static void AddOnly_Context() => ProcessSelected(addOnly: true);

    [MenuItem("Assets/FDIM/Namespace(�͵�)/ǿ���滻Ϊ FDIM.Framework")]
    public static void Force_Context() => ProcessSelected(addOnly: false);

    // ���� ʵ�� ������������������������������������������������������������������������������������������������
    private static readonly Regex RxHasNs = new(@"^\s*namespace\s+[A-Za-z0-9_.]+", RegexOptions.Multiline);
    private static readonly Regex RxAsmAttr = new(@"\[assembly\s*:", RegexOptions.Multiline);
    private static readonly Regex RxTopUsings = new(@"^(?:\s*using\s+[A-Za-z0-9_.]+;\s*)+", RegexOptions.Multiline);

    private static void ProcessSelected(bool addOnly)
    {
        var roots = CollectRoots();
        if (roots.Count == 0)
        {
            if (!EditorUtility.DisplayDialog("FDIM Namespace",
                "δѡ���κ��ļ��л�ű���\n�Ƿ������ Assets Ŀ¼ִ�У�",
                "�� Assets ִ��", "ȡ��")) return;
            roots.Add("Assets");
        }

        int changed = 0, skipped = 0, total = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var root in roots)
            {
                var guids = AssetDatabase.FindAssets("t:MonoScript", new[] { root });
                foreach (var g in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(g);
                    if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;

                    var text = File.ReadAllText(path, Encoding.UTF8);
                    total++;

                    // ������Ҫ�������ռ�����ļ�
                    if (RxAsmAttr.IsMatch(text)) { skipped++; continue; }

                    string newText = null;

                    if (addOnly)
                    {
                        if (RxHasNs.IsMatch(text)) { skipped++; continue; }
                        newText = WrapWithNamespace(text, TargetNs);
                    }
                    else
                    {
                        if (RxHasNs.IsMatch(text))
                            newText = Regex.Replace(text, @"^\s*namespace\s+[A-Za-z0-9_.]+",
                                                    $"namespace {TargetNs}", RegexOptions.Multiline);
                        else
                            newText = WrapWithNamespace(text, TargetNs);
                    }

                    if (newText != null && newText != text)
                    {
                        if (!NO_BACKUP)
                        {
                            var bak = path + ".bak";
                            if (!File.Exists(bak)) File.WriteAllText(bak, text, Encoding.UTF8);
                        }
                        File.WriteAllText(path, newText, Encoding.UTF8);
                        changed++;
                    }
                    else skipped++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
        Debug.Log($"[FDIM] �͵ش�����ɣ���={string.Join(", ", roots)}���� {total}���޸� {changed}������ {skipped}��");
    }

    private static HashSet<string> CollectRoots()
    {
        var roots = new HashSet<string>();
        foreach (var guid in Selection.assetGUIDs)
        {
            var p = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(p)) continue;
            if (AssetDatabase.IsValidFolder(p)) roots.Add(p);
            else if (p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                roots.Add(Path.GetDirectoryName(p).Replace('\\', '/'));
        }
        return roots;
    }

    private static string WrapWithNamespace(string text, string ns)
    {
        var m = RxTopUsings.Match(text);
        int insertPos = m.Success ? m.Index + m.Length : 0;

        var head = text[..insertPos];
        var body = text[insertPos..];

        // �������м�����
        var indented = string.Join("\n", body.Replace("\r\n", "\n")
                                   .Split('\n').Select(line => "    " + line));

        return $"{head}{(head.Length > 0 && !head.EndsWith("\n") ? "\n" : "")}" +
               $"namespace {ns}\n{{\n{indented}{(indented.EndsWith("\n") ? "" : "\n")}}}\n";
    }
}
#endif
