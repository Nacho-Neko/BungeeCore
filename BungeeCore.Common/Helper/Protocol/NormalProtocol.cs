namespace BungeeCore.Common.Helper.Protocol
{
    public class NormalProtocol : ProtocolHeand
    {
        public override Block block { get; set; }
        public override int PacketSize { get; set; }              // 数据包大小
        public override int PacketId { get; set; }                // 数据包Id
        public override byte[] PacketData { get; set; }           // 数据包荷载数据
        public NormalProtocol() { }
        public override void Analyze(Block block)
        {
            this.block = block;
            PacketSize = block.readVarInt();
            byte[] PacketData = block.readPacket(PacketSize);
            Block b = new Block(PacketData);
            PacketId = b.readVarInt();
            if (PacketSize > 0)
            {
                this.PacketData = b.readPacket();
            }
        }
    }
}
