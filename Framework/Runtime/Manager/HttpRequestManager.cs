using LitJson;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FDIM.Framework
{
    // 注意：本脚本使用了 LitJson 进行序列化与反序列化
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
    /// 通用的 HTTP 请求管理器，支持 GET / POST / PUT / DELETE。
    /// 特性：
    ///   - 可直接使用完整 URL（不依赖 BaseUrl）
    ///   - 使用 LitJson 进行序列化和反序列化
    /// </summary>
    public class HttpRequestManager : SingletonPatternBase<HttpRequestManager>
    {
        [Tooltip("请求超时时间，单位秒。设置为 0 表示不超时。")]
        public int TimeoutSeconds = 10;

        #region 公共请求方法

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> query = null)
        {
            url = AppendQuery(url, query);
            using var req = UnityWebRequest.Get(url);
            ConfigureRequest(req);
            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"[GET] {url} 失败：{req.error}");

            Debug.Log($"[HTTP Success] GET {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<T>(req.downloadHandler.text);
        }

        /// <summary>
        /// 发送 POST 请求.TRequest为请求类型，TResponse为返回类型
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
                throw new Exception($"[POST] {url} 失败：{req.error}");

            Debug.Log($"[HTTP Success] POST {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<TResponse>(req.downloadHandler.text);
        }

        /// <summary>
        /// 发送 PUT 请求
        /// </summary>
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
                throw new Exception($"[PUT] {url} 失败：{req.error}");

            Debug.Log($"[HTTP Success] PUT {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<TResponse>(req.downloadHandler.text);
        }

        /// <summary>
        /// 发送 DELETE 请求
        /// </summary>
        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> query = null)
        {
            url = AppendQuery(url, query);
            using var req = UnityWebRequest.Delete(url);
            ConfigureRequest(req);
            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"[DELETE] {url} 失败：{req.error}");

            Debug.Log($"[HTTP Success] DELETE {url}\nResponse: {req.downloadHandler.text}");
            return JsonMapper.ToObject<T>(req.downloadHandler.text);
        }

        #endregion

        #region 私有工具方法

        /// <summary>
        /// 配置请求头、超时和授权等。
        /// </summary>
        private void ConfigureRequest(UnityWebRequest req, bool includeJsonHeader = false)
        {
            if (includeJsonHeader)
                req.SetRequestHeader("Content-Type", "application/json");

            // 如果需要授权，可在此处添加 Authorization 头
            // var token = PlayerPrefs.GetString("AuthToken", "");
            // if (!string.IsNullOrEmpty(token))
            //     req.SetRequestHeader("Authorization", $"Bearer {token}");

            req.timeout = TimeoutSeconds;
        }

        /// <summary>
        /// 拼接 URL 查询参数
        /// </summary>
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
