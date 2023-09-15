using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Misc
{
    /// <summary>
    /// 单连接策略
    /// </summary>
    public enum SingleConStrategy
    {
        /// <summary>
        /// 当接收新连接时保留现有的
        /// </summary>
        KeepExistingWhenAccept = 0,

        /// <summary>
        /// 当接收新连接时关闭现有的
        /// </summary>
        CloseExistingWhenAccept = 1,

        /// <summary>
        /// 当接收新连接时直接拒绝
        /// </summary>
        RefuseNewerWhenAccept = 2,
    }

    /// <summary>
    /// 单连接TCP
    /// 只保留一个Socket与客户端通信
    /// </summary>
    public class SingleConTCPServer
    {
        public delegate void ReceiveBytes(byte[] buf, int size);

        public event ReceiveBytes OnReceiveBytes;

        public TCPServer TcpServer { get; }

        private Socket _socket;
        private Thread _receiveThread;
        private readonly SingleConStrategy _strategy = SingleConStrategy.KeepExistingWhenAccept;

        public SingleConTCPServer(int port)
        {
            TcpServer = new TCPServer();
            TcpServer.StartAccept(port);
            TcpServer.OnAcceptSocket += OnAcceptSocket;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <param name="strategy">自定义Accept策略</param>
        public SingleConTCPServer(int port, SingleConStrategy strategy)
            : this(port)
        {
            _strategy = strategy;
        }

        /// <summary>
        /// 通过套接字发送数据
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <returns></returns>
        public int Send(byte[] buffer)
        {
            return _socket.Send(buffer);
        }

        /// <summary>
        /// 关闭套接字和服务端
        /// </summary>
        public void CloseServer()
        {
            Close();
            TcpServer.Close();
        }

        /// <summary>
        /// 关闭套接字
        /// </summary>
        public void Close()
        {
            _socket?.Close();
            _socket = null;
        }

        /// <summary>
        /// 套接字超时关闭时触发
        /// </summary>
        protected virtual void OnSocketTimeoutClosed()
        {
            Debug.LogWarning("远程主机连接超时，Socket已关闭。");
        }

        /// <summary>
        /// 套接字正常关闭时触发
        /// </summary>
        protected virtual void OnSocketForcedClosed()
        {
            Debug.Log("Socket强制关闭。");
        }

        /// <summary>
        /// 接收套接字时回调
        /// </summary>
        /// <param name="socket"></param>
        private void OnAcceptSocket(Socket socket)
        {
            if (_socket == null || !_receiveThread.IsAlive)
            {
                // 目前没有正常的连接
                _socket = socket;
                _receiveThread = new Thread(ReceiveListener);
                _receiveThread.Start();
                return;
            }

            switch (_strategy)
            {
                case SingleConStrategy.KeepExistingWhenAccept:
                    break;
                case SingleConStrategy.CloseExistingWhenAccept:
                    _socket = socket;
                    _receiveThread?.Abort(); // 终止旧线程
                    _receiveThread = new Thread(ReceiveListener);
                    _receiveThread.Start();
                    break;
                case SingleConStrategy.RefuseNewerWhenAccept:
                    socket.Close(); // 直接关闭新套接字
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 接收监听线程
        /// </summary>
        private void ReceiveListener()
        {
            var receiveEmptyCount = 0;
            while (!TcpServer.Closed && _socket is { Connected: true })
            {
                var buf = new byte[1024];
                try
                {
                    var size = _socket.Receive(buf);
                    if (size != 0)
                    {
                        receiveEmptyCount = 0;
                        OnReceiveBytes?.Invoke(buf, size);
                    }
                    else
                    {
                        receiveEmptyCount++;
                        if (receiveEmptyCount >= 100)
                        {
                            OnSocketTimeoutClosed();
                            Close();
                        }
                        else Thread.Sleep(100);
                    }
                }
                catch (SocketException)
                {
                    OnSocketForcedClosed();
                }
            }
        }
    }
}