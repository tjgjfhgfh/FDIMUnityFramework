using System;
using System.IO;
using UnityEngine;


public class LogMessage : SingletonPatternBase<LogMessage>
{
    public bool IsLogMessage;
    private string _logTxtPath;

    public void Start()
    {
        InitLogTxt();
        StartLog();
        EventCenterManager.Instance.AddListener("EndWork", OnDestroy);
    }

    void StartLog()
    {
        // 注册 Unity 日志回调，捕获所有日志信息
        Application.logMessageReceivedThreaded += ShowMessage;
    }

    public void OnDestroy()
    {
        Debug.Log("EndWork");
        Application.logMessageReceivedThreaded -= ShowMessage;
    }

    /// <summary>
    /// 调试信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="type"></param>
    public void Log<T>(T message, LogType type = LogType.Log)
    {
        if (!IsLogMessage) return;

        string msg = message?.ToString() ?? string.Empty;
        // 输出到控制台
        switch (type)
        {
            case LogType.Warning:
                Debug.LogWarning(msg);
                break;
            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                Debug.LogError(msg);
                break;
            default:
                Debug.Log(msg);
                break;
        }

        // 写入文件
        ShowMessage(msg, string.Empty, type);
    }


    /// <summary>
    /// 日志回调：生成日志字符串并写入文件
    /// </summary>
    /// <param name="condition">日志内容</param>
    /// <param name="stackTrace">堆栈信息</param>
    /// <param name="type">日志类型</param>
    private void ShowMessage(string condition, string stackTrace, LogType type)
    {
        string logRecord = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{type}\n{condition}\n{stackTrace}\n";
        WriteLogTxt(logRecord);
    }

    /// <summary>
    /// 初始化日志文件：按当前日期生成文件名，并删除15天前的日志文件
    /// </summary>
    private void InitLogTxt()
    {
        // 使用当前日期生成日志文件名，例如 "logTex_20250325.txt"
        string dateStr = DateTime.Now.ToString("yyyyMMdd");
        _logTxtPath = Path.Combine(Application.persistentDataPath, $"logTex_{dateStr}.txt");

        // 扫描所有日志文件，删除15天前的文件
        string[] logFiles = Directory.GetFiles(Application.persistentDataPath, "logTex_*.txt");
        foreach (var file in logFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file); // 例如 "logTex_20250310"
            string datePart = fileName.Replace("logTex_", "");
            if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                    out DateTime logDate))
            {
                if ((DateTime.Now - logDate).TotalDays > 15)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"删除日志文件 {file} 失败：{ex.Message}");
                    }
                }
            }
        }

        // 如果当前日期的日志文件不存在，则创建文件
        if (!File.Exists(_logTxtPath))
        {
            File.CreateText(_logTxtPath).Dispose();
        }
    }

    /// <summary>
    /// 以追加方式写入日志信息到文件
    /// </summary>
    /// <param name="log">日志字符串</param>
    private void WriteLogTxt(string log)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(_logTxtPath))
            {
                sw.WriteLine(log);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"写入日志失败：{ex.Message}");
        }
    }
}