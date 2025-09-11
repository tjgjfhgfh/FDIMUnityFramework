/// <summary>
/// 按键输入的命令
/// </summary>
public enum E_InputCommand
{
    None = 0,

    /// <summary>
    /// 按下了某个键
    /// </summary>
    GetKeyDown,

    /// <summary>
    /// 松开了某个键
    /// </summary>
    GetKeyUp,

    /// <summary>
    /// 按住了某个键
    /// </summary>
    GetKey,

    /// <summary>
    /// 按下了鼠标的某个键
    /// </summary>
    GetMouseButtonDown,

    /// <summary>
    /// 松开了鼠标的某个键
    /// </summary>
    GetMouseButtonUp,

    /// <summary>
    /// 按住了鼠标的某个键
    /// </summary>
    GetMouseButton,
}