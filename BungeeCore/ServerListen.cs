using Autofac;
using BungeeCore.Common.Sockets;
using BungeeCore.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BungeeCore
{
    public class ServerListen : IHostedService
    {
        public readonly ILogger Logger;                               // 日志
        public readonly IConfiguration Configuration;                 // 配置文件
        private readonly ILifetimeScope LifetimeScope;                // 服务
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
            ILifetimeScope lifetimeScope = LifetimeScope.BeginLifetimeScope();
            PlayerService playerToken = lifetimeScope.Resolve<PlayerService>();
            playerToken.ServerCore.Accpet(e.AcceptSocket);
            if (playerToken is IDisposable disposable)
            {
                lifetimeScope.Disposer.AddInstanceForDisposal(disposable);
            }
            // 接受后面的连接请求
            StartAccept(e);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Listen();
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            ServerSocket.Close();
            return Task.CompletedTask;
        }
    }
}
