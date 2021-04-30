using System;
using System.Net;
using System.Net.Sockets;

namespace BungeeCore.Common.Sockets
{
    public class ClientCore
    {
        private readonly IPEndPoint iPEndPoint;
        private Socket Socket;                                         // Socket
        private SocketAsyncEventArgs ReceiveEventArgs;
        private byte[] ReceiveBuffer = new byte[2097151];
        #region 事件
        public delegate void TunnelReceive(byte[] Packet);
        public event TunnelReceive OnTunnelReceive;
        public delegate void TunnelClose();
        public event TunnelClose OnTunnelClose;
        public delegate void TunnelConnect();
        public event TunnelConnect OnTunnelConnect;

        #endregion

        public ClientCore(IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
        }
        public void SendPacket(byte[] Packet)
        {
            SocketAsyncEventArgs SendEventArgs = new SocketAsyncEventArgs();
            SendEventArgs.SetBuffer(Packet);
            Socket.SendAsync(SendEventArgs);
        }
        public void Start()
        {
            Socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };
            SocketAsyncEventArgs connSocketAsyncEventArgs = new SocketAsyncEventArgs
            {
                RemoteEndPoint = iPEndPoint
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
                {
                    ProcessReceive(ReceiveEventArgs);
                }
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
                OnTunnelConnect();
            }
        }
        public void Stop()
        {
            ReceiveBuffer = null;
            ReceiveEventArgs = null;
            OnTunnelClose?.Invoke();
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }
    }
}
