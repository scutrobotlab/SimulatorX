using System;

namespace Gameplay.Embedded.CustomizeUI
{
    /// <summary>
    /// 自定义UI处理器
    /// </summary>
    public class CustomizeUIProcessor
    {
        /// <summary>
        /// 以自定义UI的模式解析帧
        /// </summary>
        /// <param name="frame">帧</param>
        /// <exception cref="ArgumentException">遇到己方机器人通信</exception>
        /// <exception cref="ArgumentOutOfRangeException">枚举越界</exception>
        public void Parse(UartFrame frame)
        {
            var contentId = BitConverter.ToUInt16(frame.Data, 0);
            switch ((DataCommandId)contentId)
            {
                case DataCommandId.OwnRobotCommunicate:
                    throw new ArgumentException("己方机器人的通信不应该交付给UI处理器。");
                case DataCommandId.DeleteCustomGraphic:
                    var deleteCustomGraphicData = new DeleteCustomGraphicData(frame.Data);
                    break;
                case DataCommandId.Draw1Graphic:
                    var custom1GraphicData = new CustomGraphicData(CustomGraphicsCount.Draw1Graphic, frame.Data);
                    break;
                case DataCommandId.Draw2Graphics:
                    var custom2GraphicsData = new CustomGraphicData(CustomGraphicsCount.Draw2Graphics, frame.Data);
                    break;
                case DataCommandId.Draw5Graphics:
                    var custom5GraphicsData = new CustomGraphicData(CustomGraphicsCount.Draw5Graphics, frame.Data);
                    break;
                case DataCommandId.Draw7Graphics:
                    var custom7GraphicsData = new CustomGraphicData(CustomGraphicsCount.Draw7Graphics, frame.Data);
                    break;
                case DataCommandId.DrawCharGraphic:
                    var customCharacterData = new CustomCharacterData(frame.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}