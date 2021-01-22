using System;

namespace BungeeCore.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandler : Attribute
    {
        public int PakcetId { get; set; }
        public Rose Rose { get; set; } = Rose.Player;
    }

    public enum Rose
    {
        Anonymouse = 1,
        Player = 2,
    }
}
