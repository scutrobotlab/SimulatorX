using System;

namespace Gameplay.Embedded
{
    /// <summary>
    /// 交互数据头部
    /// </summary>
    public struct InteractiveHeader
    {
        public const short OwnRobotCommStart = 0x0200; // 己方机器人间通信起始
        public const short OwnRobotCommEnd = 0x02FF; // 己方机器人间通信结束
        public const int HeaderLength = 6;

        public DataCommandId ContentId; // 数据段内容ID
        public ushort SenderID; // 发送者ID
        public ushort ReceiverID; // 接收者ID
    }

    /// <summary>
    /// 交互数据
    /// </summary>
    public abstract class InteractiveData
    {
        /// <summary>
        /// 获取指令ID
        /// </summary>
        /// <returns></returns>
        public abstract DataCommandId GetCommandId();

        /// <summary>
        /// 获取原始字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetOriginalBytes()
        {
            var bytes = new byte[InteractiveHeader.HeaderLength + Data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)Header.ContentId), 0, bytes, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Header.SenderID), 0, bytes, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Header.ReceiverID), 0, bytes, 4, 2);
            Buffer.BlockCopy(Data, 0, bytes, 6, Data.Length);
            return bytes;
        }

        public InteractiveData(byte[] bytes)
        {
            var contentId = BitConverter.ToUInt16(bytes, 0);
            if (contentId >= InteractiveHeader.OwnRobotCommStart && contentId <= InteractiveHeader.OwnRobotCommEnd)
                contentId = (ushort)DataCommandId.OwnRobotCommunicate;
            var senderId = BitConverter.ToUInt16(bytes, 2);
            var receiverId = BitConverter.ToUInt16(bytes, 4);

            Header = new InteractiveHeader
            {
                ContentId = (DataCommandId)contentId,
                SenderID = senderId,
                ReceiverID = receiverId
            };

            Data = new byte[bytes.Length - InteractiveHeader.HeaderLength];
            Buffer.BlockCopy(bytes, 6, Data, 0, Data.Length);
        }

        public InteractiveData(InteractiveHeader header, byte[] data)
        {
            Header = header;
            Data = data;
        }

        // 数据头部
        public InteractiveHeader Header { get; private set; }

        // 数据
        public byte[] Data { get; private set; }
    }
}