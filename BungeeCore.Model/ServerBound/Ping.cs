using BungeeCore.Common.Attributes;

namespace BungeeCore.Model.ServerBound
{
    [Packet(PakcetId = 1, Bound = Bound.Server)]
    public class Ping
    {
        public long Payload { get; set; }
    }
}
