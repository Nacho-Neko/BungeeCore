namespace BungeeCore.Common.Helper.Protocol
{
    public abstract class ProtocolHeand
    {
        public abstract Block block { get; set; }
        public abstract int PacketSize { get; set; }
        public abstract int PacketId { get; set; }
        public abstract byte[] PacketData { get; set; }
        public abstract void Analyze(Block block);
    }
}
