using System;

namespace BungeeCore.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketAttribute : Attribute
    {
        public int PakcetId { get; set; }
        public Bound Bound { get; set; }
    }
    public enum Bound : byte
    {
        Client = 1,
        Server = 2,
    }
}
