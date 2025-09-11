using UnityEngine;

public class DemoHost : SingletonPatternMonoAutoBase_DontDestroyOnLoad<DemoHost>
{
    public int Port = 51365;
    public string Service = "Default";
    public bool LogData = true;
    public bool BinaryData = false;
    public DemoStringEvent OnMessageEvent;

    private PeerHost<DemoHost> mHost;

    private float mHeatTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        StartServer();
    }

    // Update is called once per frame
    void Update()
    {
        mHost?.Update();
        //if (mHeatTime > 0f)
        //{
        //    mHeatTime -= Time.deltaTime;
        //    if (mHeatTime <= 0f)
        //    {
        //        HreatDead();
        //    }
        //}
    }

    public void StartServer()
    {
        if (mHost != null) return;
        mHost = new PeerHost<DemoHost>(this, Port, Service);
        mHost.LogData = LogData;
        mHost.Start();
    }

    /// <summary>
    /// 直接泛型，只要继承NetData的类都可以走这里发送
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <exception cref="System.Exception"></exception>
    public void SendNetData<T>(T data) where T : NetData
    {
        if (mHost == null)
            throw new System.Exception("未连接");
        mHost.SendData(data, binary: BinaryData);
    }


    public void SetBinaryData(bool binary)
    {
        Debug.Log($"切换消息模式：binary={binary}");
        BinaryData = binary;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="demoMessage"></param>
    public void OnDemoMessage(Session session, DemoMessage demoMessage)
    {
        mHost.SendData(new DemoReply { Reply = $"Reply:{demoMessage.Message}" }, session, binary: BinaryData);

        OnMessageEvent?.Invoke(demoMessage.Message);
    }






    /// <summary>
    /// 收到心跳消息 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="heartBeatData"></param>
    public void OnTrackerDataPacket(Session session, HeartBeatData heartBeatData)
    {
        mHeatTime = 10;
        Debug.Log("收到心跳消息");
        Managers.TimerManager.CreateTimer(() => { SendNetData(new HeartBeatData { Status = true }); }, 3);
    }

    /// <summary>
    /// 接收手部Tracker信息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="demoMessage"></param>

    public void OnTrackerDataPacketProto(Session session, TrackerDataPacketProto trackerDataPacketProto)
    {
        Debug.Log("收到手部数据");
        Managers.EventCenterManager.Dispatch<TrackerDataPacketProto>("ReceiveTrackerDataPacketProto", trackerDataPacketProto);

    }

    /// <summary>
    /// 收到基本数据
    /// </summary>
    /// <param name="session"></param>
    /// <param name="heartBeatData"></param>
    public void OnPacketMetadata(Session session, PacketMetadata packetMetadata)
    {
        //Debug.Log("收到基本数据");
    }


    /// <summary>
    /// 接收手部Tracker信息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="demoMessage"></param>

    public void OnHandTracker(Session session, HandTracker handTracker)
    {

        Managers.EventCenterManager.Dispatch<HandTracker>("ReceiveHandTracker", handTracker);
    }
    /// <summary>
    /// 接收探头数据信息
    /// </summary>
    /// <param name="session">会话对象</param>
    /// <param name="probeData">探头数据包</param>
    public void OnProbeData(Session session, ProbeData probeData)
    {
        Managers.EventCenterManager.Dispatch<ProbeData>("ReceiveProbeData", probeData);
    }

    /// <summary>
    /// 接收校准数据信息
    /// </summary>
    /// <param name="session">会话对象</param>
    /// <param name="calibrationData">校准数据包</param>
    public void OnCalibrationData(Session session, CalibrationData calibrationData)
    {
        Managers.EventCenterManager.Dispatch<CalibrationData>("ReceiveCalibrationData", calibrationData);
    }

    /// <summary>
    /// 心跳超时，已经断开连接,1.需要清空连接，2.通知UI
    /// </summary>
    public void HreatDead()
    {

        if (mHost != null)
        {
            mHost.Destroy();
            mHost = null;
        }
    }
}