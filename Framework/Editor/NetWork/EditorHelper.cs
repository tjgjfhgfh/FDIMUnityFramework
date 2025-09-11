using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


public static class EditorHelper
{
    [MenuItem("Reacool/BuildMessageProto")]
    static void BuildMessageProto()
    {
        var path = EditorUtility.SaveFilePanel("Save proto file", "", "message", "proto");
        if (string.IsNullOrEmpty(path)) return;

        var proto = ProtoBuf.Serializer.GetProto<ExportMessages>();
        File.WriteAllText(path, proto);
    }

    [MenuItem("Reacool/PrintSupportedMessages")]
    static void PrintSupportedMessages()
    {
        var str = string.Join("\n", NetData.NetDatas.Select(netData => LitJson.JsonMapper.ToJson(new NetDataPackage { Id = netData.DataId, Data = netData })));
        Debug.Log(str);
    }

    [MenuItem("Reacool/CreateMessageFromProto")]
    static void CreateMessageFromProto()
    {
        var path = EditorUtility.OpenFilePanel("Open proto file", "", "proto");
        if (string.IsNullOrEmpty(path)) return;

        var lines = File.ReadAllLines(path);
        var isMessage = false;
        var isEnum = false;
        var isPackage = false;
        var package = "ReacoolNet";
        var comments = new List<string>();
        var sb = new StringBuilder();
        sb.AppendLine("// 通过proto文件自动生成，请勿修改。");
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("package"))
            {
                isPackage = true;
                package = Regex.Match(line, @"package\s+(\w+);").Groups[1].Value;
                sb.AppendLine($"namespace {package}")
                    .AppendLine("{");
            }
            else if (line.StartsWith("//"))
            {
                comments.Add(line);
                if (isPackage) sb.Append("    ");
                if (isMessage || isEnum) sb.Append("    ");
                sb.AppendLine(line);
            }
            else if (line.StartsWith("message"))
            {
                isMessage = true;
                var name = Regex.Match(line, @"message\s+(\w+)").Groups[1].Value;
                var messageId = comments.FirstOrDefault(comment => comment.Contains("消息ID-"));
                sb.AppendLine("    [ProtoBuf.ProtoContract]")
                    .AppendLine($"    public class {name}{(messageId != null ? " : NetData" : "")}")
                    .AppendLine("    {");
                if (messageId != null)
                    sb.AppendLine($"        {messageId}")
                        .AppendLine($"        public override byte DataId => {messageId.Substring(messageId.IndexOf('-') + 1)};")
                        .AppendLine();
            }
            else if (line.StartsWith("enum"))
            {
                isEnum = true;
                var name = Regex.Match(line, @"enum\s+(\w+)").Groups[1].Value;
                sb.AppendLine($"    public enum {name}")
                    .AppendLine("    {");
            }
            else if (line.StartsWith("}"))
            {
                isEnum = false;
                isMessage = false;
                sb.AppendLine("    }");
            }
            else if (string.IsNullOrEmpty(line))
            {
                comments.Clear();
                sb.AppendLine();
            }
            else if (line.StartsWith("{"))
            {

            }
            else if (isMessage)
            {
                var match = Regex.Match(line, @"(?:repeated\s+)?(\w+)\s+(\w+)\s*=\s*(\d+);");
                var isArray = line.StartsWith("repeated");
                var type = match.Groups[1].Value;
                var name = match.Groups[2].Value;
                var id = int.Parse(match.Groups[3].Value);
                if (type == "int32") type = "int";
                if (type == "int64") type = "long";
                sb.AppendLine($"        [ProtoBuf.ProtoMember({id})]");
                sb.AppendLine($"        public {type}{(isArray ? "[]" : "")} {name};");
            }
            else if (isEnum)
            {
                sb.AppendLine($"        {line.Replace(';', ',')}");
            }
        }
        sb.AppendLine("}");

        var code = sb.ToString();
        var savePath = EditorUtility.SaveFilePanelInProject("Save message code", "MessageTypes", "cs", "");
        if (string.IsNullOrEmpty(savePath)) return;
        File.WriteAllText(savePath, code);
    }
}
