using Autofac;
using BungeeCore.Common.Helper.Protocol;
using BungeeCore.Common.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BungeeCore.Service
{
    public class PlayerService : IDisposable
    {
        public readonly ILogger Logger;
        public readonly ServerCore ServerCore;
        public readonly ClientCore ClientCore;
        private readonly HandlerServcie HandlerServcie;
        private readonly AnalysisService AnalysisService;

        private ILifetimeScope LifetimeScope;

        public string PlayerName;                            // 玩家Name

        public DateTime ConnectDateTime;                     // 连接时间
        public DateTime EndTime;                             // 到期时间
        public PlayerService(ILogger<PlayerService> Logger, ServerCore ServerCore, ClientCore ClientCore, HandlerServcie HandlerServcie, AnalysisService AnalysisService, ILifetimeScope LifetimeScope)
        {
            this.Logger = Logger;
            this.ServerCore = ServerCore;
            this.ClientCore = ClientCore;
            this.HandlerServcie = HandlerServcie;
            this.AnalysisService = AnalysisService;
            this.LifetimeScope = LifetimeScope;

            ServerCore.OnClose += OnClose;
            ClientCore.OnClose += OnClose;
            ServerCore.OnServerReceive += ServerCore_OnServerReceive;
            ClientCore.OnTunnelReceive += ClientCore_OnTunnelReceive;
        }

        private void ClientCore_OnTunnelReceive(byte[] Packet)
        {

        }
        private void ServerCore_OnServerReceive(byte[] Packet)
        {
            bool flag = true;
            try
            {
                List<ProtocolHeand> protocolHeands = AnalysisService.AnalysisHeand(false, Packet);
                foreach (var protocolHeand in protocolHeands)
                {
                    Type type = HandlerServcie.IHandler(protocolHeand.PacketId);
                    if (type != null)
                    {
                        IService service = (IService)LifetimeScope.Resolve(type);
                        object obj = AnalysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);


                        flag = service.Handler(obj).GetEnumerator().MoveNext();


                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }


            if (flag)
            {

            }
        }
        public void StartTunnel()
        {
            ClientCore.Start();
        }
        private void OnClose()
        {
            Dispose();
        }
        public void Dispose()
        {
            ServerCore.OnClose -= OnClose;
            ClientCore.OnClose -= OnClose;
            ServerCore.OnServerReceive -= ServerCore_OnServerReceive;
            ClientCore.OnTunnelReceive -= ClientCore_OnTunnelReceive;
        }
        /*
public void Login(string ServerAddress, ushort ServerPort)
{
   MemoryStream memoryStream1 = new MemoryStream();
   Handshake handshake = new Handshake();
   memoryStream1.WriteInt(0);
   memoryStream1.WriteInt(Handshake.PacketId);
   memoryStream1.WriteInt(ProtocolVersion);
   memoryStream1.WriteString(ServerAddress, true);
   memoryStream1.WriteUShort(ServerPort);
   memoryStream1.WriteInt((int)NextState.login);
   byte[] buffer = new byte[memoryStream1.Position];
   int size1 = (int)memoryStream1.Position - 1;
   memoryStream1.Position = 0;
   memoryStream1.WriteInt(size1);
   Array.Copy(memoryStream1.GetBuffer(), 0, buffer, 0, buffer.Length);

   ClientCore.SendPacket(buffer);


   MemoryStream memoryStream = new MemoryStream();
   Login login = new Login();
   login.Name = PlayerName;
   memoryStream.WriteInt(0);
   memoryStream.WriteInt(login.PacketId);
   memoryStream.WriteString(PlayerName, true);
   byte[] buffer2 = new byte[memoryStream.Position];
   int size = (int)memoryStream.Position - 1;
   memoryStream.Position = 0;
   memoryStream.WriteInt(size);
   Array.Copy(memoryStream.GetBuffer(), 0, buffer2, 0, buffer2.Length);

   ClientCore.SendPacket(buffer2);
}
*/
    }
}
