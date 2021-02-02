using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions.Conver;
using BungeeCore.Common.Sockets;
using BungeeCore.Model.ServerBound;
using BungeeCore.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace BungeeCore.Service
{
    [PacketHandler(PakcetId = 1, Rose = Rose.Anonymouse)]
    public class PingService : BaseService
    {
        private readonly ILogger Logger;
        private readonly ServerCore ServerCore;
        public override Type PacketTypes { get; protected set; } = typeof(Ping);
        public override object Parameter { set; protected get; }
        public PingService(ILogger<AuthService> Logger, ServerCore ServerCore)
        {
            this.Logger = Logger;
            this.ServerCore = ServerCore;
        }
        public override IEnumerable<bool> Prerouting()
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
