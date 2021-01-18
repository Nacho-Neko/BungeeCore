using BungeeCore.Common.Extensions.Conver;
using System;
using System.IO;
using System.IO.Compression;

namespace BungeeCore.Common.Helper.Protocol
{
    public class ZipProtocol : ProtocolHeand
    {
        public override Block block { get; set; }
        public override int PacketSize { get; set; }           // 数据包中剩余的字节数，包括数据长度字段

        public int DataLength;                                 // 如果长度为0则标识当前数据包未压缩,否则为原始大小
        public override int PacketId { get; set; }             // zlib压缩的数据包Id
        public override byte[] PacketData { get; set; }        // zlib压缩的荷载数据
        public ZipProtocol() { }
        public override void Analyze(Block block)
        {
            this.block = block;
            this.PacketSize = block.readVarInt();
            this.DataLength = block.readVarInt();
            if (DataLength == 0)
            {
                this.PacketId = block.readVarInt();
                this.PacketData = block.readPacket(PacketSize);
                return;
            }
            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(block.readPacket(PacketSize));

            using MemoryStream decompressedStream = new MemoryStream();

            GZipStream zipArchive = new GZipStream(memoryStream, CompressionMode.Decompress);
            zipArchive.CopyTo(decompressedStream);

            int lenght = (int)decompressedStream.Position;
            PacketId = decompressedStream.ReadVarInt();

            int PacketLenght = lenght - (int)decompressedStream.Position;

            PacketData = new byte[PacketLenght];
            Array.Copy(decompressedStream.ToArray(), 0, PacketData, 0, PacketLenght);
        }
    }
}
