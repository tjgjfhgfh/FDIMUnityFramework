using FDIM.Framework;

namespace FDIM.Framework
{
    //�ӿڿ�����
    public interface IManager
    {
        public void Init();
    }
    
    /// <summary>
    /// ͳһ��ڣ������õ���Ŀ�����еĹ�����
    /// </summary>
    public static class Managers
    {
        /// <summary> ��Ƶ������ </summary>
        public static AudioManager AudioManager => AudioManager.Instance;
    
        /// <summary> �¼����� </summary>
        public static EventCenterManager EventCenterManager => EventCenterManager.Instance;
    
        /// <summary> ����������� </summary>
        public static InputKeysManager InputKeysManager => InputKeysManager.Instance;
    
        /// <summary> �������ع��� </summary>
        public static LoadSceneManager LoadSceneManager => LoadSceneManager.Instance;
    
        /// <summary> ��־���Թ��� </summary>
        public static LogMessage LogMessage => LogMessage.Instance;
    
        /// <summary> MonoBehaviour ���ȹ����Update �ɷ�����Э�̿����� </summary>
        public static MonoManager MonoManager => MonoManager.Instance;
    
        /// <summary> ����ع��� </summary>
        public static ObjectPoolsManager ObjectPoolsManagerPools => ObjectPoolsManager.Instance;
    
        /// <summary> ��Դ���ع��� </summary>
        public static ResourcesManager ResourcesManager => ResourcesManager.Instance;
    
        /// <summary> ��ʱ������ </summary>
        public static TimerManager TimerManager => TimerManager.Instance;
    
        /// <summary> UI ������� </summary>
        public static UIManager UIManager => UIManager.Instance;
    
        /// <summary> ���ñ���� </summary>
        public static ConfigManager ConfigManager => ConfigManager.Instance;
    
        /// <summary> ���̨������� </summary>
        public static HttpRequestManager HttpRequestManager => HttpRequestManager.Instance;
    
        /// <summary> Addressables��Դ���� </summary>
        public static AddressablesManager AddressablesManager => AddressablesManager.Instance;
    }
}
