using Autofac;
using BungeeCore.Common.Event;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;

namespace BungeeCore.Common.Sockets
{
    public class ServerCore
    {
        public readonly ILogger ILogger;                               // 日志
        public readonly IConfiguration IConfiguration;                 // 配置文件

        private byte[] ReceiveBuffer;

        private ILifetimeScope LifetimeScope;
        private Socket Socket;
        private PlayerToken playerToken;
        private SocketAsyncEventArgs ReceiveEventArgs;

        #region 事件
        public static ServerReceive OnServerReceive;
        public static PlayerLeave OnClose;
        #endregion

        public ServerCore(ILogger<ServerCore> ILogger, IConfiguration IConfiguration)
        {
            this.ILogger = ILogger;
            this.IConfiguration = IConfiguration;

            ReceiveBuffer = new byte[2097151];

            ReceiveEventArgs = new SocketAsyncEventArgs();
            ReceiveEventArgs.SetBuffer(ReceiveBuffer, 0, ReceiveBuffer.Length);
            ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
        }

        public void SendPacket(byte[] buffer, int offset, int count)
        {
            SocketAsyncEventArgs SendEventArgs = new SocketAsyncEventArgs();
            SendEventArgs.SetBuffer(buffer, offset, count);
            Socket.SendAsync(SendEventArgs);
        }
        public void SendPacket(byte[] Packet)
        {
            SocketAsyncEventArgs SendEventArgs = new SocketAsyncEventArgs();
            SendEventArgs.SetBuffer(Packet);
            Socket.SendAsync(SendEventArgs);
        }
        public void Accpet(Socket Socket, PlayerToken playerToken)
        {
            this.Socket = Socket;
            this.playerToken = playerToken;
            bool willRaiseEvent = Socket.ReceiveAsync(ReceiveEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(ReceiveEventArgs);
            }
        }
        public void Start(ILifetimeScope LifetimeScope)
        {
            this.LifetimeScope = LifetimeScope;
        }
        /// <summary>
        /// 每当套接字上完成接收或发送操作时，都会调用此方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">与完成的接收操作关联的SocketAsyncEventArg</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
        /// <summary>
        /// 消息处理的回调
        /// </summary>
        /// <param name="e">操作对象</param>
        private void ProcessReceive(SocketAsyncEventArgs socketAsync)
        {
            int offset = ReceiveEventArgs.Offset;
            int count = ReceiveEventArgs.BytesTransferred;
            byte[] Buffer = ReceiveEventArgs.Buffer;
            if (count > 0 && ReceiveEventArgs.SocketError == SocketError.Success)
            {
                if (OnServerReceive != null)
                {
                    byte[] packet = new byte[count];
                    Array.Copy(Buffer, offset, packet, 0, count);
                    OnServerReceive(playerToken, packet);
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
        private void Stop()
        {
            OnClose.Invoke(playerToken);
            // ServiceScope.Dispose();
            // Socket.Shutdown(SocketShutdown.Send);
            Socket.Close();
            Socket.Dispose();
            // ServiceScope.Dispose();
        }
    }
}
