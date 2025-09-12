using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using LitJson; 
namespace FDIM.Framework
{
    // ����㻹���� LitJson �����л�
    
    public static class UnityAsyncExtensions
    {
        public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation op)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            op.completed += _ => tcs.SetResult(op.webRequest);
            return tcs.Task.GetAwaiter();
        }
    }
    
    /// <summary>
    /// �������� HTTP ����GET/POST/PUT/DELETE
    ///   �� ֱ��ʹ�ô���� URL������ BaseUrl
    ///   �� ���л������л�ʹ�� LitJson
    /// </summary>
    public class HttpRequestManager : SingletonPatternBase<HttpRequestManager>
    {
        [Tooltip("����ʱ������0 ��ʾ����ʱ��")] public int TimeoutSeconds = 10;
    
        #region �������󷽷�
    
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> query = null)
        {
            url = AppendQuery(url, query);
            using var req = UnityWebRequest.Get(url);
            ConfigureRequest(req);
            await req.SendWebRequest();
    
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"[GET] {url} ʧ�ܣ�{req.error}");
    
            Debug.Log($"[HTTP Success] GET {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<T>(req.downloadHandler.text);
        }
    
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest body)
        {
            string json = JsonMapper.ToJson(body);
            using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            ConfigureRequest(req, includeJsonHeader: true);
            await req.SendWebRequest();
    
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"[POST] {url} ʧ�ܣ�{req.error}");
    
            Debug.Log($"[HTTP Success] POST {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<TResponse>(req.downloadHandler.text);
        }
    
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest body)
        {
            string json = JsonMapper.ToJson(body);
            using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            ConfigureRequest(req, includeJsonHeader: true);
            await req.SendWebRequest();
    
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"[PUT] {url} ʧ�ܣ�{req.error}");
    
            Debug.Log($"[HTTP Success] PUT {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<TResponse>(req.downloadHandler.text);
        }
    
        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> query = null)
        {
            url = AppendQuery(url, query);
            using var req = UnityWebRequest.Delete(url);
            ConfigureRequest(req);
            await req.SendWebRequest();
    
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"[DELETE] {url} ʧ�ܣ�{req.error}");
    
            Debug.Log($"[HTTP Success] DELETE {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<T>(req.downloadHandler.text);
        }
    
        #endregion
    
        #region ˽�й���
    
        private void ConfigureRequest(UnityWebRequest req, bool includeJsonHeader = false)
        {
            if (includeJsonHeader)
                req.SetRequestHeader("Content-Type", "application/json");
    
            // �����Ҫ��Ȩ������������ӣ�
            // var token = PlayerPrefs.GetString("AuthToken", "");
            // if (!string.IsNullOrEmpty(token))
            //     req.SetRequestHeader("Authorization", $"Bearer {token}");
    
            req.timeout = TimeoutSeconds;
        }
    
        private string AppendQuery(string url, Dictionary<string, string> query)
        {
            if (query == null || query.Count == 0)
                return url;
    
            var sb = new StringBuilder(url);
            sb.Append(url.Contains("?") ? '&' : '?');
            bool first = true;
            foreach (var kv in query)
            {
                if (!first) sb.Append('&');
                first = false;
                sb.Append(UnityWebRequest.EscapeURL(kv.Key))
                    .Append('=')
                    .Append(UnityWebRequest.EscapeURL(kv.Value));
            }
    
            return sb.ToString();
        }
    
        #endregion
    }
}
