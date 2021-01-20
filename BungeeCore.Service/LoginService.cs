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

        private int ProtocolVersion;
        public Type PacketTypes { get; private set; }
        public LoginService(ILogger<LoginService> Logger, ServerCore ServerCore)
        {
            this.Logger = Logger;
            this.ServerCore = ServerCore;
            PacketTypes = typeof(Handshake);
        }
        public IEnumerable<bool> Handler(object obj)
        {
            Handshake handshake = (Handshake)obj;
            if (handshake.NextState() == NextState.Status)
            {
                Response response = new Response("1.8.9", ProtocolVersion);
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
            PacketTypes = typeof(Login);
            yield return false;
            Login login = (Login)obj;
            yield return false;
        }
    }
}
