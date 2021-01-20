using Autofac;
using BungeeCore.Common;
using BungeeCore.Common.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace MinecraftTunnel.Core
{
    public class ServerListen
    {
        public readonly ILogger Logger;                               // 日志
        public readonly IConfiguration Configuration;                 // 配置文件
        private readonly ILifetimeScope LifetimeScope;            // 服务
        private Socket ServerSocket;                                  // Socket
        public ServerListen(ILogger<ServerListen> Logger, IConfiguration Configuration, ILifetimeScope LifetimeScope)
        {
            this.Logger = Logger;
            this.Configuration = Configuration;
            this.LifetimeScope = LifetimeScope;
        }
        public void Listen()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse(Configuration["Server:IP"]);
            IPEndPoint serverIP = new IPEndPoint(iPAddress, Configuration.GetValue<int>("Server:Port"));
            ServerSocket.Bind(serverIP);
            ServerSocket.NoDelay = true;
            ServerSocket.Listen(100);
            StartAccept(null);
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                // 被主动调用,并设置回调方法
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // 因为被复用所以得主动清除Accpet套接字
                acceptEventArg.AcceptSocket = null;
            }
            // Accpet 成功
            bool willRaiseEvent = ServerSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs socketAsync)
        {
            ProcessAccept(socketAsync);
        }
        /// <summary>
        /// accept 异步回调
        /// </summary>
        /// <param name="acceptEventArg"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            using (ILifetimeScope lifetimeScope = LifetimeScope.BeginLifetimeScope())
            {
                ServerCore serverCore = lifetimeScope.Resolve<ServerCore>();
                PlayerToken playerToken = lifetimeScope.Resolve<PlayerToken>();
                serverCore.Accpet(e.AcceptSocket, playerToken);
            }
            // 接受后面的连接请求
            StartAccept(e);
        }
    }
}
