using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;

namespace BungeeCore.Common.Sockets
{
    public class ServerCore : IDisposable
    {
        public delegate void ServerReceive(byte[] Packet);
        public event ServerReceive OnServerReceive;
        public delegate void ServerClose();
        public event ServerClose OnServerClose;

        private readonly ILogger ILogger;                               // 日志
        private readonly IConfiguration IConfiguration;                 // 配置文件
        private byte[] ReceiveBuffer;
        private Socket Socket;
        private SocketAsyncEventArgs ReceiveEventArgs;

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
            try
            {
                SocketAsyncEventArgs SendEventArgs = new SocketAsyncEventArgs();
                SendEventArgs.SetBuffer(buffer, offset, count);
                Socket.SendAsync(SendEventArgs);
            }
            catch (Exception ex)
            {
                ILogger.LogError(ex.Message);
            }
        }
        public void SendPacket(byte[] Packet)
        {
            try
            {
                SocketAsyncEventArgs SendEventArgs = new SocketAsyncEventArgs();
                SendEventArgs.SetBuffer(Packet);
                Socket.SendAsync(SendEventArgs);
            }
            catch (Exception ex)
            {
                ILogger.LogError(ex.Message);
            }
        }
        public void Accpet(Socket Socket)
        {
            try
            {
                this.Socket = Socket;
                bool willRaiseEvent = Socket.ReceiveAsync(ReceiveEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessReceive(ReceiveEventArgs);
                }
            }
            catch (Exception ex)
            {
                ILogger.LogError(ex.Message);
            }
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
                    OnServerReceive(packet);
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
        public void Stop()
        {
            ReceiveBuffer = null;
            ReceiveEventArgs = null;
            Socket.Shutdown(SocketShutdown.Both);
            OnServerClose?.Invoke();
            Socket.Close();
        }

        public void Dispose()
        {

        }
    }
}
