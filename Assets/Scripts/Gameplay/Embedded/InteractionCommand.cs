namespace Gameplay.Embedded
{
    /// <summary>
    /// 交互指令
    /// </summary>
    public enum InteractionCommand
    {
        Control = 0x00, // 控制信息
        Info = 0x10, // 提示信息
        Warning = 0x11, // 警告信息
        Error = 0x12, // 错误信息
        Uart = 0x20, // 串口通信
    }
}