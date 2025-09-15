using FDIM.Framework;

namespace FDIM.Framework
{
    //�ӿڿ�����
    public interface IManager
    {
        public void Init();
    }
    
    /// <summary>
    /// 模块统一管理
    /// </summary>
    public static class Managers
    {
        /// <summary> 音频管理器 </summary>
        public static AudioManager AudioManager => AudioManager.Instance;
    
        /// <summary> 事件管理 </summary>
        public static EventCenterManager EventCenterManager => EventCenterManager.Instance;
    
        /// <summary> 输入管理 </summary>
        public static InputKeysManager InputKeysManager => InputKeysManager.Instance;
    
        /// <summary> 加载场景管理 </summary>
        public static LoadSceneManager LoadSceneManager => LoadSceneManager.Instance;
    
        /// <summary> 日志管理 </summary>
        public static LogMessage LogMessage => LogMessage.Instance;
    
        /// <summary> Mono管理 </summary>
        public static MonoManager MonoManager => MonoManager.Instance;
    
        /// <summary> 对象池管理 </summary>
        public static ObjectPoolsManager ObjectPoolsManagerPools => ObjectPoolsManager.Instance;

        /// <summary> Resources加载管理</summary>
        public static ResourcesManager ResourcesManager => ResourcesManager.Instance;
    
        /// <summary> 时间管理器 </summary>
        public static TimerManager TimerManager => TimerManager.Instance;
    
        /// <summary> UI 管理 </summary>
        public static UIManager UIManager => UIManager.Instance;
    
        /// <summary> 配置表管理 </summary>
        public static ConfigManager ConfigManager => ConfigManager.Instance;
    
        /// <summary> 后台请求管理 </summary>
        public static HttpRequestManager HttpRequestManager => HttpRequestManager.Instance;
    
        /// <summary> Addressables加载管理 </summary>
        public static AddressablesManager AddressablesManager => AddressablesManager.Instance;
    }
}
