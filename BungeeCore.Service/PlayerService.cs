using Autofac;
using BungeeCore.Common.Helper.Protocol;
using BungeeCore.Common.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    public class PlayerService : IDisposable
    {
        public readonly ILogger Logger;
        private readonly InfoService infoService;

        public readonly ServerCore ServerCore;
        public readonly ClientCore ClientCore;

        private readonly HandlerServcie HandlerServcie;
        private readonly AnalysisService AnalysisService;

        private readonly TunnelServcie tunnelServcie;

        private readonly Dictionary<int, IEnumerator<bool>> keyValues = new Dictionary<int, IEnumerator<bool>>();

        private readonly ILifetimeScope LifetimeScope;


        public PlayerService(ILogger<PlayerService> Logger, InfoService infoService, ServerCore ServerCore, ClientCore ClientCore,
            HandlerServcie HandlerServcie, AnalysisService AnalysisService, TunnelServcie tunnelServcie,
            ILifetimeScope LifetimeScope)
        {
            this.Logger = Logger;
            this.infoService = infoService;
            this.ServerCore = ServerCore;
            this.ClientCore = ClientCore;
            this.HandlerServcie = HandlerServcie;
            this.AnalysisService = AnalysisService;
            this.tunnelServcie = tunnelServcie;
            this.LifetimeScope = LifetimeScope;

            ServerCore.OnServerClose += OnClose;
            ClientCore.OnTunnelClose += OnClose;
            ServerCore.OnServerReceive += ServerCore_OnServerReceive;
            ClientCore.OnTunnelReceive += ClientCore_OnTunnelReceive;
            ClientCore.OnTunnelConnect += ClientCore_OnTunnelConnect;
        }

        private void ClientCore_OnTunnelConnect()
        {
            tunnelServcie.Next();
        }

        private void ClientCore_OnTunnelReceive(byte[] Packet)
        {
            List<ProtocolHeand> protocolHeands = AnalysisService.AnalysisHeand(Packet);
            foreach (ProtocolHeand protocolHeand in protocolHeands)
            {
                Type type = HandlerServcie.IHandler(protocolHeand.PacketId, infoService.Rose);
                IService service = (IService)LifetimeScope.Resolve(type);
                service.Parameter = AnalysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);
            }
            ServerCore.SendPacket(Packet);
        }
        private void ServerCore_OnServerReceive(byte[] Packet)
        {
            bool flag = true;
            try
            {
                List<ProtocolHeand> protocolHeands = AnalysisService.AnalysisHeand(Packet);
                foreach (ProtocolHeand protocolHeand in protocolHeands)
                {
                    Type type = HandlerServcie.IHandler(protocolHeand.PacketId, infoService.Rose);
                    if (type != null)
                    {
                        IService service = (IService)LifetimeScope.Resolve(type);
                        service.Parameter = AnalysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);
                        if (keyValues.TryGetValue(protocolHeand.PacketId, out IEnumerator<bool> value))
                        {
                            if (value.MoveNext())
                            {
                                flag = value.Current;
                            }
                            else
                            {
                                keyValues.Remove(protocolHeand.PacketId);
                            }
                        }
                        else
                        {
                            IEnumerator<bool> enumerator = service.Prerouting().GetEnumerator();
                            if (enumerator.MoveNext())
                            {
                                flag = enumerator.Current;
                                keyValues.Add(protocolHeand.PacketId, enumerator);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            if (flag)
            {
                ClientCore.SendPacket(Packet);
            }
        }
        private void OnClose()
        {
            ServerCore.OnServerReceive -= ServerCore_OnServerReceive;
            ClientCore.OnTunnelReceive -= ClientCore_OnTunnelReceive;
            ServerCore.OnServerClose -= OnClose;
            ClientCore.OnTunnelClose -= OnClose;
            LifetimeScope.Dispose();
        }

        public void Dispose()
        {

        }
    }
}
