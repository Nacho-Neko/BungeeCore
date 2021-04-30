using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BungeeCore.Service
{
    public interface IService
    {
        public Type PacketTypes { get; }
        public object Parameter { set; }
        public IEnumerable<Task<bool>> Prerouting();
        public IEnumerable<Task<bool>> Postrouting();
    }
}
