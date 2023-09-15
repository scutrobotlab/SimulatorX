using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Gameplay.Embedded.CustomizeUI;
using Misc;
using UnityEngine;

namespace Gameplay.Embedded
{
    /// <summary>
    /// 电控仿真
    /// </summary>
    public class EmbeddedSimulate
    {
        // 默认电控仿真TCP端口号
        private const int DefaultEmbeddedTcpPort = 5300;
        public readonly int EmbeddedTcpPort;

        private readonly EmbeddedSingleCon _singleConTcp;
        private readonly UartProcessor _uartProcessor = new UartProcessor();
        private readonly CustomizeUIProcessor _uiProcessor = new CustomizeUIProcessor();

        private static EmbeddedSimulate _instance;

        public static EmbeddedSimulate Instance() => _instance ??= new EmbeddedSimulate();

        /// <summary>
        /// 发送帧
        /// </summary>
        /// <param name="command">指令代码</param>
        /// <param name="data">数据</param>
        /// <param name="attribute">附加属性</param>
        /// <returns></returns>
        public int SendFrame(InteractionCommand command, byte[] data, byte attribute = 0x00)
        {
            var buffer = new byte[4 + data.Length];
            // 生成指令
            buffer[0] = (byte)command;
            // 生成数据长度
            var dataLength = (short)data.Length;
            var dataLengthBytes = BitConverter.GetBytes(dataLength);
            buffer[1] = dataLengthBytes[0];
            buffer[2] = dataLengthBytes[1];
            // 生成属性
            buffer[3] = attribute;
            // 写入数据
            Buffer.BlockCopy(data, 0, buffer, 4, dataLength);
            // 发送数据
            return _singleConTcp.Send(buffer);
        }

        /// <summary>
        /// 关闭TCPServer
        /// </summary>
        public void Close()
        {
            _singleConTcp.CloseServer();
        }

        private EmbeddedSimulate()
        {
            for (EmbeddedTcpPort = DefaultEmbeddedTcpPort;
                 EmbeddedTcpPort < DefaultEmbeddedTcpPort + 10;
                 EmbeddedTcpPort++)
            {
                try
                {
                    _singleConTcp = new EmbeddedSingleCon(EmbeddedTcpPort, SingleConStrategy.CloseExistingWhenAccept);
                    break;
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                    {
                        // 地址占用（通常是端口占用）
                    }
                    else throw;
                }
            }

            if (_singleConTcp != null) _singleConTcp.OnReceiveBytes += OnReceiveFrame;
        }

        /// <summary>
        /// 解析控制信号
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="attribute"></param>
        /// <param name="data"></param>
        private void ParseControl(int dataLength, byte attribute, byte[] data)
        {
        }

        /// <summary>
        /// 解析串口通信
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="attribute"></param>
        /// <param name="data"></param>
        private void ParseUart(int dataLength, byte attribute, byte[] data)
        {
            var uartFrame = _uartProcessor.ParseFrame(data);
            if (uartFrame == null) return;
            var frame = uartFrame.Value;
            switch (frame.CommandId)
            {
                case UartCommandId.RobotInteraction:
                    var contentId = BitConverter.ToUInt16(frame.Data, 0);
                    if (contentId >= 0x0100 && contentId <= 0x01FF) _uiProcessor.Parse(frame);
                    else if (contentId == (int)DataCommandId.OwnRobotCommunicate)
                        throw new NotImplementedException(); // TODO 2022年8月22日
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 接收到信息时调用
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        private void OnReceiveFrame(byte[] buf, int size)
        {
            var command = (InteractionCommand)buf[0];
            var dataLength = BitConverter.ToInt16(buf, 1);
            var attribute = buf[3];
            var data = buf.Skip(4).Take(dataLength).ToArray();
            switch (command)
            {
                case InteractionCommand.Control:
                    ParseControl(dataLength, attribute, data);
                    break;
                case InteractionCommand.Info:
                    Debug.Log(Encoding.Default.GetString(data));
                    break;
                case InteractionCommand.Warning:
                    Debug.LogWarning(Encoding.Default.GetString(data));
                    break;
                case InteractionCommand.Error:
                    Debug.LogError(Encoding.Default.GetString(data));
                    break;
                case InteractionCommand.Uart:
                    ParseUart(dataLength, attribute, data);
                    break;
                default:
                    var errMsg = $"不支持帧头部指令Command:0x{Convert.ToString(buf[0], 16)}";
                    SendFrame(InteractionCommand.Error, Encoding.UTF8.GetBytes(errMsg));
                    break;
            }
        }
    }
}