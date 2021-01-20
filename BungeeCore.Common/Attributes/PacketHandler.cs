using System;

namespace BungeeCore.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandler : Attribute
    {
        public int PakcetId { get; set; }
    }
}
