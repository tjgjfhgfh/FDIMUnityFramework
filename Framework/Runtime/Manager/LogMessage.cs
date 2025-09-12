using System;
using System.IO;
using UnityEngine;


namespace FDIM.Framework
{
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
            // ע�� Unity ��־�ص�������������־��Ϣ
            Application.logMessageReceivedThreaded += ShowMessage;
        }
    
        public void OnDestroy()
        {
            Debug.Log("EndWork");
            Application.logMessageReceivedThreaded -= ShowMessage;
        }
    
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void Log<T>(T message, LogType type = LogType.Log)
        {
            if (!IsLogMessage) return;
    
            string msg = message?.ToString() ?? string.Empty;
            // ���������̨
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
    
            // д���ļ�
            ShowMessage(msg, string.Empty, type);
        }
    
    
        /// <summary>
        /// ��־�ص���������־�ַ�����д���ļ�
        /// </summary>
        /// <param name="condition">��־����</param>
        /// <param name="stackTrace">��ջ��Ϣ</param>
        /// <param name="type">��־����</param>
        private void ShowMessage(string condition, string stackTrace, LogType type)
        {
            string logRecord = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{type}\n{condition}\n{stackTrace}\n";
            WriteLogTxt(logRecord);
        }
    
        /// <summary>
        /// ��ʼ����־�ļ�������ǰ���������ļ�������ɾ��15��ǰ����־�ļ�
        /// </summary>
        private void InitLogTxt()
        {
            // ʹ�õ�ǰ����������־�ļ��������� "logTex_20250325.txt"
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            _logTxtPath = Path.Combine(Application.persistentDataPath, $"logTex_{dateStr}.txt");
    
            // ɨ��������־�ļ���ɾ��15��ǰ���ļ�
            string[] logFiles = Directory.GetFiles(Application.persistentDataPath, "logTex_*.txt");
            foreach (var file in logFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file); // ���� "logTex_20250310"
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
                            Debug.LogError($"ɾ����־�ļ� {file} ʧ�ܣ�{ex.Message}");
                        }
                    }
                }
            }
    
            // �����ǰ���ڵ���־�ļ������ڣ��򴴽��ļ�
            if (!File.Exists(_logTxtPath))
            {
                File.CreateText(_logTxtPath).Dispose();
            }
        }
    
        /// <summary>
        /// ��׷�ӷ�ʽд����־��Ϣ���ļ�
        /// </summary>
        /// <param name="log">��־�ַ���</param>
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
                Debug.LogError($"д����־ʧ�ܣ�{ex.Message}");
            }
        }
    }
}
