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
        public readonly ServerCore serverCore;
        public readonly ClientCore clientCore;
        private readonly ILogger Logger;
        private readonly InfoService infoService;
        private readonly HandlerServcie handlerServcie;
        private readonly AnalysisService analysisService;
        private readonly TunnelServcie tunnelServcie;
        private readonly Dictionary<int, IEnumerator<bool>> keyValues = new Dictionary<int, IEnumerator<bool>>();
        private readonly ILifetimeScope lifetimeScope;
        public PlayerService(ILogger<PlayerService> Logger, InfoService infoService, ServerCore serverCore, ClientCore clientCore,
            HandlerServcie handlerServcie, AnalysisService analysisService, TunnelServcie tunnelServcie,
            ILifetimeScope lifetimeScope)
        {
            this.Logger = Logger;
            this.infoService = infoService;
            this.serverCore = serverCore;
            this.clientCore = clientCore;
            this.handlerServcie = handlerServcie;
            this.analysisService = analysisService;
            this.tunnelServcie = tunnelServcie;
            this.lifetimeScope = lifetimeScope;

            serverCore.OnServerClose += OnClose;
            clientCore.OnTunnelClose += OnClose;
            serverCore.OnServerReceive += ServerCore_OnServerReceive;
            clientCore.OnTunnelReceive += ClientCore_OnTunnelReceive;
            clientCore.OnTunnelConnect += ClientCore_OnTunnelConnect;
        }
        private void ClientCore_OnTunnelConnect()
        {
            tunnelServcie.Next();
        }
        private void ClientCore_OnTunnelReceive(byte[] Packet)
        {
            List<ProtocolHeand> protocolHeands;
            if (infoService.Encryption)
            {
                // 解密数据
                // protocolHeands = analysisService.AnalysisHeand(Packet);
                protocolHeands = new List<ProtocolHeand>();
            }
            else
            {
                protocolHeands = analysisService.AnalysisHeand(Packet);
            }
            foreach (ProtocolHeand protocolHeand in protocolHeands)
            {
                Type type = handlerServcie.IHandler(protocolHeand.PacketId, infoService.Rose);
                if (type != null)
                {
                    IService service = (IService)lifetimeScope.Resolve(type);
                    service.Parameter = analysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);
                }
            }
            serverCore.SendPacket(Packet);
        }
        private void ServerCore_OnServerReceive(byte[] Packet)
        {
            bool flag = true;
            try
            {
                List<ProtocolHeand> protocolHeands;
                if (infoService.Encryption)
                {
                    // 解密数据
                    // protocolHeands = analysisService.AnalysisHeand(Packet);
                    protocolHeands = new List<ProtocolHeand>();
                }
                else
                {
                    protocolHeands = analysisService.AnalysisHeand(Packet);
                }
                foreach (ProtocolHeand protocolHeand in protocolHeands)
                {
                    Type type = handlerServcie.IHandler(protocolHeand.PacketId, infoService.Rose);
                    if (type != null)
                    {
                        IService service = (IService)lifetimeScope.Resolve(type);
                        service.Parameter = analysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);
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
                clientCore.SendPacket(Packet);
            }
        }
        private void OnClose()
        {
            serverCore.OnServerReceive -= ServerCore_OnServerReceive;
            clientCore.OnTunnelReceive -= ClientCore_OnTunnelReceive;
            serverCore.OnServerClose -= OnClose;
            clientCore.OnTunnelClose -= OnClose;
            lifetimeScope.Dispose();
        }
        public void Dispose()
        {

        }
    }
}
