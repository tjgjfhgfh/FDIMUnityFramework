using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using LitJson; // 如果你还想用 LitJson 反序列化

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
/// 负责所有 HTTP 请求：GET/POST/PUT/DELETE
///   — 直接使用传入的 URL，无需 BaseUrl
///   — 序列化反序列化使用 LitJson
/// </summary>
public class HttpRequestManager : SingletonPatternBase<HttpRequestManager>
{
    [Tooltip("请求超时秒数，0 表示不超时。")] public int TimeoutSeconds = 10;

    #region 公共请求方法

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

    #region 私有工具

    private void ConfigureRequest(UnityWebRequest req, bool includeJsonHeader = false)
    {
        if (includeJsonHeader)
            req.SetRequestHeader("Content-Type", "application/json");

        // 如果需要鉴权，可以在这里加：
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