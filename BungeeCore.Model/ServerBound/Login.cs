using BungeeCore.Common.Attributes;

namespace BungeeCore.Model.ServerBound
{
    [Packet(PakcetId = 0, Bound = Bound.Server)]
    public class Login
    {
        public string Name { get; set; }
    }
}
