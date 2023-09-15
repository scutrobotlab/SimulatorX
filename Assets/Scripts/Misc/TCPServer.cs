using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Misc
{
    /// <summary>
    /// TCP 服务端
    /// </summary>
    public class TCPServer
    {
        /// <summary>
        /// 接受套接字
        /// </summary>
        public delegate void AcceptSocket(Socket socket);

        /// <summary>
        /// 接受套接字时发生事件
        /// </summary>
        public event AcceptSocket OnAcceptSocket;

        /// <summary>
        /// 是否套接字关闭
        /// </summary>
        public bool Closed { get; private set; }

        private TcpListener _tcpListener;
        private readonly Thread _listener;
        private readonly List<Socket> _connections = new List<Socket>();

        public TCPServer()
        {
            _listener = new Thread(AcceptListener)
            {
                Name = "TCPAcceptListener",
                IsBackground = true
            };
        }

        /// <summary>
        /// 启动同意连接监听
        /// </summary>
        /// <param name="port">监听端口号</param>
        public void StartAccept(int port)
        {
            Closed = false;
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _listener.Start();
            Debug.Log($"TCP服务器监听 0.0.0.0:{port}");
        }

        /// <summary>
        /// Accept 监听器
        /// </summary>
        private void AcceptListener()
        {
            while (!Closed)
            {
                try
                {
                    var socket = _tcpListener.AcceptSocket();
                    _connections.Add(socket);
                    OnAcceptSocket?.Invoke(socket);
                }
                catch (SocketException)
                {
                    Debug.Log("TCP服务器被正常关闭。");
                }
            }
        }

        /// <summary>
        /// 关闭TCP服务器
        /// </summary>
        public void Close()
        {
            Closed = true;
            // 关闭所有套接字
            foreach (var socket in _connections)
            {
                try
                {
                    socket.Close(10);
                }
                catch (SocketException)
                {
                }
            }

            _tcpListener.Stop();
        }
    }
}