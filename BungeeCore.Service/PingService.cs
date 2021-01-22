using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions.Conver;
using BungeeCore.Common.Sockets;
using BungeeCore.Model.ServerBound;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace BungeeCore.Service
{
    [PacketHandler(PakcetId = 1, Rose = Rose.Anonymouse)]
    public class PingService : IService
    {
        private readonly ILogger Logger;
        private readonly ServerCore ServerCore;
        public Type PacketTypes { get; private set; } = typeof(Ping);
        public object Parameter { set; private get; }
        public PingService(ILogger<LoginService> Logger, ServerCore ServerCore)
        {
            this.Logger = Logger;
            this.ServerCore = ServerCore;
        }
        public IEnumerable<bool> Handler()
        {
            Ping ping = (Ping)Parameter;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (MemoryStream packet = new MemoryStream())
                {
                    packet.WriteInt(1);
                    packet.WriteLong(ping.Payload);
                    memoryStream.WriteInt((int)packet.Position);
                    packet.WriteTo(memoryStream);
                }
                ServerCore.SendPacket(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
            }
            yield return false;
        }
    }
}
