using BungeeCore.Common.Attributes;
using BungeeCore.Common.Extensions;

namespace BungeeCore.Model.ServerBound
{
    [Packet(PakcetId = 3, Bound = Bound.Server)]
    public class SetCompression
    {
        public VarInt Threshold { get; set; }
    }
}
