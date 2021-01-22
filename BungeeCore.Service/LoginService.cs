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
    [PacketHandler(PakcetId = 0)]
    public class LoginService : IService
    {
        private readonly ILogger Logger;
        private readonly ServerCore ServerCore;

        private Handshake handshake;
        private Login login;
        private bool IsForge;
        public Type PacketTypes { get; private set; }
        public object Parameter { set; private get; }
        public LoginService(ILogger<LoginService> Logger, ServerCore ServerCore)
        {
            this.Logger = Logger;
            this.ServerCore = ServerCore;
            PacketTypes = typeof(Handshake);
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
            // 开始登录
            yield return false;
        }
    }
}