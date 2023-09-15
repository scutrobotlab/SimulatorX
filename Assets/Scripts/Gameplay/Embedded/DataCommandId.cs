namespace Gameplay.Embedded
{
    /// <summary>
    /// 交互信息 内容ID
    /// </summary>
    public enum DataCommandId
    {
        OwnRobotCommunicate = 0x0200, // 己方机器人间通信
        DeleteCustomGraphic = 0x0100, // 删除自定义图形
        Draw1Graphic = 0x0101, // 绘制一个图形
        Draw2Graphics = 0x0102, // 绘制两个图形
        Draw5Graphics = 0x0103, // 绘制五个图形
        Draw7Graphics = 0x0104, // 绘制七个图形
        DrawCharGraphic = 0x0110 // 绘制字符图形
    }
}