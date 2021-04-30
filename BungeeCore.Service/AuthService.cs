using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions.Conver;
using BungeeCore.Common.Helper;
using BungeeCore.Model.ClientBound;
using BungeeCore.Model.ServerBound;
using BungeeCore.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using static BungeeCore.Model.ClientBound.Response;

namespace BungeeCore.Service
{
    /// <summary>
    /// 认证服务
    /// </summary>
    [PacketHandler(PakcetId = 0, Rose = Rose.Anonymouse)]
    public class AuthService : BaseService
    {
        private readonly ILogger Logger;
        private readonly ChannelService channelService;
        private readonly PlayerService playerService;
        private readonly HandlerServcie HandlerServcie;

        private Handshake handshake;
        private Login login;
        private bool IsForge;
        public override Type PacketTypes { get; protected set; } = typeof(Handshake);
        public override object Parameter { set; protected get; }
        public AuthService(ILogger<AuthService> Logger, ChannelService channelService, PlayerService playerService, HandlerServcie HandlerServcie)
        {
            this.Logger = Logger;
            this.channelService = channelService;
            this.playerService = playerService;
            this.HandlerServcie = HandlerServcie;
        }
        public override IEnumerable<Task<bool>> Prerouting()
        {
            handshake = (Handshake)Parameter;
            if (handshake.NextState() == NextState.Status)
            {
                yield return Task.FromResult(false);
                Response response = new Response("1.8.9", handshake.ProtocolVersion);
                response.players.online = 10;
                response.players.max = 100;
                response.players.sample = new List<SampleItem>();
                response.description.text = "新版本测试";
                response.favicon = "data:image/png;base64,<data>";
                using (River temp = new River())
                {
                    temp.WriteInt(0);
                    temp.WriteString(JsonSerializer.Serialize(response), true);
                    byte[] packet = temp.GetBytes();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.WriteInt(packet.Length);
                        memoryStream.Write(packet);
                        byte[] buffer = new byte[memoryStream.Position];
                        Array.Copy(memoryStream.GetBuffer(), buffer, memoryStream.Position);
                        channelService.SendPacket(buffer);
                    }
                }
                channelService.PlayerDisconnec();
                yield return Task.FromResult(false);
            }
            IsForge = handshake.ServerAddress.IndexOf("FML") > 0;
            PacketTypes = typeof(Login);
            yield return Task.FromResult(false);
            login = (Login)Parameter;
            Logger.LogInformation($"PlayerLogin : {login.Name} {DateTime.Now}");
            channelService.PlayerConnect();

            playerService.PlayerName = login.Name;
            // 开始登录
            yield return Task.FromResult(false);
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
                channelService.SendPacket(memory.ToArray());
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
                channelService.SendPacket(memory.ToArray());
            }
        }
    }
}