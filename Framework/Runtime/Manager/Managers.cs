using XH;

//接口可扩充
public interface IManager
{
    public void Init();
}

/// <summary>
/// 统一入口：快速拿到项目里所有的管理器
/// </summary>
public static class Managers
{
    /// <summary> 音频管理器 </summary>
    public static AudioManager AudioManager => AudioManager.Instance;

    /// <summary> 事件中心 </summary>
    public static EventCenterManager EventCenterManager => EventCenterManager.Instance;

    /// <summary> 按键输入管理 </summary>
    public static InputKeysManager InputKeysManager => InputKeysManager.Instance;

    /// <summary> 场景加载管理 </summary>
    public static LoadSceneManager LoadSceneManager => LoadSceneManager.Instance;

    /// <summary> 日志调试管理 </summary>
    public static LogMessage LogMessage => LogMessage.Instance;

    /// <summary> MonoBehaviour 调度管理（Update 派发）及协程控制器 </summary>
    public static MonoManager MonoManager => MonoManager.Instance;

    /// <summary> 对象池管理 </summary>
    public static ObjectPoolsManager ObjectPoolsManagerPools => ObjectPoolsManager.Instance;

    /// <summary> 资源加载管理 </summary>
    public static ResourcesManager ResourcesManager => ResourcesManager.Instance;

    /// <summary> 计时器管理 </summary>
    public static TimerManager TimerManager => TimerManager.Instance;

    /// <summary> UI 界面管理 </summary>
    public static UIManager UIManager => UIManager.Instance;

    /// <summary> 配置表管理 </summary>
    public static ConfigManager ConfigManager => ConfigManager.Instance;

    /// <summary> 与后台请求管理 </summary>
    public static HttpRequestManager HttpRequestManager => HttpRequestManager.Instance;

    /// <summary> Addressables资源管理 </summary>
    public static AddressablesManager AddressablesManager => AddressablesManager.Instance;
}