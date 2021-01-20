using BungeeCore.Common.Attributes;
using BungeeCore.Model.ServerBound;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    [PacketHandler(PakcetId = 0)]
    public class LoginService : IService
    {
        private readonly ILogger Logger;
        public Type PacketTypes { get; private set; }
        public LoginService(ILogger<LoginService> Logger)
        {
            this.Logger = Logger;
            PacketTypes = typeof(Handshake);
        }
        public IEnumerable<bool> Handler(object obj)
        {
            Handshake handshake = (Handshake)obj;
            if (handshake.NextState() == NextState.Status)
            {
                yield break;
            }
            PacketTypes = typeof(Login);
            yield return false;
            Login login = (Login)obj;
            yield return false;
        }
    }
}
