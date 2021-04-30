using Autofac;
using BungeeCore.Common.Helper.Protocol;
using BungeeCore.Common.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BungeeCore.Service
{
    /// <summary>
    /// 通道服务
    /// </summary>
    public class ChannelService
    {
        private ClientCore clientCore;

        public bool Online { get; set; }
        public Stack<Action> Actions = new Stack<Action>();
        public readonly Dictionary<int, IEnumerator<Task<bool>>> keyValues = new Dictionary<int, IEnumerator<Task<bool>>>();

        private readonly ILogger logger;
        private readonly ServerCore serverCore;
        private readonly AnalysisService analysisService;
        private readonly ILifetimeScope lifetimeScope;

        private delegate Type IHannd(int PacketId);
        private IHannd hannd = HandlerServcie.Guest;

        public ChannelService(ILogger<ChannelService> logger, ServerCore serverCore,
            AnalysisService analysisService, ILifetimeScope lifetimeScope)
        {
            this.logger = logger;
            this.serverCore = serverCore;
            this.analysisService = analysisService;
            this.lifetimeScope = lifetimeScope;

            serverCore.OnServerClose += PlayerDisconnect;
            serverCore.OnServerReceive += ServerCore_OnServerReceive;
        }
        /// <summary>
        /// 来自服务器的数据包
        /// </summary>
        /// <param name="Packet">数据包内容</param>
        /// <returns></returns>
        private async void ClientCore_OnTunnelReceive(byte[] Packet)
        {
            byte[] buffer = Packet;
            List<ProtocolHeand> protocolHeands = analysisService.AnalysisHeand(buffer);
            foreach (ProtocolHeand protocolHeand in protocolHeands)
            {
                Type type = hannd(protocolHeand.PacketId);
                if (type != null)
                {
                    IService service = (IService)lifetimeScope.Resolve(type);
                    service.Parameter = analysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);
                }
            }
            serverCore.SendPacket(Packet);
        }
        /// <summary>
        /// 来自玩家的数据包
        /// </summary>
        /// <param name="Packet"></param>
        private async void ServerCore_OnServerReceive(byte[] Packet)
        {
            bool flag = true;
            try
            {
                byte[] buffer = Packet;
                List<ProtocolHeand> protocolHeands = analysisService.AnalysisHeand(buffer);
                foreach (ProtocolHeand protocolHeand in protocolHeands)
                {
                    Type type = hannd(protocolHeand.PacketId);
                    if (type != null)
                    {
                        IService service = (IService)lifetimeScope.Resolve(type);
                        service.Parameter = analysisService.MapToEntities(service.PacketTypes, protocolHeand.PacketData);
                        if (keyValues.TryGetValue(protocolHeand.PacketId, out IEnumerator<Task<bool>> value))
                        {
                            if (value.MoveNext())
                            {
                                flag = await value.Current;
                            }
                            else
                            {
                                keyValues.Remove(protocolHeand.PacketId);
                            }
                        }
                        else
                        {
                            IEnumerator<Task<bool>> enumerator = service.Prerouting().GetEnumerator();
                            if (enumerator.MoveNext())
                            {
                                flag = await enumerator.Current;
                                keyValues.Add(protocolHeand.PacketId, enumerator);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            if (flag)
            {
                clientCore.SendPacket(Packet);
            }
        }
        internal void PlayerConnect()
        {
            ClientCore clientCore = lifetimeScope.Resolve<ClientCore>();
            clientCore.Start();
            clientCore.OnTunnelClose += PlayerDisconnect;
            clientCore.OnTunnelReceive += ClientCore_OnTunnelReceive;
        }
        internal void PlayerKick()
        {
            serverCore.Stop();
        }
        internal void PlayerDisconnect()
        {
            serverCore.OnServerReceive -= ServerCore_OnServerReceive;
            clientCore.OnTunnelReceive -= ClientCore_OnTunnelReceive;
            serverCore.OnServerClose -= PlayerDisconnect;
            clientCore.OnTunnelClose -= PlayerDisconnect;
            lifetimeScope.Dispose();
        }
    }
}
