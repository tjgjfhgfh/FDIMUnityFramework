using FDIM.Framework;
using LitJson;
using OfficeOpenXml;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ExcelToCSharpEditor : EditorWindow
{
    private enum Mode
    {
        Classes,
        Data
    }

    private enum DataFormat
    {
        JSON,
        BINARY
    }

    [SerializeField] private Mode currentMode = Mode.Classes;
    [SerializeField] private string excelFilePath = "";
    [SerializeField] private string outputDir = "";
    [SerializeField] private bool prettyJson = false;
    [SerializeField] private DataFormat dataFormat = DataFormat.JSON;

    private const string PrefsExcelPathKey = "ExcelToCSharpEditor_LastExcelPath";
    private const string PrefsOutputDirKey = "ExcelToCSharpEditor_LastOutputDir";

    [MenuItem("Tools/Data/Excel To C# Classes")]
    public static void ShowWindowForClasses() => ShowWindow(Mode.Classes);

    [MenuItem("Tools/Data/Excel To JSON OR BIN")]
    public static void ShowWindowForData() => ShowWindow(Mode.Data);

    private static void ShowWindow(Mode mode)
    {
        var window = GetWindow<ExcelToCSharpEditor>();
        window.titleContent = new GUIContent(mode == Mode.Classes ? "Excel To C#" : "Excel To Data");
        window.currentMode = mode;
    }

    private void OnEnable()
    {
        // 读取上次选择的 Excel 文件路径和输出目录
        excelFilePath = EditorPrefs.GetString(PrefsExcelPathKey, excelFilePath);
        outputDir = EditorPrefs.GetString(PrefsOutputDirKey, outputDir);
    }

    private void OnGUI()
    {
        GUILayout.Label("1. 选择 Excel 文件:", EditorStyles.boldLabel);
        if (GUILayout.Button("浏览"))
        {
            string path = EditorUtility.OpenFilePanel("选择 Excel 文件", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                excelFilePath = path;
                EditorPrefs.SetString(PrefsExcelPathKey, excelFilePath);
            }
        }

        EditorGUILayout.LabelField("已选: ", string.IsNullOrEmpty(excelFilePath) ? "无" : excelFilePath);

        GUILayout.Space(5);
        GUILayout.Label("2. 选择输出目录:", EditorStyles.boldLabel);
        if (GUILayout.Button("选择输出目录"))
        {
            string path = EditorUtility.OpenFolderPanel("选择输出目录", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                outputDir = path;
                EditorPrefs.SetString(PrefsOutputDirKey, outputDir);
            }
        }

        EditorGUILayout.LabelField("已选: ", string.IsNullOrEmpty(outputDir) ? "无" : outputDir);

        GUILayout.Space(10);
        if (currentMode == Mode.Data)
        {
            GUILayout.Label("3. 数据格式选择:", EditorStyles.boldLabel);
            dataFormat = (DataFormat)EditorGUILayout.EnumPopup("格式", dataFormat);
            if (dataFormat == DataFormat.JSON)
            {
                prettyJson = EditorGUILayout.Toggle("格式化 JSON", prettyJson);
                if (prettyJson)
                    EditorGUILayout.HelpBox("开启后会给导出的 JSON 添加缩进和换行，方便阅读", MessageType.Info);
            }
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("导出", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(outputDir))
            {
                Debug.LogError("请先选择 Excel 文件和输出目录");
            }
            else
            {
                try
                {
                    if (currentMode == Mode.Classes)
                    {
                        GenerateClassesFromExcel(excelFilePath, outputDir);
                        Debug.Log("C# 类生成成功");
                    }
                    else
                    {
                        if (dataFormat == DataFormat.JSON)
                            SaveToJson(excelFilePath, outputDir, prettyJson);
                        else
                            SaveToBinary(excelFilePath, outputDir);
                        Debug.Log($"数据文件({dataFormat})导出成功");
                    }

                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"导出出错: {ex}");
                }
            }
        }
    }

    #region Excel 数据读取

    private Dictionary<string, List<Dictionary<string, object>>> ReadExcelData(string path)
    {
        var result = new Dictionary<string, List<Dictionary<string, object>>>();
        var fi = new FileInfo(path);
        using var pkg = new ExcelPackage(fi);
        foreach (var sheet in pkg.Workbook.Worksheets)
        {
            if (sheet.Dimension == null || sheet.Dimension.End.Row < 4) continue;

            int cols = sheet.Dimension.End.Column;
            var headers = new Dictionary<string, int>();
            var types = new Dictionary<int, string>();
            for (int c = 1; c <= cols; c++)
            {
                string h = sheet.Cells[2, c].Text.Trim();
                string t = sheet.Cells[3, c].Text.Trim().ToLower();
                if (string.IsNullOrEmpty(h) || headers.ContainsKey(h)) continue;
                headers[h] = c;
                types[c] = t;
            }

            var rows = new List<Dictionary<string, object>>();
            for (int r = 4; r <= sheet.Dimension.End.Row; r++)
            {
                var rowdict = new Dictionary<string, object>();
                bool empty = true;
                foreach (var kv in headers)
                {
                    var cell = sheet.Cells[r, kv.Value];
                    object v = ConvertCellValue(cell, types[kv.Value]);
                    rowdict[kv.Key] = v;
                    if (!string.IsNullOrEmpty(cell.Text.Trim()))
                        empty = false;
                }

                if (!empty) rows.Add(rowdict);
            }

            if (rows.Count > 0)
                result[sheet.Name] = rows;
        }

        return result;
    }

    private object ConvertCellValue(ExcelRange cell, string columnType)
    {
        string txt = cell.Text.Trim();
        if (string.IsNullOrEmpty(txt))
        {
            return columnType switch
            {
                "int" or "integer" => 0,
                "float" or "double" => 0m,
                "bool" or "boolean" => false,
                "datetime" => DateTime.MinValue,
                _ => string.Empty,
            };
        }

        return columnType switch
        {
            "int" or "integer" => int.TryParse(txt, out var i) ? i : 0,
            "float" or "double" => decimal.TryParse(txt, out var d) ? d : 0m,
            "bool" or "boolean" => txt == "1",
            "datetime" => DateTime.TryParse(txt, out var dt) ? dt : DateTime.MinValue,
            _ => txt,
        };
    }

    #endregion

    #region JSON 导出

    private void SaveToJson(string excelPath, string outDir, bool pretty)
    {
        var data = ReadExcelData(excelPath);
        string json;

        if (pretty)
        {
            var writer = new JsonWriter
            {
                PrettyPrint = true,
                IndentValue = 4
            };
            JsonMapper.ToJson(data, writer);
            json = writer.ToString();
        }
        else
        {
            json = JsonMapper.ToJson(data);
        }

        json = Regex.Unescape(json);

        Directory.CreateDirectory(outDir);

        string fp = Path.Combine(outDir, "GameData_Json.json");
        File.WriteAllText(fp, json, new UTF8Encoding(true));

        Debug.Log($"[Exporter] JSON 文件已写入: {fp}");
    }

    #endregion

    #region Protobuf 序列化 导出

    private void SaveToBinary(string excelPath, string outDir)
    {
        var raw = ReadExcelData(excelPath);
        var gd = new GameData();
        foreach (var sheetName in raw.Keys)
        {
            var listDict = raw[sheetName];
            var prop = typeof(GameData).GetProperty(sheetName);
            var field = typeof(GameData).GetField(sheetName);
            if (prop == null && field == null)
            {
                Debug.LogWarning($"未在 GameData 中找到 {sheetName} 对应的成员，已跳过");
                continue;
            }

            Type itemType = (prop != null
                ? prop.PropertyType.GetGenericArguments()[0]
                : field.FieldType.GetGenericArguments()[0]);
            IList targetList = (IList)(prop != null ? prop.GetValue(gd) : field.GetValue(gd));

            foreach (var row in listDict)
            {
                var inst = Activator.CreateInstance(itemType);
                foreach (var kv in row)
                {
                    var p = itemType.GetProperty(kv.Key);
                    if (p != null) p.SetValue(inst, kv.Value);
                    else itemType.GetField(kv.Key)?.SetValue(inst, kv.Value);
                }

                targetList.Add(inst);
            }
        }

        Directory.CreateDirectory(outDir);
        string bytesPath = Path.Combine(outDir, "GameData.bytes");
        using (var fs = File.Create(bytesPath))
            Serializer.Serialize(fs, gd);
        long size = new FileInfo(bytesPath).Length;
        Debug.Log($"[Exporter] Protobuf 二进制文件写入成功，文件大小 = {size} 字节 ({bytesPath})");
    }

    #endregion

    #region C# 类 生成 (含 Protobuf 注解 和 namespace)

    private void GenerateClassesFromExcel(string excelPath, string outDir)
    {
        var fi = new FileInfo(excelPath);
        using var pkg = new ExcelPackage(fi);

        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using ProtoBuf;");
        sb.AppendLine();

        var classNames = new List<string>();

        foreach (var sheet in pkg.Workbook.Worksheets)
        {
            if (sheet.Dimension == null || sheet.Dimension.End.Row < 3)
                continue;

            string cls = SanitizeClassName(sheet.Name);
            classNames.Add(cls);

            sb.AppendLine("[ProtoContract]");
            sb.AppendLine($"public class {cls}");
            sb.AppendLine("{");

            int cols = sheet.Dimension.End.Column;
            int memberIndex = 1;
            for (int c = 1; c <= cols; c++)
            {
                string colName = sheet.Cells[2, c].Text.Trim();
                string dataType = sheet.Cells[3, c].Text.Trim();
                if (string.IsNullOrEmpty(colName) || string.IsNullOrEmpty(dataType))
                    continue;

                string propName = SanitizePropertyName(colName);
                string csType = ConvertToCSharpType(dataType);

                sb.AppendLine($"    [ProtoMember({memberIndex})]");
                sb.AppendLine($"    public {csType} {propName} {{ get; set; }}");
                sb.AppendLine();

                memberIndex++;
            }

            sb.AppendLine("}");
            sb.AppendLine();
        }

        sb.AppendLine("[ProtoContract]");
        sb.AppendLine("public partial class GameData");
        sb.AppendLine("{");
        for (int i = 0; i < classNames.Count; i++)
        {
            string cls = classNames[i];
            sb.AppendLine($"    [ProtoMember({i + 1})]");
            sb.AppendLine($"    public List<{cls}> {cls} {{ get; set; }}  = new List<{cls}>();");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        string outFile = Path.Combine(outDir, "GameDataTable.cs");
        Directory.CreateDirectory(Path.GetDirectoryName(outFile));
        File.WriteAllText(outFile, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[Exporter] C# + Protobuf 文件写入: {outFile}");
    }

    #endregion

    private string SanitizeClassName(string name)
    {
        var s = Regex.Replace(name, "[^\\w]", "");
        if (string.IsNullOrEmpty(s) || char.IsDigit(s[0])) s = "_" + s;
        return s;
    }

    private string SanitizePropertyName(string name)
    {
        var s = Regex.Replace(name, "[^\\w]", "");
        if (string.IsNullOrEmpty(s) || char.IsDigit(s[0])) s = "_" + s;
        return s;
    }

    private string ConvertToCSharpType(string dataType)
    {
        return dataType.ToLower() switch
        {
            "int" or "integer" => "int",
            "float" or "double" => "decimal",
            "bool" or "boolean" => "bool",
            "datetime" => "DateTime",
            _ => "string",
        };
    }
}
