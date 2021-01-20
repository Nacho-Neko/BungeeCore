using System;
using System.Collections.Generic;

namespace BungeeCore.Service
{
    public interface IService
    {
        public Type PacketTypes { get; }
        public IEnumerable<bool> Handler(object obj);
    }
}
