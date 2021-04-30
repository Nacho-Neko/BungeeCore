using Autofac;
using BungeeCore.Common.Helper.Protocol;
using BungeeCore.Common.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    /// <summary>
    /// 通道服务
    /// </summary>
    public class ChannelService
    {
        public bool Online { get; set; }
        public Stack<Action> Actions = new Stack<Action>();
        public readonly Dictionary<int, IEnumerator<bool>> keyValues = new Dictionary<int, IEnumerator<bool>>();

        private readonly ILogger logger;
        public readonly ServerCore serverCore;
        public readonly ClientCore clientCore;
        private readonly AnalysisService analysisService;
        private readonly EncryptionService encryptionService;
        private readonly ILifetimeScope lifetimeScope;

        private delegate Type IHannd(int PacketId);
        private IHannd hannd = HandlerServcie.Guest;

        public ChannelService(ILogger<ChannelService> logger,
            ServerCore serverCore, ClientCore clientCore,
            AnalysisService analysisService, HandlerServcie handlerServcie, EncryptionService encryptionService,
            ILifetimeScope lifetimeScope)
        {
            this.logger = logger;
            this.serverCore = serverCore;
            this.clientCore = clientCore;
            this.analysisService = analysisService;
            this.encryptionService = encryptionService;
            this.lifetimeScope = lifetimeScope;

            serverCore.OnServerClose += OnClose;
            clientCore.OnTunnelClose += OnClose;
            serverCore.OnServerReceive += ServerCore_OnServerReceive;
            clientCore.OnTunnelReceive += ClientCore_OnTunnelReceive;
            clientCore.OnTunnelConnect += ClientCore_OnTunnelConnect;
        }
        public void SendPacket(object Entities)
        {
            byte[] Packet = (byte[])Entities;
            if (encryptionService.Enable)
                Packet = encryptionService.Encrypt(Packet);
            //先序列化 Entities
            //然后发出这个包
            serverCore.SendPacket(Packet);
        }
        public void SendPacket(byte[] Packet)
        {
            if (encryptionService.Enable)
                Packet = encryptionService.Encrypt(Packet);
            serverCore.SendPacket(Packet);
        }
        public void Connect()
        {
            encryptionService.Enable = false;
            clientCore.Start();
        }
        private void ClientCore_OnTunnelConnect()
        {
            /// 先判断类型
            /// 决定目标服务器是 代理模式 还是 隧道模式
            Action action = Actions.Pop();
            action();
        }
        private void ClientCore_OnTunnelReceive(byte[] Packet)
        {
            byte[] buffer = Packet;
            if (encryptionService.Enable)
                buffer = encryptionService.Decrypt(Packet);
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
        private void ServerCore_OnServerReceive(byte[] Packet)
        {
            bool flag = true;
            try
            {
                byte[] buffer = Packet;
                if (encryptionService.Enable)
                    buffer = encryptionService.Decrypt(Packet);
                List<ProtocolHeand> protocolHeands = analysisService.AnalysisHeand(buffer);
                foreach (ProtocolHeand protocolHeand in protocolHeands)
                {
                    Type type = hannd(protocolHeand.PacketId);
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
                logger.LogError(ex.Message);
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
    }
}
