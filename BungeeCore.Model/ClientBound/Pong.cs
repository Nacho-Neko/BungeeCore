using BungeeCore.Common.Attributes;

namespace BungeeCore.Model.ClientBound
{
    [Packet(PakcetId = 0, Bound = Bound.Client)]
    public class Pong
    {
        public long Payload { get; set; }
        public int PacketId => 0;
    }
}