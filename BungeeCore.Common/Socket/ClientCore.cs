using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace BungeeCore.Common.Sockets
{
    public class ClientCore  : IDisposable
    {
        private readonly ILogger Logger;                               // 日志
        private readonly IConfiguration Configuration;                 // 配置文件
        private Socket Socket;                                         // Socket
        private SocketAsyncEventArgs ReceiveEventArgs;
        private byte[] ReceiveBuffer = new byte[2097151];
        #region 事件
        public delegate void TunnelReceive(byte[] Packet);
        public event TunnelReceive OnTunnelReceive;
        public delegate void Close();
        public event Close OnClose;
        #endregion
        public ClientCore(ILogger<ClientCore> Logger, IConfiguration Configuration)
        {
            this.Logger = Logger;
            this.Configuration = Configuration;
        }
        public void SendPacket(byte[] Packet)
        {
            SocketAsyncEventArgs SendEventArgs = new SocketAsyncEventArgs();
            SendEventArgs.SetBuffer(Packet);
            Socket.SendAsync(SendEventArgs);
        }
        public void Start()
        {
            IPAddress ipaddr;
            if (!IPAddress.TryParse(Configuration["Nat:IP"], out ipaddr))
            {
                IPAddress[] iplist = Dns.GetHostAddresses(Configuration["Nat:IP"]);
                if (iplist != null && iplist.Length > 0)
                {
                    ipaddr = iplist[0];
                }
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipaddr, Configuration.GetValue<int>("Nat:Port"));
            Socket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            SocketAsyncEventArgs connSocketAsyncEventArgs = new SocketAsyncEventArgs
            {
                RemoteEndPoint = localEndPoint
            };
            connSocketAsyncEventArgs.Completed += IO_Completed;
            if (!Socket.ConnectAsync(connSocketAsyncEventArgs))
            {
                ProcessConnect(connSocketAsyncEventArgs);
            }
        }
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                default:
                    Logger.LogError("套接字上完成的最后一个操作不是接收或发送或连接。");
                    throw new ArgumentException("套接字上完成的最后一个操作不是接收或发送或连接。");
            }
        }
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (ReceiveEventArgs.BytesTransferred > 0 && ReceiveEventArgs.SocketError == SocketError.Success)
            {
                if (OnTunnelReceive != null)
                {
                    byte[] Buffer = new byte[ReceiveEventArgs.BytesTransferred];
                    Array.Copy(ReceiveEventArgs.Buffer, ReceiveEventArgs.Offset, Buffer, 0, ReceiveEventArgs.BytesTransferred);
                    OnTunnelReceive(Buffer);
                }
                bool willRaiseEvent = Socket.ReceiveAsync(ReceiveEventArgs);
                if (!willRaiseEvent)
                    ProcessReceive(ReceiveEventArgs);
            }
            else
            {
                Stop();
            }
        }
        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                ReceiveEventArgs = new SocketAsyncEventArgs();
                ReceiveEventArgs.SetBuffer(ReceiveBuffer, 0, ReceiveBuffer.Length);
                ReceiveEventArgs.Completed += IO_Completed;
                if (!Socket.ReceiveAsync(ReceiveEventArgs))
                {
                    ProcessReceive(ReceiveEventArgs);
                }
            }
        }
        private void Stop()
        {
            OnClose?.Invoke();
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            Socket.Dispose();
            Dispose();
        }
        public void Dispose()
        {
            ReceiveBuffer = null;
            ReceiveEventArgs = null;
        }
    }
}
