using BungeeCore.Common.Extensions.Conver;
using BungeeCore.Common.Sockets;
using BungeeCore.Model.ServerBound;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BungeeCore.Service
{
    public class TunnelServcie : IService
    {
        private readonly ILogger Logger;
        private readonly ClientCore ClientCore;
        public Type PacketTypes { get; private set; }
        public object Parameter { set; private get; }
        public TunnelServcie(ILogger<TunnelServcie> Logger, ClientCore ClientCore)
        {
            this.Logger = Logger;
            this.ClientCore = ClientCore;
        }
        public IEnumerable<bool> Handler()
        {
            ClientCore.Start();
            while (true)
            {
                Thread.Sleep(50);
                if (ClientCore.Connect)
                {
                    Handshake handshake = (Handshake)Parameter;
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

                    yield return false;

                    Login login = (Login)Parameter;
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
                    yield break;
                }
            }

        }
    }
}
