using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions.Conver;
using BungeeCore.Common.Helper;
using BungeeCore.Common.Sockets;
using BungeeCore.Model.ClientBound;
using BungeeCore.Model.ServerBound;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static BungeeCore.Model.ClientBound.Response;

namespace BungeeCore.Service
{
    [PacketHandler(PakcetId = 0, Rose = Rose.Anonymouse)]
    public class LoginService : IService
    {
        private readonly ILogger Logger;
        private readonly InfoService infoService;
        private readonly ServerCore ServerCore;
        private readonly ClientCore ClientCore;
        private readonly TunnelServcie TunnelServcie;
        private readonly HandlerServcie HandlerServcie;

        private Handshake handshake;
        private Login login;
        private bool IsForge;
        public Type PacketTypes { get; private set; } = typeof(Handshake);
        public object Parameter { set; private get; }
        public LoginService(ILogger<LoginService> Logger, InfoService infoService, ServerCore ServerCore, ClientCore ClientCore, TunnelServcie TunnelServcie, HandlerServcie HandlerServcie)
        {
            this.Logger = Logger;
            this.infoService = infoService;
            this.ServerCore = ServerCore;
            this.ClientCore = ClientCore;
            this.TunnelServcie = TunnelServcie;
            this.HandlerServcie = HandlerServcie;
        }
        public IEnumerable<bool> Handler()
        {
            handshake = (Handshake)Parameter;
            if (handshake.NextState() == NextState.Status)
            {
                yield return false;
                Response response = new Response("1.8.9", handshake.ProtocolVersion);
                response.players.online = 10;
                response.players.max = 100;
                response.players.sample = new List<SampleItem>();
                response.description.text = "新版本测试";
                response.favicon = "";
                using (River temp = new River())
                {
                    temp.WriteInt(0);
                    temp.WriteString(JsonSerializer.Serialize(response), true);
                    byte[] packet = temp.GetBytes();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.WriteInt(packet.Length);
                        memoryStream.Write(packet);
                        ServerCore.SendPacket(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                    }
                }
                yield return false;
            }
            IsForge = handshake.ServerAddress.IndexOf("FML") > 0;
            PacketTypes = typeof(Login);
            yield return false;
            login = (Login)Parameter;
            Logger.LogInformation($"PlayerLogin : {login.Name} {DateTime.Now.ToString()}");
            TunnelServcie.Actions.Push(Login);
            ClientCore.Start();
            infoService.Info(login.Name);
            // 开始登录
            yield return false;
        }

        public void Login()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (MemoryStream packet = new MemoryStream())
                {
                    packet.WriteInt(0);
                    packet.WriteInt(handshake.ProtocolVersion);
                    packet.WriteString("mc.hypixel.net", true);
                    packet.WriteUShort(25565);
                    packet.WriteInt((int)NextState.Login);
                    memory.WriteInt((int)packet.Position);
                    packet.WriteTo(memory);
                }
                ClientCore.SendPacket(memory.ToArray());
            }
            using (MemoryStream memory = new MemoryStream())
            {
                using (MemoryStream packet = new MemoryStream())
                {
                    packet.WriteInt(0);
                    packet.WriteString(login.Name, true);

                    memory.WriteInt((int)packet.Position);
                    packet.WriteTo(memory);
                }
                ClientCore.SendPacket(memory.ToArray());
            }
        }
    }
}