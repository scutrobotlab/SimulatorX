using System;
using System.Text;

namespace Gameplay.Embedded
{
    /// <summary>
    /// 串口帧
    /// </summary>
    public struct UartFrame
    {
        public UartFrameHeader FrameHeader; // 帧头部
        public UartCommandId CommandId; // 指令
        public byte[] Data; // 数据
        public byte[] CRC16; // CRC16校验和
    }

    /// <summary>
    /// 帧头部
    /// </summary>
    public struct UartFrameHeader
    {
        public byte Sof; // 帧起始字符
        public short DataLength; // 数据长度
        public byte Seq; // 包序号
        public byte CRC8; // CRC8校验和
    }

    /// <summary>
    /// 串口数据处理器
    /// </summary>
    public class UartProcessor
    {
        private const byte StartOfFrame = 0xA5;

        private byte _sequence;

        /// <summary>
        /// 生成帧
        /// </summary>
        /// <param name="commandId">指令ID</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public byte[] GetFrame(UartCommandId commandId, byte[] data)
        {
            var buf = new byte[9 + data.Length];

            // 写入帧头部
            buf[0] = StartOfFrame;
            var dataLength = BitConverter.GetBytes((short)data.Length);
            buf[1] = dataLength[0];
            buf[2] = dataLength[1];
            buf[3] = _sequence;
            _sequence++;
            // 计算CRC8校验和
            buf[4] = CRCCheckSum.GetCRC8CheckSum(buf, 4);

            // 写入指令
            var commandIdBytes = BitConverter.GetBytes((short)commandId);
            buf[5] = commandIdBytes[0];
            buf[6] = commandIdBytes[1];

            // 写入数据
            Buffer.BlockCopy(data, 0, buf, 7, data.Length);

            // 计算CRC16校验和
            var crc16 = CRCCheckSum.GetCRC16CheckSum(buf, (uint)(buf.Length - 2));
            var crc16Bytes = BitConverter.GetBytes(crc16);
            buf[buf.Length - 2] = crc16Bytes[0];
            buf[buf.Length - 1] = crc16Bytes[1];

            return buf;
        }

        /// <summary>
        /// 解析帧
        /// </summary>
        /// <param name="bytes">字节流</param>
        /// <returns></returns>
        public UartFrame? ParseFrame(byte[] bytes)
        {
            // 读取帧头部
            var startOfFrame = bytes[0];
            if (startOfFrame != StartOfFrame) return null;
            var dataLength = BitConverter.ToInt16(bytes, 1);
            var sequence = bytes[3];
            var crc8 = bytes[4];

            // 检查CRC8校验和
            if (!CRCCheckSum.VerifyCRC8CheckSum(bytes, 5))
            {
                var msg = Encoding.UTF8.GetBytes($"校验CRC8失败，帧Seq={sequence}已被拒收。");
                EmbeddedSimulate.Instance().SendFrame(InteractionCommand.Error, msg);
                return null;
            }

            // 读取帧指令和数据
            var commandId = BitConverter.ToInt16(bytes, 5);
            var data = new byte[dataLength];
            Buffer.BlockCopy(bytes, 7, data, 0, dataLength);
            var crc16 = new byte[2];
            Buffer.BlockCopy(bytes, 7 + dataLength, crc16, 0, 2);

            // 检查CRC16校验和
            if (!CRCCheckSum.VerifyCRC16CheckSum(bytes, (uint)bytes.Length))
            {
                var msg = Encoding.UTF8.GetBytes($"校验CRC16失败，帧Seq={sequence}已被拒收。");
                EmbeddedSimulate.Instance().SendFrame(InteractionCommand.Error, msg);
                return null;
            }

            return new UartFrame
            {
                FrameHeader = new UartFrameHeader
                {
                    Sof = startOfFrame,
                    DataLength = dataLength,
                    Seq = sequence,
                    CRC8 = crc8
                },
                CommandId = (UartCommandId)commandId,
                Data = data,
                CRC16 = crc16
            };
        }
    }
}