using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    public interface IService
    {
        public Type PacketTypes { get; }
        public object Parameter { set; }
        public IEnumerable<bool> Handler();
    }
}